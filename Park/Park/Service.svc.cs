using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Generic;

namespace Park
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service :IPlayerService
    {
        SynchronizedCollection<IPlayNotification> clients = new System.Collections.Generic.SynchronizedCollection<IPlayNotification>();

        public void Subscribe()
        {
            clients.Add(OperationContext.Current.GetCallbackChannel<IPlayNotification>()); 
        }

        public void Publish(string message)
        {
            foreach (IPlayNotification channel in clients)
            {
                if (channel != OperationContext.Current.GetCallbackChannel<IPlayNotification>())
                {
                    channel.Notify(message);
                }
            }
        }
    }
}