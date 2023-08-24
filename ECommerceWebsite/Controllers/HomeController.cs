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
using NuGet.Protocol.Core.Types;
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
        private readonly IRepository _repository;
        public HomeController(ILogger<HomeController> logger, ITokenService tokenService, IUnitOfWork unitOfWork, DataContext db, IRepository repository)
        {
            _logger = logger;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _db = db;
            _repository = repository;
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
        public IActionResult DocumentUpload(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file != null && file.Length > 0)
            {
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var excelData = new List<ExcelDataModel>();

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        excelData.Add(new ExcelDataModel
                        {
                            Column1 = worksheet.Cells[row, 1].Value?.ToString(),
                            Column2 = worksheet.Cells[row, 2].Value?.ToString(),
                            Column3 = worksheet.Cells[row, 3].Value?.ToString(),
                            Column4 = worksheet.Cells[row, 4].Value?.ToString(),
                            Column5 = worksheet.Cells[row, 5].Value?.ToString(),

                        });
                    }
                    _repository.insertExcelFileSP(excelData);
                    return RedirectToAction("DocumentUpload", "Home");
                }
            }
            return RedirectToAction("DocumentUpload", "Home");
        }

    }
}