using AutoTechSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Services
{
    public interface IMarket
    {
        void InsertNewMarkets();
        void UpdateCurrentListOfMarkets();
        List<Market> GetSavedMarkets();
        List<Market> GetNewMarkets();
    }
}
