using AutoTechSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Services
{
    public interface IDeviceRepository
    {
        void Delete(double id);
        Device Get(double id);
        List<Device> GetDevices();
        void Update(Device device);
        void Add(Device device);
        void AddAddress(Device device);
        void UpdateAddress(Device device);
        Device GetAddress(string id);
        void UpdateDevice(Device device);
        void UpdateSumHourse(Device device);
        void UpdateHoursOffline(Device device);
        void UpdateNotes(string name, string note);
        void PostActivity(double id, DateTime currentDate, int active);
    }
}
