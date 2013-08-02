using Microsoft.ServiceModel.PollingDuplex.Scalable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dsService
{
    public class Session
    {
        public DateTime LastUsed { get; set; }
        public PollingDuplexSession PollingDuplexSession { get; set; }
    }
}