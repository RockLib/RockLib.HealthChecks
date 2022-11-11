---
sidebar_position: 14
sidebar_label: 'SystemUptime HealthCheck'
---

# SystemUptimeHealthCheck

This health check records the uptime of the system. Results will always have a status of `Pass`.

None of its constructor's parameters are required.

Parameter          | Default Value          | Description
------------------ | ---------------------- | -----------
componentName      | `"system"`            | The name of the logical downstream dependency or sub-component of a service. Must not contain a colon.
measurementName    | `"uptime"`             | The name of the measurement that the status is reported for. Must not contain a colon.
componentType      | `"system"`             | The type of the component.
componentId        | `null`                 | A unique identifier of an instance of a specific sub-component/dependency of a service.
