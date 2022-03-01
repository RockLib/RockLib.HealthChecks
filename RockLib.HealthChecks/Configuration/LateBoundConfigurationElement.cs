#if NET35 || NET40 || NET45
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
#if NET35
using RockLib.Immutable;
#endif

namespace RockLib.HealthChecks.Configuration
{
    /// <summary>
    /// A configuration section capable of creating objects that are defined by the
    /// unrecognized elements and attributes of the configuration section.
    /// </summary>
    /// <typeparam name="TTarget">The type of object to create.</typeparam>
    public class LateBoundConfigurationElement<TTarget> : ConfigurationElement
    {
        private static readonly Regex _enumFlagsRegex = new Regex(@"(?:\s*\|\s*)|(?:\s+or\s+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly List<XElement> _additionalElements = new List<XElement>();
        private readonly List<XAttribute> _additionalAttributes = new List<XAttribute>();

        private readonly Lazy<Type> _type;
        private readonly Type _defaultType;

        private string _elementTagName;

        /// <summary>
        /// Initialize a new instance of the <see cref="LateBoundConfigurationElement{TTarget}"/> class.
        /// </summary>
        public LateBoundConfigurationElement()
            : this(null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="LateBoundConfigurationElement{TTarget}"/> class
        /// with the specified default type.
        /// </summary>
        /// <param name="defaultType">The default type of object to create.</param>
        public LateBoundConfigurationElement(Type defaultType)
        {
            if (defaultType is null && !typeof(TTarget).IsAbstract)
            {
                _defaultType = typeof(TTarget);
            }

            _defaultType = ThrowIfNotAssignableToT(defaultType);
            _type = new Lazy<Type>(() => Type.GetType(TypeAssemblyQualifiedName, true));
            if (_defaultType is not null)
                this["type"] = _defaultType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the type of object to create.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeAssemblyQualifiedName => (string)this["type"];

        /// <summary>
        /// Creates an object of the type defined by the <see cref="TypeAssemblyQualifiedName"/> property.
        /// </summary>
        /// <returns>An object of the type defined by the <see cref="TypeAssemblyQualifiedName"/> property.</returns>
        public TTarget CreateInstance()
        {
            Type type;

            try
            {
                type = _type.Value;
            }
            catch (Exception ex)
            {
                var xml = SerializeToXml();

                if (TypeAssemblyQualifiedName is null)
                {
                    throw new LateBoundConfigurationElementException("The required 'type' attribute was not provided.", ex, xml);
                }

                throw new LateBoundConfigurationElementException(string.Format(
                    "The value provided for the required 'type' attribute, '{0}', is not a valid assembly-qualified name.",
                    TypeAssemblyQualifiedName), ex, xml);
            }

            var creationScenario = GetCreationScenario(type);
            var args = CreateArgs(creationScenario.Parameters);
            var instance = creationScenario.Constructor.Invoke(args);

            foreach (var property in creationScenario.Properties)
            {
                object value;

                if (TryGetValueForInstance(property.Name, property.PropertyType, out value))
                {
                    property.SetValue(instance, value, null);
                }
            }

            return (TTarget)instance;
        }

        /// <inheritdoc/>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            _additionalAttributes.Add(new XAttribute(name, value));
            return true;
        }

        /// <inheritdoc/>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            var element = XElement.Load(reader.ReadSubtree());
            _additionalElements.Add(element);
            return true;
        }

        /// <inheritdoc/>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            _elementTagName = reader.Name;
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        internal string SerializeToXml()
        {
            const SaveOptions saveOptions =
#if NET35
                SaveOptions.None;
#else
                SaveOptions.OmitDuplicateNamespaces;
#endif

            try
            {
                var sb = new StringBuilder();

                using (var stringWriter = new StringWriter(sb))
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                    {
                        SerializeToXmlElement(xmlWriter, _elementTagName);
                    }
                }

                var xElement = XElement.Parse(sb.ToString());

                foreach (var attribute in _additionalAttributes)
                {
                    xElement.Add(attribute);
                }

                foreach (var element in _additionalElements)
                {
                    xElement.Add(element);
                }

                return xElement.ToString(saveOptions);
            }
            catch
            {
                return null;
            }
        }

