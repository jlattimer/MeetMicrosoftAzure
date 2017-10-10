using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using XrmMoq;

namespace PluginShared.Tests
{
    [TestClass]
    public class AppInsightsTests
    {
        private static readonly AiSecureConfig AiSecureConfig = new AiSecureConfig
        {
            AiEndpoint = "https://dc.services.visualstudio.com/v2/track",
            InstrumentationKey = "{Your Value}"
        };

        private static readonly CrmPluginMock PluginMock = CreatePluginMock();

        private readonly AppInsights _aiLogger = new AppInsights(CreateAiConfig(), PluginMock.FakeOrganizationService, PluginMock.FakeTracingService);

        private static AiConfig CreateAiConfig()
        {
            AiConfig aiConfig = new AiConfig
            {
                InstrumentationKey = AiSecureConfig.InstrumentationKey,
                AiEndpoint = AiSecureConfig.AiEndpoint,
                CorrelationId = PluginMock.PluginExecutionContext.CorrelationId,
                InitiatingUserId = PluginMock.PluginExecutionContext.InitiatingUserId,
                UserId = PluginMock.PluginExecutionContext.UserId,
                EntityName = PluginMock.PluginExecutionContext.PrimaryEntityName,
                EntityId = PluginMock.PluginExecutionContext.PrimaryEntityId,
                Message = PluginMock.PluginExecutionContext.MessageName,
                Depth = PluginMock.PluginExecutionContext.Depth,
                Mode = PluginMock.PluginExecutionContext.Mode,
                Stage = PluginMock.PluginExecutionContext.Stage,
                LogEvents = true,
                LogMetrics = true,
                LogExceptions = true,
                LogTraces = true
            };

            return aiConfig;
        }

        private static CrmPluginMock CreatePluginMock()
        {
            Guid userId = Guid.NewGuid();

            CrmPluginMock pluginMock = new CrmPluginMock
            {
                PluginExecutionContext = {
                    CorrelationId = Guid.NewGuid(),
                    PrimaryEntityId = Guid.NewGuid(),
                    PrimaryEntityName = "account",
                    InitiatingUserId = userId,
                    UserId = userId,
                    MessageName = MessageName.create.ToString(),
                    Depth = 1,
                    Mode = (int)PluginMode.Asynchronous,
                    Stage = (int)Stage.PostOperation
                }
            };

            OrganizationResponse retrieveVersionResponse = new RetrieveVersionResponse
            {
                Results = new ParameterCollection
                {
                    {"Version", "8.2.1.360"}
                }
            };

            pluginMock.SetMockExecutes(retrieveVersionResponse);

            return pluginMock;
        }

        [TestMethod]
        public void Send_trace()
        {
            bool result = _aiLogger.WriteTrace("Entering Method", "AppInsightsPlugin.LogTest.ExecutePlugin", AiTraceSeverity.Information);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Send_event()
        {
            bool result = _aiLogger.WriteEvent("Test Event", "AppInsightsPlugin.EventTest.ExecutePlugin");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Send_exception()
        {
            bool result = false;
            try
            {
                var x = 0;
                var z = 2 / x;
            }
            catch (Exception e)
            {
                result = _aiLogger.WriteException(e, "AppInsightsPlugin.ExceptionTest.ExecutePlugin", AiExceptionSeverity.Error);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Send_performance_metric()
        {
            bool result = _aiLogger.WriteMethodPerfMetric("TestMethod", 4543, "AppInsightsPlugin.PerfTest.ExecutePlugin");

            Assert.IsTrue(result);
        }
    }
}