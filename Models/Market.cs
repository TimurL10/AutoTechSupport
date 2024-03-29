﻿using AutoTechSupport.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoTechSupport.Models
{
    public class Market : IMarket
    {
        DateTimeOffset timeStamp;
        DateTimeOffset stockDate;
        IMarketRepository _marketRepository;
        List<Market> Week1;
        List<string> NetsList;
        List<Day> DaysList = new List<Day>();

        public Market() { }
        public Market(Guid storeId, string storeName, string netName, string softwareName, DateTimeOffset stockDate, bool activeFl, bool reserveFl, bool stocksFl, DateTime timeStamp)
        {
            StoreId = storeId;
            StoreName = storeName;
            NetName = netName;
            SoftwareName = softwareName;
            StockDate = stockDate;
            ActiveFl = activeFl;
            ReserveFl = reserveFl;
            StocksFl = stocksFl;
            TimeStamp = timeStamp;
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
        public DateTimeOffset TimeStamp { get; set; }        
        public string Reason { get; set; }
        public string Status { get; set; }

        public void InsertNewMarkets()
        {
            var noStockMarkets = _marketRepository.GetNewMarkets(); // заменить на локальную переменную List вместо var
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
        }
        public void UpdateCurrentListOfMarkets()
        {
            var noStockMarkets = _marketRepository.GetNewMarkets().ToList(); ; // заменить на локальную переменную List вместо var
            var savedMarkets = _marketRepository.GetSavedMarkets().ToList(); 
            if (savedMarkets.Count == 0)
                InsertNewMarkets();
                  
            for (var i = 0; i < savedMarkets.Count; i ++) 
            {
                for (var j = 0; j < noStockMarkets.Count; j ++)
                {
                    //ищем магазины которые есть в обоих списках но ts новее и добавляем его с новой датой
                    if (savedMarkets[i].StoreId == noStockMarkets[j].StoreId && savedMarkets[i].TimeStamp.Day != noStockMarkets[j].TimeStamp.Day)
                    {
                        savedMarkets[i].TimeStamp = DateTime.Now;
                        _marketRepository.InsertMarkets(savedMarkets[i]);
                        break;
                    }
                    //ищем новые магазины в старом листе и если их нет добавляем
                    var newMarket = savedMarkets.Select(m => m.StoreId).Contains(noStockMarkets[j].StoreId);
                    if (!newMarket)
                        _marketRepository.InsertMarkets(noStockMarkets[j]);
                }
                //ищем старые магазины в новом листе и если его уже нет и прошло <= 3 часа меняем статус
               var markeExist = noStockMarkets.Select(a => a.StoreId).Contains(savedMarkets[i].StoreId);
                if (!markeExist && savedMarkets[i].TimeStamp.Hour <= 3)
                {
                    savedMarkets[i].Status = "on-line";
                    savedMarkets[i].Reason = "> 24h";
                    _marketRepository.UpdateMarkets(savedMarkets[i]);
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
        public void AggWeekListOfMarkets()
        {
            List<Market> listByNet = new List<Market>();
            var savedMarkets = _marketRepository.GetSavedMarkets();
            //NetsList = savedMarkets.Select(m => m.NetName).Distinct().ToList();
            var sortedMarkets = (from m in savedMarkets orderby m.TimeStamp, m.NetName select m).ToList();

            for (int i = 0; i < sortedMarkets.Count - 1; i ++)
            {
                listByNet.Add(sortedMarkets[i]);
                if (sortedMarkets[i].NetName != sortedMarkets[i + 1].NetName)
                {
                    var list = listByNet.GroupBy(x => x.Status).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).ToList();
                    foreach (var a in list)
                    {
                        if (a.Value == "in work")
                        {
                            Day day = new Day(sortedMarkets[i].NetName, a.Count, 0, sortedMarkets[i].TimeStamp.Day, a.Value);
                            DaysList.Add(day);
                        }
                        else if (a.Value == "on-line")
                        {
                            Day day = new Day(sortedMarkets[i].NetName, 0, a.Count, sortedMarkets[i].TimeStamp.Day, a.Value);
                            DaysList.Add(day);
                        }
                    }
                    listByNet.Clear();
                }
            }                        
        }
        public void BuildExcelReport(List<Market> week1)
        {            
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "АС";
                worksheet.Cells["B1"].Value = "МНН";


                DataTable dataTable = new DataTable();
                for (int i = 0; week1.Count > 0; i++)
                    for (int j = 0; j < 28; j++)
                    {

                    }
                   
                //add three colums to the datatable
                dataTable.Columns.Add("ID", typeof(int));
                dataTable.Columns.Add("Type", typeof(string));
                dataTable.Columns.Add("Name", typeof(string));

                //add some rows
                dataTable.Rows.Add(0, "Country", "Netherlands");
                dataTable.Rows.Add(1, "Country", "Japan");

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
            }
                    
                //var root = System.IO.Path.GetDirectoryName(pricePath);
                //string subdir = @$"{root}\Reports";
                //Directory.CreateDirectory(subdir);
                //System.IO.Path.Combine(root, "Reports");

                //FileInfo excelFile = new FileInfo($@"{root}\Reports\{fileName}.xlsx");
                //excel.SaveAs(excelFile);
                //long totalMemory = GC.GetTotalMemory(false);

                //GC.Collect(1, GCCollectionMode.Forced);
                //GC.WaitForPendingFinalizers();         
            
            
        }
        public List<Market> SortingByNet()
        {
            var listMarkets = _marketRepository.GetNewMarkets();
            var list = listMarkets.Where(s => s.NetName == "ХАБАРОВСК - Астери").ToList();


            return list;
        }
        public void SendMarketsToTelegramBot()
        {
            string dataToChat = "";
            var list = SortingByNet();
            for  (int i = 0; i < list.Count(); i ++)
            {
                dataToChat += list[i].StoreName + "; " + "Дата выгрузки осттаков: " + list[i].StockDate.ToString() + "\n";
            }

            var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>( "chat_id", "-541844670" ),
                    new KeyValuePair<string, string>( "text", dataToChat )
                };
            var content = new FormUrlEncodedContent(pairs);

            using (var client = new HttpClient())
            {
                var response =
                    client.PostAsync("https://api.telegram.org/bot1736378267:AAFevqGYytgSGAsPFjcMlXJDrCIBQ6XYZwA/sendMessage", content).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine(result);
                
            }
        }



    }
}
