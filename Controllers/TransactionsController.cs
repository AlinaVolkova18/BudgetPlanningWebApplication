using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetPlanningWebApplication;
using System.Net;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetPlanningWebApplication.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly BudgetPlanningContext _context;
        User User;
        public TransactionsController()
        {
            _context = new BudgetPlanningContext();
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            if(!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            var budgetPlanningContext = _context.Transactions
                .Include(t => t.Active)
                .Include(t => t.Category)
                .Include(t => t.TypeOfTransaction)
                .Include(t => t.User)
                .Where(u=> u.UserId== Convert.ToInt32(Request.Cookies["uId"]));
            string balances1="";
            string balances2="";
            foreach (var item in _context.Balances.Include(u=> u.Active).Where(u=>u.UserId== Convert.ToInt32(Request.Cookies["uId"])))
            {
                if (item.Active.Name == "Наличные")
                    balances1 += $"{item.Sum} ";
                else
                    balances2 += $"{item.Sum} ";

                //if (item.Active.Name == "Наличные")
                //    balances1 += $"{item.Active.Name} - {item.Sum} ";
                //else
                //    balances2 += $"{item.Active.Name} - {item.Sum} ";
            }
            ViewData["Balances1"] = balances1;
            ViewData["Balances2"] = balances2;

            return View(await budgetPlanningContext.ToListAsync());
        }
        public async Task<IActionResult> SelectTypeOfTransaction()
        {
            return View();
        }
        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }

            var transaction = await _context.Transactions
                .Include(t => t.Active)
                .Include(t => t.Category)
                .Include(t => t.TypeOfTransaction)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null || transaction.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create(int typeOfTransactionId)
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(u=> u.TypeOfTransactionId==typeOfTransactionId), "Id", "Name");
            ViewData["TypeOfTransactionId"] = typeOfTransactionId;
            return View();
        }
        
        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TypeOfTransactionId, int CategoryId, int ActiveId, /*[RegularExpression(@"^\d{3}-\d{3}-\d{4}$",ErrorMessage = "Не указана сумма")] */float Sum, DateTime Date, string? Comment)
        {
            
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if (ModelState.IsValid)
            {
                var transactions = _context.Transactions.Where(u => u.UserId == Convert.ToInt32(Request.Cookies["uId"]) && u.CategoryId == CategoryId);
                float totalSum = 0;
                foreach(var item in transactions)
                {
                    totalSum += item.Sum;
                }
                totalSum+= Sum;
                if (_context.FinancialPlans.Where(u=> u.UserId== Convert.ToInt32(Request.Cookies["uId"]) && u.CategoryId== CategoryId && u.Limit<totalSum && Date<u.EndDate && Date>u.StartDate).Any())
                {
                    ViewData["Error"] = "Limit";
                }
                var balance = _context.Balances.Where(u => u.UserId == Convert.ToInt32(Request.Cookies["uId"]) && u.ActiveId == ActiveId).First();
                if(TypeOfTransactionId==1)
                {
                    balance.Sum += Sum;
                }else
                {
                    balance.Sum -= Sum;
                }
                _context.Balances.Update(balance);
                _context.Add(new Transaction { ActiveId = ActiveId, CategoryId = CategoryId, TypeOfTransactionId = TypeOfTransactionId, Sum = Sum, Date = Date, Comment= Comment, UserId =Convert.ToInt32(Request.Cookies["uId"]) });
                await _context.SaveChangesAsync();
                if(ViewData.ContainsKey("Error"))
                    return View();
                else
                return RedirectToAction(nameof(Index),ViewData);
            }
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TypeOfTransactionId"] = new SelectList(_context.TypeOfTransactions, "Id", "Name");
            return View(new Transaction { ActiveId = ActiveId, CategoryId = CategoryId, TypeOfTransactionId = TypeOfTransactionId, Sum = Sum, Date = Date, Comment = Comment, UserId = Convert.ToInt32(Request.Cookies["uId"]) });
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId!= Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Name", transaction.ActiveId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
            ViewData["TypeOfTransactionId"] = new SelectList(_context.TypeOfTransactions, "Id", "Name", transaction.TypeOfTransactionId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int TypeOfTransactionId, int CategoryId, int ActiveId, float Sum, DateTime Date, string? Comment)
        {

            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var transaction = await _context.Transactions.FindAsync(id);
                    if (transaction == null || transaction.UserId != Convert.ToInt32(Request.Cookies["uId"]))
                    {
                        return NotFound();
                    }
                    transaction.Sum = Sum;
                    transaction.Date = Date;
                    transaction.Comment = Comment;
                    transaction.ActiveId = ActiveId;
                    transaction.CategoryId = CategoryId;
                    transaction.TypeOfTransactionId = TypeOfTransactionId;
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Name", ActiveId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", CategoryId);
            ViewData["TypeOfTransactionId"] = new SelectList(_context.TypeOfTransactions, "Id", "Name", TypeOfTransactionId);
            return View(new Transaction { Id = id, ActiveId = ActiveId, CategoryId = CategoryId, TypeOfTransactionId = TypeOfTransactionId, Sum = Sum, Date = Date, Comment = Comment, UserId = Convert.ToInt32(Request.Cookies["uId"]) });
            
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            var transaction = await _context.Transactions
                .Include(t => t.Active)
                .Include(t => t.Category)
                .Include(t => t.TypeOfTransaction)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null || transaction.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'BudgetPlanningContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null || transaction.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
