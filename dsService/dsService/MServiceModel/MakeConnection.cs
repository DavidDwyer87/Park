using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.PollingDuplex.Scalable
{
    [MessageContract(WrapperName = "MakeConnection", WrapperNamespace = "http://docs.oasis-open.org/ws-rx/wsmc/200702")]
    public class MakeConnection
    {
        [MessageBodyMember(Name = "Address", Namespace = "http://docs.oasis-open.org/ws-rx/wsmc/200702")]
        public string Address { get; set; }

        public Message PrepareRespose(Message response, string sessionId)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            if (sessionId == null)
            {
                throw new ArgumentNullException("sessionId");
            }

            DuplexHeader.AddToMessage(response, this.Address, sessionId);

            return response;
        }

        public Message CreateEmptyResponse()
        {
            Message response = Message.CreateMessage(MessageVersion.Default, null, (object)null);

            HttpResponseMessageProperty http = new HttpResponseMessageProperty();
            http.StatusCode = System.Net.HttpStatusCode.OK;
            http.SuppressEntityBody = true;
            response.Properties.Add(HttpResponseMessageProperty.Name, http);

            return response;
        }
    }
}
