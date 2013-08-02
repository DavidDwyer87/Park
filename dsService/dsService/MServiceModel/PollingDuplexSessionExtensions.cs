using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Microsoft.ServiceModel.PollingDuplex.Scalable
{
    public static class PollingDuplexSessionExtensions
    {
        public static PollingDuplexSession GetPollingDuplexSession(this OperationContext context)
        {
            return DuplexHeader.FindHeader(context.IncomingMessageHeaders);
        }
    }
}
