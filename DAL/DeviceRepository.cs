using AutoTechSupport.Models;
using AutoTechSupport.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.DAL
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly string connectionString;
        public DeviceRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }

        internal IDbConnection Connection 
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }

        public List<Device> GetDevices()
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                return dbConnection.Query<Device>("Select * From terminal order by hours_offline DESC").ToList();
            }
        }

        public Device Get(double id)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                return dbConnection.Query<Device>("Select * From terminal Where id = @id", new { Id = id }).FirstOrDefault();
            }
        }

        public void Update(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Update terminal Set id = @Id, name = @Name, status = @Status, ip = @Ip, last_online = @Last_Online, campaign_name = @Campaign_Name Where id = @Id", device);
                dbConnection.Close();
            }
        }

        public void UpdateSumHourse(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Update terminal Set SumHours = @SumHours Where id = @Id", device);
                dbConnection.Close();
            }
        }
        public void UpdateHoursOffline(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Update terminal Set hours_offline = @Hours_Offline  Where id = @Id", device);
                dbConnection.Close();
            }
        }


        public void Delete(double id)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM terminal WHERE Id = @id", new { Id = id });
                dbConnection.Close();
            }
        }

        public void Add(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Insert Into terminal(id, name, status, ip, last_online, campaign_name) Values (@Id, @Name, @Status, @Ip, @Last_Online, @Campaign_Name)", device);
                dbConnection.Close();
            }
        }

        public void UpdateAddress(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Update terminal Set name = @Name, address = @Address Where name = @Name", device);
                dbConnection.Close();
            }
        }

        public void AddAddress(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                //dbConnection.Execute("Insert Into address (name, address) Values (@Name, @Address)", device);
                dbConnection.Execute("Update terminal Set address = @Address Where name = @Name", device);
            }
        }

        public Device GetAddress(string name)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                return dbConnection.Query<Device>("Select * from terminal where name = @Name", new { Name = name }).FirstOrDefault();
            }
        }

        public void UpdateDevice(Device device)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute("Update terminal Set teamviewer = @TeamViewer, address = @Address, note = @Note Where name = @Name", device);
                dbConnection.Close();
            }
        }

        public void UpdateNotes(string name, string note)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute($"Update terminal Set note = {note} Where name = {name}");
                dbConnection.Close();
            }
        }

        public void PostActivity(double id, DateTime currentDate, int active)
        {
            using (var dbConnection = new SqlConnection(connectionString))
            {
                dbConnection.Open();
                dbConnection.Execute($"Insert Into terminal_activity(t_id, active_date, active) Values ({id},convert(varchar, DATEADD(HOUR, +3, GETUTCDATE())),{active})");
                dbConnection.Close();
            }
        }

    }
}
