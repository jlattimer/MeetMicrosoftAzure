using Microsoft.Xrm.Sdk;
using System;
using System.Reflection;
using System.Threading;

namespace AppInsightsPlugin
{
    public class LogTest : PluginBase
    {
        #region Secure/Unsecure Configuration

        private string _secureConfig;
        private string _unsecureConfig;

        public LogTest(string unsecureConfig, string secureConfig) : base(typeof(LogTest))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }

        #endregion

        [Time]
        protected override void ExecutePlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new InvalidPluginExecutionException("localContext");

            AppInsights aiLogger = SetupAi(localContext);

            aiLogger.WriteTrace("Entering Method", LogHelper.GetFullMethodName(MethodBase.GetCurrentMethod()), AiTraceSeverity.Information);

            try
            {

                Entity target = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];

                Random random = new Random();
                Thread.Sleep(random.Next(5000));

            }
            finally
            {
                aiLogger.WriteTrace("Exiting Method", LogHelper.GetFullMethodName(MethodBase.GetCurrentMethod()), AiTraceSeverity.Information);
            }
        }

        [Time]
        public void DoException(int x, AppInsights aiLogger)
        {
            aiLogger.WriteTrace("Entering Method", LogHelper.GetFullMethodName(MethodBase.GetCurrentMethod()), AiTraceSeverity.Information);

            try
            {
                var z = 1 / x;
            }
            catch (Exception e) {
                aiLogger.WriteException(e, LogHelper.GetFullMethodName(MethodBase.GetCurrentMethod()), AiExceptionSeverity.Error);
            }
            finally
            {
                aiLogger.WriteTrace("Exiting Method", LogHelper.GetFullMethodName(MethodBase.GetCurrentMethod()), AiTraceSeverity.Information);
            }
        }

        [Time]
        private AppInsights SetupAi(LocalPluginContext localContext)
        {
            AiSecureConfig aiSecureConfig = Serialization.DeserializeResponse<AiSecureConfig>(_secureConfig);

            AiConfig aiConfig = new AiConfig
            {
                InstrumentationKey = aiSecureConfig.InstrumentationKey,
                AiEndpoint = aiSecureConfig.AiEndpoint,
                CorrelationId = localContext.PluginExecutionContext.CorrelationId,
                InitiatingUserId = localContext.PluginExecutionContext.InitiatingUserId,
                UserId = localContext.PluginExecutionContext.UserId,
                EntityName = localContext.PluginExecutionContext.PrimaryEntityName,
                EntityId = localContext.PluginExecutionContext.PrimaryEntityId,
                Message = localContext.PluginExecutionContext.MessageName,
                Stage = localContext.PluginExecutionContext.Stage,
                Mode = localContext.PluginExecutionContext.Mode,
                Depth = localContext.PluginExecutionContext.Depth,
                LogTraces = aiSecureConfig.LogTraces,
                LogEvents = aiSecureConfig.LogEvents,
                LogMetrics = aiSecureConfig.LogMetrics,
                LogExceptions = aiSecureConfig.LogExceptions
            };

            AppInsights aiLogger = new AppInsights(aiConfig, localContext.OrganizationService, localContext.TracingService);

            MethodTimeLogger.AiLogger = aiLogger;

            return aiLogger;
        }
    }
}