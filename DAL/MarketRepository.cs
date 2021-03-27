using AutoTechSupport.Models;
using AutoTechSupport.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.DAL
{
    public class MarketRepository : IMarketRepository
    {
        private string _configuration;
        private string _configurationAzure;
        public MarketRepository(IConfiguration configuration)
        {
            _configuration = configuration.GetValue<string>("DBInfo:ConnectionString");
            _configurationAzure = configuration.GetValue<string>("DataBaseAzureInfo:ConnectionString");
        }

        internal IDbConnection Connection
        {
            get { return new SqlConnection(_configuration); }
        }
        internal IDbConnection ConnectionAzure
        {
            get { return new SqlConnection(_configurationAzure); }
        }

        public void InsertMarkets(Market market)
        {
            using (IDbConnection connection = Connection)
            {
                connection.Execute("Insert into MarketsActivity (StoreId, StoreName, NetName, SoftwareName, StockDate, ActiveFl, reserveFl, StocksFl, TimeStamp, Reason, Status) Values (@StoreId, @StoreName, @NetName, @SoftwareName, @StockDate, @ActiveFl, @reserveFl, @StocksFl, @TimeStamp, @Reason, @Status) ", market);
            }
        }
        public List<Market> GetSavedMarkets()
        {
            using (IDbConnection connection = Connection)
            {
                return connection.Query<Market>("Select * from MarketsActivity Order by TimeStamp").ToList();
            }
        }
        public void UpdateMarkets(Market market)
        {
            using (IDbConnection connection = Connection)
            {
                connection.Execute("Update MarketsActivity set Status = @Status, Reason = @Reason Where StoreId = @StoreId", market);
            }
        }             

        public List<Market> GetNewMarkets()
        {
            ArrayList arrayList = new ArrayList();
            //List<Market> markets = new List<Market>();
            //using (IDbConnection connection = dbConnection)
            //{
            //    var sql = "exec [Monitoring].[GetOffStoresFromSite] @deep=1";
            //    int i = 1;
            //    markets = connection.Query<Market>(sql).ToList();

            //}

            List<Market> markets = new List<Market>();
            using (IDbConnection connection = ConnectionAzure)
            {
                string spName = "[Monitoring].[GetOffStoresFromSite]";

                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = (SqlConnection)connection;
                command.CommandText = spName;
                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@deep";
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Direction = ParameterDirection.Input;
                parameter.Value = 1;
                command.Parameters.AddWithValue("@deep", 1);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //var parser = reader.GetRowParser<Market>(typeof(Market));
                            //var myObject = parser(reader);
                            //markets.Add(myObject);
                            for (var i = 0; i < 8; i++)
                            {
                                if (reader[i] == null || reader[i] == DBNull.Value && i == 4)
                                    arrayList.Add(DateTimeOffset.MinValue);
                                else if (reader[i] == null || reader[i] == DBNull.Value)
                                    arrayList.Add("-");
                                else
                                    arrayList.Add(reader[i]);
                            }
                            Market market = new Market((Guid)arrayList[0], (string)arrayList[1], (string)arrayList[2],
                                (string)arrayList[3], (DateTimeOffset)arrayList[4], (bool)arrayList[5], (bool)arrayList[6], (bool)arrayList[7]);
                            markets.Add(market);
                            arrayList.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    reader.Close();
                }
            }
            return markets;
        }

    }
}
