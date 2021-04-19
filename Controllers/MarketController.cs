using AutoTechSupport.Models;
using AutoTechSupport.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Controllers
{
    public class MarketController : Controller
    {
        private IMarket _market;

        public MarketController(IMarket market)
        {
            _market = market;
        }

        // GET: MarketController
        public ActionResult Index()
        {
            //_market.UpdateCurrentListOfMarkets();
            //var markets = _market.GetSavedMarkets();
            //_market.AggWeekListOfMarkets();
            _market.SendMarketsToTelegramBot();
            return View();
        }         
            
        public void GetReport()
        {
            _market.UpdateCurrentListOfMarkets();
        }
    }
}
