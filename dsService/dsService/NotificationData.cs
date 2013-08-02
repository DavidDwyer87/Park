using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace dsService
{
    [MessageContract]
    public class NotificationData
    {
        public const string NotificationAction = "http://microsoft.com/samples/pollingDuplex/notification";

        [MessageBodyMember]
        public string Content { get; set; }
    }
}