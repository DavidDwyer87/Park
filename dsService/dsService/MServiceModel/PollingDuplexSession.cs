using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ServiceModel.PollingDuplex.Scalable
{
    public class PollingDuplexSession
    {
        public string Address { get; set; }
        public string SessionId { get; set; }

        public PollingDuplexSession(string clientId, string sessionId)
        {
            this.Address = clientId;
            this.SessionId = sessionId;
        }
    }
}
