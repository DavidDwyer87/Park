using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace dsService
{
    [ServiceContract]
    public interface ICallBack
    {
        [OperationContract(IsOneWay=true)]
        void CallBack(string message);
    }

    [ServiceContract(Namespace = "",CallbackContract=typeof(ICallBack))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1
    {
        Dictionary<string, SynchronizedCollection<ICallBack>> client = new Dictionary<string, SynchronizedCollection<ICallBack>>();

        [OperationContract]
        public void Subscribe(string roomName)
        {
            if (client.ContainsKey(roomName))
                client["hello"].Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
            else
            {
                client.Add(roomName, new SynchronizedCollection<ICallBack>());
                client[roomName].Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
            }
        }

        [OperationContract]
        public void Publish(string roomName, string message)
        {
            if (client.ContainsKey(roomName))
                foreach (ICallBack ic in client[roomName])
                {
                    ic.CallBack(message);
                }
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