        private CreationScenario GetCreationScenario(Type type)
        {
            return new CreationScenario(GetConstructor(type), type);
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            // The constructor with the most resolvable parameters wins.
            // If tied, the constructor with fewest parameters wins.

            return
               (from ctor in type.GetConstructors()
                let parameters = ctor.GetParameters()
                orderby
                    parameters.Count(CanResolveParameterValue) descending,
                    parameters.Length ascending
                select ctor).First();
        }

        private bool CanResolveParameterValue(ParameterInfo parameter)
        {
            object dummy;
            if (TryGetValueForInstance(parameter.Name, parameter.ParameterType, out dummy))
            {
                return true;
            }

            return false;
        }

        private object[] CreateArgs(IEnumerable<ParameterInfo> parameters)
        {
            var argsList = new List<object>();

            foreach (var parameter in parameters)
            {
                object argValue;

                if (TryGetValueForInstance(parameter.Name, parameter.ParameterType, out argValue))
                {
                    argsList.Add(argValue);
                }
                else
                {
                    var hasDefaultValue = (parameter.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;

                    argsList.Add(hasDefaultValue ? parameter.DefaultValue : null);
                }
            }

            return argsList.ToArray();
        }

        private bool TryGetValueForInstance(string name, Type type, out object value)
        {
            if (TryGetPropertyValue(name, type, out value))
            {
                if (value is null)
                {
                    object additionalValue;
                    if (TryGetAdditionalValue(name, type, out additionalValue))
                    {
                        value = additionalValue;
                    }
                }

                return true;
            }

            if (TryGetAdditionalValue(name, type, out value))
            {
                return true;
            }

            return false;
        }

