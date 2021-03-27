using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Models
{
    public class Day
    {
        public Day(string net, int marketsCount, int dayOfWeek, string reasonName, string currentStatus)
        {
            Net = net;
            MarketsCount = marketsCount;
            DayOfWeek = dayOfWeek;
            ReasonName = reasonName;
            CurrentStatus = currentStatus;
        }

        public string Net { get; set; }
        public int MarketsCount { get; set; }
        public int DayOfWeek { get; set; }
        public string ReasonName { get; set; }
        public string CurrentStatus { get; set; }
        
    }
}
