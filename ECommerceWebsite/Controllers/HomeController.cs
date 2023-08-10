﻿using ECommerceWebsite.DTOs;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            LoginDTO model = new();
            ViewBag.Message = TempData["Message"];
            ViewBag.Type = TempData["Type"];
            TempData.Remove("Message");
            TempData.Remove("Type");
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}