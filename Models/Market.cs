using AutoTechSupport.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Models
{
    public class Market : IMarket
    {
        DateTime timeStamp;
        DateTimeOffset stockDate;
        IMarketRepository _marketRepository;

        public Market() { }
        public Market(Guid storeId, string storeName, string netName, string softwareName, DateTimeOffset stockDate, bool activeFl, bool reserveFl, bool stocksFl)
        {
            StoreId = storeId;
            StoreName = storeName;
            NetName = netName;
            SoftwareName = softwareName;
            StockDate = stockDate;
            ActiveFl = activeFl;
            ReserveFl = reserveFl;
            StocksFl = stocksFl;
        }
        public Market(IMarketRepository marketRepository)
        {
            _marketRepository = marketRepository;
        }

        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string NetName { get; set; }
        public string SoftwareName { get; set; }
        public DateTimeOffset StockDate { get; set; }
        public bool ActiveFl { get; set; }
        public bool ReserveFl { get; set; }
        public bool StocksFl { get; set; }
        public DateTime TimeStamp
        {

            get
            {
                return timeStamp = DateTime.Now;
            }
            set => timeStamp = value;

        }
        public string Reason { get; set; }
        public string Status { get; set; }

        public void InsertNewMarkets()
        {
            var noStockMarkets = _marketRepository.GetNewMarkets();
            var savedMarkets = _marketRepository.GetSavedMarkets();
            if (savedMarkets.Count == 0)
            {
                var Markets = _marketRepository.GetNewMarkets();
                foreach (var m in Markets)
                {
                    if (m.ReserveFl == false && m.ActiveFl == false && m.StocksFl == true)
                        m.Status = "in work";
                    m.Reason = "tech prbl";
                    _marketRepository.InsertMarkets(m);
                }
            }

            var newMarkets = noStockMarkets.Concat(savedMarkets).GroupBy(n => n.StoreId).
            Where(n => n.Count() == 1).Select(n => n.FirstOrDefault()).ToList();
            if (newMarkets.Count > 0)
            {
                foreach (var m in newMarkets)
                {
                    if (m.ReserveFl == false && m.ActiveFl == false && m.StocksFl == true)
                        m.Status = "in work";
                    m.Reason = "tech prbl";
                    _marketRepository.InsertMarkets(m);
                }
            }
        }
        public void UpdateCurrentListOfMarkets()
        {
            var noStockMarkets = _marketRepository.GetNewMarkets();
            var savedMarkets = _marketRepository.GetSavedMarkets();
            if (savedMarkets.Count == 0)
                InsertNewMarkets();

            for (var i = 0; i < savedMarkets.Count; i++)
            {
                for (var j = 0; j < noStockMarkets.Count; j++)
                {
                    // ищем магазины которые есть в обоих списках но ts новее и добавляем его с новой датой
                    if (savedMarkets[i].StoreId == noStockMarkets[j].StoreId && savedMarkets[i].TimeStamp.Day != noStockMarkets[j].TimeStamp.Day)
                    {
                        savedMarkets[i].TimeStamp = DateTime.Now;
                        _marketRepository.InsertMarkets(savedMarkets[i]);
                        break;
                    }
                    // ищем магазины которые есть в обоих списках с одинаковой датой и пропускаем его
                    else if (savedMarkets[i].StoreId == noStockMarkets[j].StoreId && savedMarkets[i].TimeStamp.Day == noStockMarkets[j].TimeStamp.Day)
                    {
                        break;
                    }
                    // ищем новые магазины в старом листе и ксли их нет добавляем
                    var newMarket = savedMarkets.Select(m => m.StoreId).Contains(noStockMarkets[j].StoreId);
                    if (!newMarket)
                        _marketRepository.InsertMarkets(savedMarkets[i]);

                }
                // ищем старые магазины в новом листе и если его уже нет и прошло <= 3 часа меняем статус
                var markeExist = noStockMarkets.Select(a => a.StoreId).Contains(savedMarkets[i].StoreId);
                if (!markeExist && savedMarkets[i].TimeStamp.Hour <= 3)
                {
                    savedMarkets[i].Status = "on-line";
                    savedMarkets[i].Reason = "> 24h";
                }
            }
        }
        public List<Market> GetNewMarkets()
        {
            return _marketRepository.GetNewMarkets();
        }

        public List<Market> GetSavedMarkets()
        {
            return _marketRepository.GetSavedMarkets();
        }
    }
}
