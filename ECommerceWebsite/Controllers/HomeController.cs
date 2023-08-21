using ECommerceWebsite.Data;
using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using ECommerceWebsite.Repositories;
using ECommerceWebsite.Services;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Diagnostics;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerceWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _db;
        public HomeController(ILogger<HomeController> logger, ITokenService tokenService, IUnitOfWork unitOfWork, DataContext db)
        {
            _logger = logger;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            var mobilephone = string.Empty;
            var mobileNumber = _tokenService.GetMobilePhoneFromToken();
            var result = _unitOfWork.UserRepository.GetUserByPhoneNumberAsync(mobileNumber);
            var userName = result.Value.Username;
            return View();
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
        [HttpGet]
        public IActionResult DocumentUpload()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult DocumentUpload(IFormFile postedFile)
        //{
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //    if (postedFile != null && postedFile.Length>0)
        //    {
        //        using (var package = new ExcelPackage(postedFile.OpenReadStream()))
        //        {
        //            var worksheet = package.Workbook.Worksheets[0];
        //            var excelData = new List<ExcelDataModel>();

        //            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        //            {
        //                excelData.Add(new ExcelDataModel
        //                {
        //                    Column1 = worksheet.Cells[row, 1].Value.ToString(),
        //                    Column2 = worksheet.Cells[row, 2].Value.ToString(),
        //                });
        //            }

        //            return View("DisplayData", excelData);
        //        }
        //    }
        //    return View("Upload");
        //}
        [HttpPost]
        public IActionResult DocumentUpload(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream= System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();

            }
            var datatiinsert = this.getDataFromExcel(file.FileName);

            _db.ExcelUpload.AddRange(datatiinsert);
            _db.SaveChanges();

            return RedirectToAction("DocumentUpload","Home");
        }
        private List<ExcelDataModel>  getDataFromExcel(string fname)
        {
            List<ExcelDataModel> dataToAdd = new List<ExcelDataModel>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fname;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using(var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while(reader.Read())
                    {
                        var newdata = new ExcelDataModel()
                        {
                            Id=Guid.NewGuid().ToString(),
                            Column1=reader.GetValue(0)?.ToString(),
                            Column2=reader.GetValue(1)?.ToString(),
                            Column3 =reader.GetValue(2)?.ToString(),
                            Column4 =reader.GetValue(3)?.ToString(),
                        };
                        dataToAdd.Add(newdata);
                    }
                }
            }
            return dataToAdd;
        }

    }
}