using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CytubeBotWeb.Models;
using Microsoft.AspNetCore.Authorization;
using CytubeBotWeb.ViewModels;
using CytubeBotWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace CytubeBotWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ServerDbContext _context;

        public DashboardController(ServerDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var CommandLogs = await _context.CommandLogs.ToListAsync();
            var model = new DashboardIndexViewModel();
            model.CommandLogs = CommandLogs;
            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
