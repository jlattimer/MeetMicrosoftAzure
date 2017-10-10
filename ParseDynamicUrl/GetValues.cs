using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace ParseDynamicUrl
{
    public class GetValues : CodeActivity
    {
        [Input("Record Dynamic Url")]
        [RequiredArgument]
        public InArgument<string> RecordUrl { get; set; }

        [Output("Record Guid")]
        public OutArgument<string> RecordGuid { get; set; }

        [Output("Record Entity Logical Name")]
        public OutArgument<string> EntityLogicalName { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracer = executionContext.GetExtension<ITracingService>();
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                var entityReference = new UrlParser(RecordUrl.Get<string>(executionContext));

                RecordGuid.Set(executionContext, entityReference.Id.ToString());
                EntityLogicalName.Set(executionContext, entityReference.GetEntityLogicalName(service));
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}