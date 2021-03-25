using AutoTechSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Services
{
    public interface IMarketRepository
    {
        void InsertMarkets(Market market);
        List<Market> GetSavedMarkets();
        void UpdateMarkets(Market market);
        List<Market> GetNewMarkets();
    }
}
