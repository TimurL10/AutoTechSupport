using AutoTechSupport.Models;
using AutoTechSupport.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoTechSupport.Controllers
{
    public class DeviceController : Controller
    {
        private ILoggerFactory _loggerFactory;
        private readonly IDeviceRepository _deviceRepository;
        private IDevice _device;
        public DeviceController(ILoggerFactory loggerFactory, IConfiguration configuration, IDeviceRepository deviceRepository, IDevice device)
        {
            _loggerFactory = loggerFactory;
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            _deviceRepository = deviceRepository;
            _device = device;
        }

        // GET: DeviceController
        public ActionResult Index()
        {
            Task task1 = new Task(() => _device.GetXmlData());
            task1.Start();
            task1.Wait();

            Task task2 = new Task(() => _device.FilterDevices());
            task2.Start();
            task2.Wait();

            Task task3 = new Task(() => _device.getHoursOffline());
            task3.Start();
            task3.Wait();

            var terminals = _deviceRepository.GetDevices();
            terminals = _device.ConverDateToMoscowTime(terminals);
            return View(terminals);
        }

        public IActionResult IndexSort(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var compain = _deviceRepository.GetDevices();
            switch (sortOrder)
            {
                case "name_desc":
                    compain = compain.OrderByDescending(s => s.Campaign_Name).ToList();
                    break;
            }
            return View("Index", compain);
        }

        public async Task<IActionResult> Edit(double id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = _deviceRepository.Get(id);

            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(Device device)
        {
            if (device == null)
            {
                return NotFound();
            }

            _deviceRepository.UpdateDevice(device);

            return View(device);
        }

        [DisableRequestSizeLimit]
        [HttpPost("Home")]
        public async Task<ViewResult> Index(IFormFile file)
        {
            long size = file.Length;

            if (size > 0)
            {
                // full path to file in temp location
                //var filePath = Path.Combine(@"d:\Domains\smartsoft83.com\wwwroot\terminal\Files\", file.FileName); //we are using Temp file name just for the example. Add your own file path.
                var filePath = Path.Combine(@"C:\Users\Timur\source\repos\GetXml\Files\", file.FileName); //we are using Temp file name just for the example. Add your own file path.

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            Task taskReadFromFile = new Task(() => _device.ReadAddressesFromExcel());
            taskReadFromFile.Start();
            taskReadFromFile.Wait();
            Task postAddressToDb = new Task(() => _device.PostAddressToDb());
            postAddressToDb.Start();
            postAddressToDb.Wait();


            return View("Privacy");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public FileResult Export()
        {
            Task createRep = new Task(() => _device.CreateExcelReport());
            createRep.Start();
            createRep.Wait();
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"d:\Domains\smartsoft83.com\wwwroot\terminal\report.xlsx");
            string fileName = "terminals_report.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
