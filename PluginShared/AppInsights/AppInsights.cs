using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;

public class AppInsights
{
    private readonly string _instrumentationKey;
    private readonly string _loggingEndpoint;
    private readonly string _applicationVersion;
    private readonly AiConfig _aiConfig;
    private readonly bool _logTraces;
    private readonly bool _logEvents;
    private readonly bool _logMetrics;
    private readonly bool _logExceptions;
    private readonly ITracingService _tracingService;

    /*Envelope names
    * Microsoft.ApplicationInsights.SessionState
    * Microsoft.ApplicationInsights.PerformanceCounter
    * Microsoft.ApplicationInsights.Message - Trace data
    * Microsoft.ApplicationInsights.Exception - Handled or unhandled exceptions
    * Microsoft.ApplicationInsights.Metric - Single or pre-aggregated metrics
    * Microsoft.ApplicationInsights.Event - Events that occurred in the application
    * Microsoft.ApplicationInsights.RemoteDependency - Interaction with a remote component
    * Microsoft.ApplicationInsights.Request - Events trigger by an external request
    */

    public AppInsights(AiConfig aiConfig, IOrganizationService service, ITracingService tracingService)
    {
        _instrumentationKey = aiConfig.InstrumentationKey;
        _loggingEndpoint = aiConfig.AiEndpoint;
        _applicationVersion = GetVersion(service);
        _aiConfig = aiConfig;
        _logTraces = aiConfig.LogTraces;
        _logEvents = aiConfig.LogEvents;
        _logMetrics = aiConfig.LogMetrics;
        _logExceptions = aiConfig.LogExceptions;
        _tracingService = tracingService;
    }

    public bool WriteTrace(string message, string operationName, AiTraceSeverity severity)
    {
        if (!_logTraces)
            return true;

        string json = GetTraceJsonString(DateTime.UtcNow, _instrumentationKey, message, operationName, severity);
        return SendToAi(json);
    }

    public bool WriteEvent(string eventName, string operationName)
    {
        if (!_logEvents)
            return true;

        string json = GetEventJsonString(DateTime.UtcNow, _instrumentationKey, eventName, operationName);
        return SendToAi(json);
    }

    public bool WriteException(Exception e, string operationName, AiExceptionSeverity severity)
    {
        if (!_logExceptions)
            return true;

        string json = GetExceptionJsonString(DateTime.UtcNow, _instrumentationKey, e, operationName, severity);
        return SendToAi(json);
    }

    public bool WriteMethodPerfMetric(string methodName, long duration, string operationName)
    {
        if (!_logMetrics)
            return true;

        string json = GetMethodPerfMetricJsonString(DateTime.UtcNow, _instrumentationKey, methodName, duration, operationName);
        return SendToAi(json);
    }

    private bool SendToAi(string json)
    {
        try
        {
            using (HttpClient client = HttpHelper.GetHttpClient())
            {
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/x-json-stream");
                HttpResponseMessage response = client.PostAsync(_loggingEndpoint, content).Result;

                if (response.IsSuccessStatusCode)
                    return true;

                _tracingService?.Trace($"Error writing to Application Insights with response: {response.StatusCode.ToString()} Message: {json}");
                return false;
            }
        }
        catch (Exception e)
        {
            _tracingService?.Trace($"Error writing to Application Insights: Message: {json}", e);
            return false;
        }
    }

    private string GetTraceJsonString(DateTime time, string instrumentationKey, string message, string operationName, AiTraceSeverity severity)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{instrumentationKey}.Message",
            Time = $"{time:O}",
            InstrumentationKey = instrumentationKey,
            Tags = new AiTags
            {
                OperationName = operationName,
                RoleInstance = ""
            },
            Data = new AiData
            {
                BaseType = "MessageData",
                BaseData = new AiBaseData
                {
                    Version = 2,
                    ApplicationVersion = _applicationVersion,
                    Message = message,
                    SeverityLevel = severity.ToString(),
                    Properties = AiProperties.GetPropertiesFromConfig(_aiConfig)
                }
            }
        };

        var json = Serialization.SerializeRequest<AiLogRequest>(logRequest);
        return json;
    }

    private string GetEventJsonString(DateTime time, string instrumentationKey, string eventName, string operationName)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{instrumentationKey}.Event",
            Time = $"{time:O}",
            InstrumentationKey = instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = "",
                OperationName = operationName
            },
            Data = new AiData
            {
                BaseType = "EventData",
                BaseData = new AiBaseData
                {
                    Version = 2,
                    ApplicationVersion = _applicationVersion,
                    Name = eventName,
                    Properties = AiProperties.GetPropertiesFromConfig(_aiConfig)
                }
            }
        };

        var json = Serialization.SerializeRequest<AiLogRequest>(logRequest);
        return json;
    }

    private string GetExceptionJsonString(DateTime time, string instrumentationKey, Exception e, string operationName, AiExceptionSeverity severity)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{instrumentationKey}.Exception",
            Time = $"{time:O}",
            InstrumentationKey = instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = "",
                OperationName = operationName
            },
            Data = new AiData
            {
                BaseType = "ExceptionData",
                BaseData = new AiBaseData
                {
                    Version = 2,
                    ApplicationVersion = _applicationVersion,
                    SeverityLevel = severity.ToString(),
                    Properties = AiProperties.GetPropertiesFromConfig(_aiConfig),
                    Exceptions = new List<AiException> {
                        ExceptionHelper.GetAiException(e)
                    }
                }
            }
        };

        var json = Serialization.SerializeRequest<AiLogRequest>(logRequest);
        return json;
    }

    private string GetMethodPerfMetricJsonString(DateTime time, string instrumentationKey, string methodName, double duration, string operationName)
    {
        AiLogRequest logRequest = new AiLogRequest
        {
            Name = $"Microsoft.ApplicationInsights.{instrumentationKey}.Metric",
            Time = $"{time:O}",
            InstrumentationKey = instrumentationKey,
            Tags = new AiTags
            {
                RoleInstance = "",
                OperationName = operationName
            },
            Data = new AiData
            {
                BaseType = "MetricData",
                BaseData = new AiBaseData
                {
                    Version = 2,
                    ApplicationVersion = _applicationVersion,
                    Metrics = new List<AiDataPoint> {
                        new AiDataPoint {
                            Kind = DataPointType.Measurement,
                            Name = methodName,
                            Value = duration
                        }
                    },
                    Properties = AiProperties.GetPropertiesFromConfig(_aiConfig)
                }
            }
        };

        var json = Serialization.SerializeRequest<AiLogRequest>(logRequest);
        return json;
    }

    private string GetVersion(IOrganizationService service)
    {
        RetrieveVersionRequest request = new RetrieveVersionRequest();
        RetrieveVersionResponse response = (RetrieveVersionResponse)service.Execute(request);

        return response.Version;
    }
}