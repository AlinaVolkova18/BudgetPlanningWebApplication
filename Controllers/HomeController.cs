using BudgetPlanningWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Diagnostics;

namespace BudgetPlanningWebApplication.Controllers
{
    public class HomeController : Controller
    {
        



        public HomeController()
        {
            
        }

        public IActionResult Index()
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            return View();
        }
        public IActionResult GoToTransaction(int id)
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                Unauthorized("Для доступа необходима авторизация");
            }
            return RedirectToAction("Index", "Transactions", Request.Cookies["uId"]);
        }

    }
}