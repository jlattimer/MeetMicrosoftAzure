using Microsoft.Azure.WebJobs.Host;
using Spire.Doc;
using System;
using System.IO;

namespace D365Functions
{
    public class Converter
    {
        private readonly byte[] _bytes;
        private readonly TraceWriter _log;
        public Converter(byte[] bytes, TraceWriter log)
        {
            _bytes = bytes;
            _log = log;
        }

        public byte[] ConvertToPdf()
        {
            try
            {
                using (Stream stream = new MemoryStream(_bytes))
                {
                    Document d = new Document(stream);
                    ToPdfParameterList tpl = new ToPdfParameterList
                    {
                        UsePSCoversion = true
                    };
                   
                    using (MemoryStream ms = new MemoryStream())
                    {
                        d.SaveToStream(ms, tpl);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Error {e.Message} ConvertToPdf: {e.StackTrace}");
                throw;
            }
        }
    }
}