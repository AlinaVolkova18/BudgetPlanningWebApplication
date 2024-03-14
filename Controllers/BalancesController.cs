using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetPlanningWebApplication;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BudgetPlanningWebApplication.Controllers
{
    public class BalancesController : Controller
    {
        private readonly BudgetPlanningContext _context;

        public BalancesController()
        {
            _context = new BudgetPlanningContext();
        }

        // GET: Balances
        public async Task<IActionResult> Index()
        {
            var budgetPlanningContext = _context.Balances.Include(b => b.Active).Include(b => b.User);
            return View(await budgetPlanningContext.ToListAsync());
        }
        public IActionResult Welcome(int ActiveId, double Sum)
        {
            ViewData["Balances"] = ActiveId + Sum;

            return View();
        }
        public async Task<IActionResult> SelectActive()
        {
            return View();
        }
        // GET: Balances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Balances == null)
            {
                return NotFound();
            }

            var balance = await _context.Balances
                .Include(b => b.Active)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }

            return View(balance);
        }

        // GET: Balances/Create
        public IActionResult Create()
        {
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Balances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sum,ActiveId,UserId")] Balance balance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(balance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Id", balance.ActiveId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", balance.UserId);
            return View(balance);
        }

        // GET: Balances/Edit/5
        public async Task<IActionResult> Edit(int activeId)
        {
            if (!Request.Cookies.ContainsKey("uId") || _context.Balances == null)
            {
                return NotFound();
            }

            var balance = await _context.Balances.Include(u=> u.Active).FirstOrDefaultAsync(u=> u.UserId == Convert.ToInt32(Request.Cookies["uId"]) && u.ActiveId == activeId);
            if (balance == null)
            {
                return NotFound();
            }
            return View(balance);
        }

        // POST: Balances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Sum,ActiveId,UserId")] Balance balance)
        {
            if (id != balance.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
                    _context.Update(balance);
                    await _context.SaveChangesAsync();
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    if (!BalanceExists(balance.Id))
                //    {
                //        return NotFound();
                //    }
                //    else
                //    {
                //        throw;
                //    }
                //}
               // return RedirectToAction("Details", "Users", new { id = balance.UserId });
            //}
            ViewData["ActiveId"] = new SelectList(_context.Actives, "Id", "Id", balance.ActiveId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", balance.UserId);
            return View(balance);
        }

        // GET: Balances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Balances == null)
            {
                return NotFound();
            }

            var balance = await _context.Balances
                .Include(b => b.Active)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (balance == null)
            {
                return NotFound();
            }

            return View(balance);
        }

        // POST: Balances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Balances == null)
            {
                return Problem("Entity set 'BudgetPlanningContext.Balances'  is null.");
            }
            var balance = await _context.Balances.FindAsync(id);
            if (balance != null)
            {
                _context.Balances.Remove(balance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BalanceExists(int id)
        {
          return (_context.Balances?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
