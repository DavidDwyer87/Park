using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Twilio;
using System.Configuration;

namespace dsService
{
    [ServiceContract]
    public interface ICallBack
    {
        //[OperationContract(IsOneWay=true)]
        //void CallBack(string message);

        [OperationContract(IsOneWay = true, AsyncPattern = true)]
        IAsyncResult BeginNotifyProcess(string message, AsyncCallback callback, object asyncState);

        void EndNotifyProcess(IAsyncResult iar);
    }

    [ServiceContract(Namespace = "",CallbackContract=typeof(ICallBack))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode=ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1
    {
        Dictionary<string, SynchronizedCollection<ICallBack>> client = new Dictionary<string, SynchronizedCollection<ICallBack>>();
        Dictionary<ICallBack, int> ping = new Dictionary<ICallBack, int>();

        Queue<ICallBack> wait_list = new Queue<ICallBack>();
        Queue<string> queueIn = new Queue<string>();
        Queue<string> queueOut = new Queue<string>();

        Timer MailOutTimer = null;
        Timer OperationTimer = null;

        bool switch_for_timer = false;

        SqlConnection conn = new SqlConnection();
        List<ICallBack> dead = new List<ICallBack>();
            
        public Service1()
        {
            try
            {
                MailOutTimer = new Timer(new TimerCallback(MailOut), null, 2000, 1000 * 5);
                OperationTimer = new Timer(new TimerCallback(OperationProcess), null, 3000, 2000);

                conn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                conn.Open();
            }
            catch (Exception ex)
            {
                string data = ex.Message;
                //var twilio = new TwilioRestClient("AC7b4d1e8619bf4a5afbf3e329488ffb6f", "9b51447e7609aa7ae54ec3cb22d2b1f7");
                //twilio.SendSmsMessage("+18065896495", "18764476773", "Alert there is no access to the database for Domino Park. Someone get on it now!");
                //twilio.SendSmsMessage("+18065896495", "18768713746", "Alert there is no access to the database for Domino Park. Someone get on it now!");
            }
        }

        void MailOut(object obj)
        {
            try
            {
                if (queueOut.Count > 0)
                {
                    for (int i = 0; i < queueOut.Count; i++)
                    {
                        string data = queueOut.Dequeue();
                        string[] ends = data.Split(',');

                        if(client.ContainsKey(ends[0]))
                        {
                            foreach(ICallBack ic in client[ends[0]])
                            {
                                IAsyncResult iar = ic.BeginNotifyProcess(ends[1], new AsyncCallback(onMessageComplete), new messageState(ic));
                                if (iar.CompletedSynchronously)
                                {
                                    messageComplete(iar);
                                }
                            }
                        }
                    }

                    //new Thread(new ThreadStart(remove_dead_Channel)).Start();
                }

                foreach (ICallBack ic in ping.Keys)
                {
                    if (ping[ic] > 200)
                    {
                        IAsyncResult iar = ic.BeginNotifyProcess("check", new AsyncCallback(onMessageComplete), new messageState(ic));
                        if (iar.CompletedSynchronously)
                        {
                            messageComplete(iar);
                        }                        
                    }
                }
                
            }
            catch (Exception ex) {
                //new SqlCommand(""+ex.Message, conn).ExecuteNonQuery();
            }
        }

        void OperationProcess(object obj)
        {
            try
            {
                if (queueIn.Count > 0 && queueOut.Count ==0)
                {
                    for (int i = 0; i < queueIn.Count; i++)
                    {
                        string data = queueIn.Dequeue();
                        queueOut.Enqueue(data);
                    }
                }

                if (wait_list.Count >= 4 && switch_for_timer == false)
                {
                    new Thread(new ThreadStart(generateRooms)).Start();
                }

                List<ICallBack> channels = new List<ICallBack>();

                foreach (ICallBack ic in ping.Keys)
                    channels.Add(ic);

                foreach (ICallBack ic in channels)
                    ping[ic]++;

                
                
            }
            catch (Exception ex) {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
                //var twilio = new TwilioRestClient("AC7b4d1e8619bf4a5afbf3e329488ffb6f", "9b51447e7609aa7ae54ec3cb22d2b1f7");
                //twilio.SendSmsMessage("+18065896495", "18764476773", "Alert there is no access to the database for Domino Park. Someone get on it now!");
                //twilio.SendSmsMessage("+18065896495", "18768713746", "Alert there is no access to the database for Domino Park. Someone get on it now!");
            }
        }

        [OperationContract]
        public void Subscribe(string roomName)
        {
            bool check = false;
            try
            {
                if (!client.ContainsKey(roomName))
                {
                    client.Add(roomName, new SynchronizedCollection<ICallBack>());
                    client[roomName].Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
                    ping.Add(OperationContext.Current.GetCallbackChannel<ICallBack>(), 0);
                }
                else
                {
                    for (int i = 0; i < client[roomName].Count; i++)
                    {
                        if (client[roomName][i] == OperationContext.Current.GetCallbackChannel<ICallBack>())
                        {
                            check = true;
                            client[roomName][i] = OperationContext.Current.GetCallbackChannel<ICallBack>();
                            ping.Add(OperationContext.Current.GetCallbackChannel<ICallBack>(), 0);
                            break;
                        }
                    }

                    if (!check)
                    {
                        client[roomName].Add(OperationContext.Current.GetCallbackChannel<ICallBack>());
                        ping.Add(OperationContext.Current.GetCallbackChannel<ICallBack>(), 0);
                    }             
                    
                }
                
            }
            catch(Exception ex) {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }

        [OperationContract]
        public void Publish(string room, string message)
        {
            try
            {
                queueIn.Enqueue(room + "," + message);
            }
            catch (Exception ex) {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }

        [OperationContract]
        public void JoinWait()
        {
            try
            {
                if(!wait_list.Contains(OperationContext.Current.GetCallbackChannel<ICallBack>()))
                    wait_list.Enqueue(OperationContext.Current.GetCallbackChannel<ICallBack>());
            }
            catch (Exception ex) {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }

        [OperationContract]
        public void checkIn()
        {
            try
            {
                if (ping.ContainsKey(OperationContext.Current.GetCallbackChannel<ICallBack>()))
                {
                    ping[OperationContext.Current.GetCallbackChannel<ICallBack>()] = 0;
                }
            }
            catch (Exception ex)
            {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }      
        
        [OperationContract]
        public void shuffle(string roomName)
        {
            try
            {
                List<int> cards = new List<int>();
                string message = "";
                while (cards.Count < 28)
                {
                    Random ran = new Random();
                    int num = ran.Next(0, 28);

                    if (!cards.Contains(num))
                    {
                        cards.Add(num);
                        message = "card:" + num + ",";
                    }
                }
            }
            catch (Exception ex)
            {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }

        void generateRooms()
        {
            try
            {
                switch_for_timer = true;

                int roomNum = 0;
                string room = "room" + roomNum;

                while (room == "room0")
                {
                    roomNum = new Random().Next(1000, 1000000);

                    if (!client.ContainsKey("room" + roomNum))
                        break;
                }
                room = "room" + roomNum;

                // generate room
                client.Add(room, new SynchronizedCollection<ICallBack>());

                //add four players to room
                int num = wait_list.Count;
                for (int i = 0; i < num; i++)
                {
                    client[room].Add(wait_list.Dequeue());
                    queueOut.Enqueue(room + ",room:" + room);

                    if (i == 3)
                        break;
                }

                //remove players from lobby
                for (int i = 0; i < client[room].Count; i++)
                {
                    if (client["lobby"].Contains(client[room][i]))
                        client["lobby"].Remove(client[room][i]);
                }

                switch_for_timer = false;
            }
            catch (Exception ex)
            {
                new SqlCommand("" + ex.Message, conn).ExecuteNonQuery();
            }
        }

        void remove_dead_Channel()
        {
            List<string> room= new List<string>();

            foreach (string r in client.Keys)
                room.Add(r);

            for (int i = 0; i < room.Count; i++)
            {
                for (int j = 0; j < client[room[i]].Count; j++)
                {
                    ICommunicationObject ch = client[room[i]][j] as ICommunicationObject;
                    if (ch.State != CommunicationState.Closed || ch.State != CommunicationState.Faulted || ch.State != CommunicationState.Closing)
                        client[room[i]].Remove(client[room[i]][j]);

                    if (dead.Contains(client[room[i]][j]))
                        client[room[i]].Remove(client[room[i]][j]);                    
                }
            }
        }

        void onMessageComplete(IAsyncResult iar)
        {
            if (iar.CompletedSynchronously)
            {
                return;
            }
            else
            {
                messageComplete(iar);
            }
        }

        void messageComplete(IAsyncResult iar)
        {
            ICallBack channel = ((messageState)(iar.AsyncState)).channel;

            try
            {
                channel.EndNotifyProcess(iar);
            }
            catch (Exception)
            {
                //client.Remove(channel);
            }
        }      
    }

    class messageState
    {
        internal ICallBack channel;
        internal messageState(ICallBack ch)
        {
            channel = ch;
        }
    }
}
