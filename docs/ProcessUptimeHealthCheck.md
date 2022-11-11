---
sidebar_position: 13
sidebar_label: 'Process Up time HealthCheck'
---

# ProcessUptimeHealthCheck

This health check records the uptime of the current process. Results will alwas have a status of `Pass`.

None of its constructor's parameters are required.

Parameter          | Default Value          | Description
------------------ | ---------------------- | -----------
componentName      | `"process"`            | The name of the logical downstream dependency or sub-component of a service. Must not contain a colon.
measurementName    | `"uptime"`             | The name of the measurement that the status is reported for. Must not contain a colon.
componentType      | `"system"`             | The type of the component.
componentId        | `null`                 | A unique identifier of an instance of a specific sub-component/dependency of a service.
