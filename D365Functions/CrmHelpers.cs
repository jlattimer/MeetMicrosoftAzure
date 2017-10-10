using Microsoft.Azure.WebJobs.Host;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace D365Functions
{
    public class CrmHelper
    {
        private readonly CrmServiceClient _client;
        private readonly TraceWriter _log;

        public CrmHelper(TraceWriter log)
        {
            string crmConnString = Environment.GetEnvironmentVariable("D365ConnectionString", EnvironmentVariableTarget.Process);
            _client = new CrmServiceClient(crmConnString);
            _log = log;
        }

        public byte[] GetDocument(EntityReference entityReference)
        {
            try
            {
                QueryExpression query = new QueryExpression
                {
                    EntityName = "annotation",
                    ColumnSet = new ColumnSet("documentbody"),
                    Criteria = new FilterExpression
                    {
                        Conditions = {
                            new ConditionExpression {
                                AttributeName = "isdocument",
                                Values = { true }
                            },
                            new ConditionExpression {
                                AttributeName = "subject",
                                Operator = ConditionOperator.Null
                            },
                            new ConditionExpression {
                                AttributeName = "notetext",
                                Operator = ConditionOperator.Null
                            },
                            new ConditionExpression {
                                AttributeName = "objectid",
                                Operator = ConditionOperator.Equal,
                                Values = { entityReference.Id }
                            },
                            new ConditionExpression {
                                AttributeName = "objecttypecode",
                                Operator = ConditionOperator.Equal,
                                Values = { entityReference.LogicalName }
                            }
                        }
                    },
                    TopCount = 1,
                    Orders = {
                        new OrderExpression("createdon",  OrderType.Descending)
                    }
                };

                EntityCollection results = _client.RetrieveMultiple(query);

                if (results.Entities.Count == 0)
                    return null;

                string b64Body = results.Entities[0].GetAttributeValue<string>("documentbody");
                if (string.IsNullOrEmpty(b64Body))
                    return null;

                byte[] bytes = Convert.FromBase64String(b64Body);
                return bytes;
            }
            catch (Exception e)
            {
                _log.Error($"Error {e.Message} retrieving notes: {e.StackTrace}");
                throw;
            }
        }

        public Guid CreateAnnotation(byte[] bytes, string filename, EntityReference entityReference)
        {
            try
            {
                Guid attachmentId;

                Entity note = new Entity("annotation")
                {
                    ["subject"] = "New PDF",
                    ["filename"] = $"{filename}.pdf",
                    ["documentbody"] = Convert.ToBase64String(bytes),
                    ["mimetype"] = "application/pdf",
                    ["objectid"] = entityReference,
                    ["isdocument"] = true
                };

                using (_client)
                {
                    attachmentId = _client.Create(note);
                }

                return attachmentId;
            }
            catch (Exception e)
            {
                _log.Error($"Error {e.Message} creating note: {e.StackTrace}");
                throw;
            }
        }
    }
}