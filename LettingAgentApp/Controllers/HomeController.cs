﻿using LettingAgentApp.Core.Contracts;
using LettingAgentApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LettingAgentApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly IHouseService houseService;

        public HomeController(IHouseService _houseService)
        {
            houseService = _houseService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await houseService.LastThreeHouses();
            return View(model);
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
    }
}