        private bool TryGetPropertyValue(string name, Type type, out object value)
        {
            var properties =
                GetType().GetProperties()
                    .Where(p => p.DeclaringType != typeof(ConfigurationElement) && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == type)
                    .OrderBy(p => p.Name, new CaseSensitiveEqualityFirstAsComparedTo(name));

            foreach (var property in properties)
            {
                try
                {
                    value = property.GetValue(this, null);

                    if (value is not null)
                    {
                        return true;
                    }
                }                                                                                                                                   // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }

            value = null;
            return false;
        }

        private bool TryGetAdditionalValue(string name, Type type, out object value)
        {
            foreach (var additionalNode in GetAdditionalNodes(name))
            {
                var additionalElement = additionalNode as XElement;

                if (additionalElement is not null)
                {
                    if (TryGetElementValue(additionalElement, type, out value))
                    {
                        return true;
                    }
                }
                else
                {
                    var additionalAttribute = (XAttribute)additionalNode;

                    if (TryConvert(additionalAttribute.Value, type, out value))
                    {
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        private IEnumerable<XObject> GetAdditionalNodes(string name)
        {
            var allAdditionalNodes =
                (_additionalElements as IEnumerable<XObject> ?? Enumerable.Empty<XObject>())
                    .Concat(_additionalAttributes as IEnumerable<XObject> ?? Enumerable.Empty<XObject>());

            var additionalNodes = allAdditionalNodes
                .Where(x => GetName(x).Equals(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(GetName, new CaseSensitiveEqualityFirstAsComparedTo(name))
                .ThenBy(x => x, new ElementsBeforeAttributes());

            return additionalNodes;
        }

        private static string GetName(XObject xObject)
        {
            var xElement = xObject as XElement;
            if (xElement is not null)
            {
                return xElement.Name.ToString();
            }

            var xAttribute = xObject as XAttribute;
            if (xAttribute is not null)
            {
                return xAttribute.Name.ToString();
            }

            return null;
        }

        private static bool TryGetElementValue(XElement additionalElement, Type type, out object value)
        {
            if (!additionalElement.HasAttributes
                && (!additionalElement.Nodes().Any()
                    || additionalElement.Nodes().All(node => node.NodeType != XmlNodeType.Element)))
            {
                var reader = additionalElement.CreateReader();
                reader.MoveToContent();
                var innerXml = reader.ReadInnerXml();

                if (TryConvert(innerXml, type, out value))
                {
                    return true;
                }
            }

            using (var reader = new StringReader(additionalElement.ToString()))
            {
                try
                {
                    XmlSerializer serializer = null;

                    var typeAttribute = additionalElement.Attribute("type");

                    if (typeAttribute is not null)
                    {
                        var typeName = typeAttribute.Value;
                        var typeFromAttribute = Type.GetType(typeName);

                        if (typeFromAttribute is not null)
                        {
                            serializer = new XmlSerializer(typeFromAttribute, new XmlRootAttribute(additionalElement.Name.ToString()));
                        }
                    }

                    if (serializer is null)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            value = null;
                            return false;
                        }

                        serializer = new XmlSerializer(type, new XmlRootAttribute(additionalElement.Name.ToString()));
                    }

                    value = serializer.Deserialize(reader);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }
        }

        private static bool TryConvert(string stringValue, Type type, out object value)
        {
            if (type == typeof(string))
            {
                value = stringValue;
                return true;
            }

            var converter = TypeDescriptor.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                if (type.IsEnum)
                {
                    stringValue = _enumFlagsRegex.Replace(stringValue, ",");
                }

                value = converter.ConvertFrom(stringValue);
                return true;
            }

            value = null;
            return false;
        }

        private static Type ThrowIfNotAssignableToT(Type type)
        {
            if (type is null)
            {
                return null;
            }

            if (!typeof(TTarget).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("The provided Type, {0}, must be assignable to Type {1}.", type, typeof(TTarget)));
            }

            return type;
        }

        private class CreationScenario
        {
            private readonly ConstructorInfo _ctor;
            private readonly ParameterInfo[] _parameters;
            private readonly IEnumerable<PropertyInfo> _properties;

            public CreationScenario(ConstructorInfo ctor, Type type)
            {
                _ctor = ctor;
                _parameters = ctor.GetParameters();

                var parameterNames = _parameters.Select(p => p.Name).ToList();

                _properties =
                    type.GetProperties()
                        .Where(p =>
                            p.CanRead
                            && p.CanWrite
                            && p.GetGetMethod(true).IsPublic
                            && p.GetSetMethod(true).IsPublic
                            && parameterNames.All(parameterName => !parameterName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
            }

            public ConstructorInfo Constructor { get { return _ctor; } }
            public IEnumerable<ParameterInfo> Parameters { get { return _parameters; } }
            public IEnumerable<PropertyInfo> Properties { get { return _properties; } }
        }

        private class CaseSensitiveEqualityFirstAsComparedTo : IComparer<string>
        {
            private readonly string _nameToMatch;

            public CaseSensitiveEqualityFirstAsComparedTo(string nameToMatch)
            {
                _nameToMatch = nameToMatch;
            }

            public int Compare(string lhs, string rhs)
            {
                if (string.Equals(lhs, rhs, StringComparison.Ordinal))
                {
                    return 0;
                }

                if (string.Equals(lhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return -1;
                }

                if (string.Equals(rhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return 1;
                }

                return 0;
            }
        }

        private class ElementsBeforeAttributes : IComparer<XObject>
        {
            public int Compare(XObject lhs, XObject rhs)
            {
                if (lhs is XElement)
                {
                    return rhs is XAttribute ? -1 : 0;
                }

                return rhs is XElement ? 1 : 0;
            }
        }
    }

    /// <summary>
    /// An exception that is thrown then the <see cref="LateBoundConfigurationElement{TTarget}.CreateInstance"/>
    /// method fails.
    /// </summary>
    public class LateBoundConfigurationElementException : InvalidOperationException
    {
        private readonly string _invalidElementXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="LateBoundConfigurationElementException"/>
        /// class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="invalidElementXml">The reserialized xml of the configuration element.</param>
        public LateBoundConfigurationElementException(string message, Exception innerException, string invalidElementXml)
            : base(message, innerException)
        {
            _invalidElementXml = invalidElementXml;
        }

        /// <summary>
        /// Gets the reserialized xml of the invalid element from the configuration.
        /// </summary>
        public string InvalidElementXml
        {
            get { return _invalidElementXml; }
        }
    }
}
#endif
