using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetPlanningWebApplication;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;

namespace BudgetPlanningWebApplication.Controllers
{
    public class FinancialPlansController : Controller
    {
        private readonly BudgetPlanningContext _context;

        public FinancialPlansController()
        {
            _context = new BudgetPlanningContext();
        }

        // GET: FinancialPlans
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            var budgetPlanningContext = _context.FinancialPlans.Include(f => f.Category).Include(f => f.MandatoryPayment).Include(f => f.User).Where(u => u.UserId == Convert.ToInt32(Request.Cookies["uId"])); ;
            return View(await budgetPlanningContext.ToListAsync());
        }

        // GET: FinancialPlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FinancialPlans == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }

            var financialPlan = await _context.FinancialPlans
                .Include(f => f.Category)
                .Include(f => f.MandatoryPayment)
                .Include(f => f.User).Include(f=> f.User.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (financialPlan == null || financialPlan.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }
            if (financialPlan == null)
            {
                return NotFound();
            }

            return View(financialPlan);
        }

        // GET: FinancialPlans/Create
        public IActionResult Create()
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["MandatoryPaymentId"] = new SelectList(_context.MandatoryPayments, "Id", "Name");
            return View();
        }

        // POST: FinancialPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Required(ErrorMessage = "Это поле обязательно для заполнения!")] 
            string Name,
            int CategoryId,
            [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
            [RegularExpression(@"\d+(\.\d+)?",ErrorMessage ="Неверно")] 
            float? Limit = null,
            [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
            DateTime? StartDate = null,
            [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
            DateTime? EndDate = null, 
            string? MandatoryPaymentName = null, 
            float? MandatoryPaymentSum = null, 
            string? MandatoryPaymentComment =null)
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if (ModelState.IsValid)
            {
                if(MandatoryPaymentName != null && MandatoryPaymentSum != null)
                {
                    _context.MandatoryPayments.Add(new MandatoryPayment { Name = MandatoryPaymentName, Sum = (float)MandatoryPaymentSum, Comment = MandatoryPaymentComment });
                    await _context.SaveChangesAsync();
                    _context.Add(new FinancialPlan { Name = Name, CategoryId = CategoryId, Limit = (float)Limit, StartDate = (DateTime)StartDate, EndDate = (DateTime)EndDate, MandatoryPaymentId = _context.MandatoryPayments.Local.Last().Id, UserId = Convert.ToInt32(Request.Cookies["uId"]) });
                }
                else
                    _context.Add(new FinancialPlan { Name = Name, CategoryId = CategoryId, Limit = (float)Limit, StartDate = (DateTime)StartDate, EndDate = (DateTime)EndDate, UserId = Convert.ToInt32(Request.Cookies["uId"]) });


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", CategoryId);
            return View(new FinancialPlan { Name = Name, CategoryId = CategoryId, Limit = (float)Limit.GetValueOrDefault(), StartDate = (DateTime)StartDate.GetValueOrDefault(), EndDate = (DateTime)EndDate.GetValueOrDefault(), UserId = Convert.ToInt32(Request.Cookies["uId"]) });
        }

        // GET: FinancialPlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FinancialPlans == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }

            var financialPlan = await _context.FinancialPlans
                .Include(f => f.Category)
                .Include(f => f.MandatoryPayment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (financialPlan == null || financialPlan.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", financialPlan.CategoryId);
            return View(financialPlan);
        }

        // POST: FinancialPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,string Name,int CategoryId,int UserId,float Limit, DateTime StartDate, DateTime EndDate,int? MandatoryPaymentId=null,string? MandatoryPaymentName=null,float? MandatoryPaymentSum=null,string? MandatoryPaymentComment=null  )
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if(MandatoryPaymentId== null && string.IsNullOrEmpty(MandatoryPaymentName) && MandatoryPaymentSum==null)
            {
                ModelState.Remove("MandatoryPaymentName");
                ModelState.Remove("MandatoryPaymentSum");
            }
                  if (!ModelState.IsValid)
            {
                try
                {
                    var financialPlan = _context.FinancialPlans.Find(id);
                    if (UserId != Convert.ToInt32(Request.Cookies["uId"]))
                    {
                        return NotFound();
                    }
                    if (MandatoryPaymentId == null && MandatoryPaymentName != null && MandatoryPaymentSum != null)
                    {
                        var MandatoryPayment_id = _context.MandatoryPayments.Add(new MandatoryPayment { Name = MandatoryPaymentName, Sum = (float)MandatoryPaymentSum, Comment = MandatoryPaymentComment }).Entity.Id;
                        await _context.SaveChangesAsync();
                        financialPlan.MandatoryPaymentId = _context.MandatoryPayments.Local.Where(u => u.Name == MandatoryPaymentName && u.Sum == MandatoryPaymentSum).First().Id;
                    }
                    else if (financialPlan.MandatoryPaymentId != null && financialPlan.MandatoryPayment.Name != null && financialPlan.MandatoryPayment.Sum != null)
                    {
                        _context.MandatoryPayments.Update(financialPlan.MandatoryPayment);
                    }
                    financialPlan.Name = Name;
                    financialPlan.Limit = Limit;
                    financialPlan.CategoryId = CategoryId;
                    financialPlan.StartDate = StartDate;
                    financialPlan.EndDate = EndDate;
                    _context.Update(financialPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinancialPlanExists(id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View(new FinancialPlan { Id = id, CategoryId = CategoryId, Limit = Limit, EndDate = EndDate, StartDate = StartDate, Name = Name, MandatoryPaymentId=MandatoryPaymentId,UserId=UserId });
        }

        // GET: FinancialPlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            
            if (id == null || _context.FinancialPlans == null || !Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            var financialPlan = await _context.FinancialPlans
                .Include(f => f.Category)
                .Include(f => f.MandatoryPayment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (financialPlan == null || financialPlan.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }

            return View(financialPlan);
        }

        // POST: FinancialPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Request.Cookies.ContainsKey("uId"))
            {
                return Unauthorized("Для доступа необходима авторизация");
            }
            if (_context.FinancialPlans == null)
            {
                return Problem("Entity set 'BudgetPlanningContext.FinancialPlans'  is null.");
            }
            var financialPlan = await _context.FinancialPlans.FindAsync(id);
            if (financialPlan == null || financialPlan.UserId != Convert.ToInt32(Request.Cookies["uId"]))
            {
                return NotFound();
            }
            if (financialPlan != null)
            {
                _context.FinancialPlans.Remove(financialPlan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinancialPlanExists(int id)
        {
          return (_context.FinancialPlans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
