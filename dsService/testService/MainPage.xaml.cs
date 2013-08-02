using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace testService
{
    public partial class MainPage : UserControl
    {
        ServiceReference1.Service1Client client;
        public MainPage()
        {
            InitializeComponent();

            client = new ServiceReference1.Service1Client(new PollingDuplexHttpBinding { DuplexMode =System.ServiceModel.Channels.PollingDuplexMode.MultipleMessagesPerPoll },
                new EndpointAddress(new Uri("http://localhost:51424/Service1.svc")));
            
            client.CallBackReceived += client_CallBackReceived;
            client.SubscribeAsync("hello");            
        }

        void client_CallBackReceived(object sender, ServiceReference1.CallBackReceivedEventArgs e)
        {
            txtbox.Text += e.message;
        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                client.PublishAsync("hello", mstext.Text);
                txtbox.Text += mstext.Text;
                mstext.Text = "";
            }
        }

       
    }
}
