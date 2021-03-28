using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Models
{
    public class Day : Market
    { 
        public Day(string netName, int inWorkStatusCount, int onlineStatusCount, int dayOfWeek, string reason)
        {
            NetName = netName;
            InWorkStatusCount = inWorkStatusCount;
            OnlineStatusCount = onlineStatusCount;
            DayOfWeek = dayOfWeek;
            Reason = reason;
            
        }

        public int InWorkStatusCount { get; set; }
        public int OnlineStatusCount { get; set; }
        public int DayOfWeek { get; set; }
    }
}
