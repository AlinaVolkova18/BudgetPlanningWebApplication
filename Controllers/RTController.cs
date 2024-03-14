using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using NuGet.Protocol.Plugins;
using System.Text;

namespace BudgetPlanningWebApplication.Controllers
{
    public class RTController : Controller
    {
        private BudgetPlanningContext _context;

        public RTController()
        {
            _context = new BudgetPlanningContext();
        }
        public IActionResult Index()
        {
            if (ViewData["Auth"] == "Success")
            {

            }
            if (Request.Cookies.ContainsKey("uId"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult LogOut()
        {
            if (ViewData["Auth"] == "Success")
            {
                
                ViewData["Auth"] = "False";
            }
            Response.Cookies.Delete("uId");
            return View("Index");
        }
        public IActionResult Registr()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Auth(string login, string password)
        {
            if (_context.Users.Where(u => (u.Login == login || u.Email == login) && u.Password == password).Any())
            {
                Response.Cookies.Append("uId", _context.Users.Where(u => (u.Login == login || u.Email == login) && u.Password == password).First().Id.ToString());
                return RedirectToAction("Index", "Home", _context.Users.Where(u => (u.Login == login || u.Email == login) && u.Password == password).First());

            }
            else if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(login))
            {
                ViewData["Auth"] = "Error";
            }

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registr(string login, string password)
        {
            if (_context.Users.Where(u => u.Login == login || u.Email == login).Any())
            {
                ViewData["Registr"] = "Error";
            }
            else if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(login))
            {
                
                _context.Users.Add(new User { Login = login, Password = password });
                _context.SaveChanges();
                _context.Balances.AddRange(
                    new Balance { Sum = 0, ActiveId = 1,UserId = _context.Users.Where(u => u.Login == login || u.Email == login).First().Id },
                    new Balance{ Sum = 0, ActiveId = 2, UserId = _context.Users.Where(u => u.Login == login || u.Email == login).First().Id });
                _context.SaveChanges();
                ViewData["Registr"] = "Success";
            }
            return View();
            
        }

       
   
    }


}

