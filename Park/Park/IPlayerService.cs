using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections;

namespace Park
{

    [ServiceContract]
    public interface IPlayNotification
    {
        [OperationContract(IsOneWay = true)]
        void Notify(string message);
    }

    [ServiceContract(Namespace=""/*,CallbackContract= typeof(IPlayNotification)*/)]
    public interface IPlayerService
    {
        [OperationContract]
        void Subscribe();

        [OperationContract]
        void Publish(string message);
    }   
}
