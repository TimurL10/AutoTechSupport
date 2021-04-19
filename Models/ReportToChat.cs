using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Models
{
    public class ReportToChat : Market
    {
        public string StoreName { get; set; }
        public DateTimeOffset StockDate { get; set; }

    }
}
