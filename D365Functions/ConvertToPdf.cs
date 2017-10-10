using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Xrm.Sdk;
using System;

namespace D365Functions
{
    public static class ConvertToPdf
    {
        [FunctionName("ConvertToPdf")]
        public static void Run([ServiceBusTrigger("pdfconversionin", AccessRights.Manage, Connection = "SbConnectionString")]RemoteExecutionContext context, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: CorrelationId: {context.CorrelationId}");

            CrmHelper crmHelper = new CrmHelper(log);
            Entity target = (Entity)context.InputParameters["Target"];

            string entity = target.GetAttributeValue<string>("lat_recordlogicalname");
            string id = target.GetAttributeValue<string>("lat_recordid");
            EntityReference recordReference = new EntityReference(entity, new Guid(id));
            string pdfName = target.GetAttributeValue<string>("lat_newpdffilename");

            log.Info("Got values from Target");

            byte[] wordBytes = crmHelper.GetDocument(recordReference);

            log.Info("Got bytes");

            Converter converter = new Converter(wordBytes, log);
            byte[] pdfBytes = converter.ConvertToPdf();

            log.Info("Made PDF");

            Guid annotationId =
                crmHelper.CreateAnnotation(pdfBytes, pdfName, recordReference);

            log.Info($"Sent PDF to D365 - Id: {annotationId}");
        }
    }
}