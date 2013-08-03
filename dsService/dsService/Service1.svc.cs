using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

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
        //Dictionary<string, SynchronizedCollection<ICallBack>> client = new Dictionary<string, SynchronizedCollection<ICallBack>>();
        int i = 0;
        SynchronizedCollection<ICallBack> client = new SynchronizedCollection<ICallBack>();

        public Service1()
        {
           
        }

        [OperationContract]
        public void Subscribe()
        {
            if (!client.Contains(OperationContext.Current.GetCallbackChannel<ICallBack>()))
                client.Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
            /*else
            {
                client.Add(roomName, new SynchronizedCollection<ICallBack>());
                client[roomName].Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
            }*/
        }

        [OperationContract]
        public void Publish(string message)
        {           
                foreach (ICallBack ic in client)
                {
                    ic.CallBack(message);
                }
        }        
    }
}
