using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetPlanningWebApplication;

namespace BudgetPlanningWebApplication.Controllers
{
    public class MandatoryPaymentsController : Controller
    {
        private readonly BudgetPlanningContext _context;

        public MandatoryPaymentsController()
        {
            _context = new BudgetPlanningContext();
        }

        // GET: MandatoryPayments
        public async Task<IActionResult> Index()
        {
              return _context.MandatoryPayments != null ? 
                          View(await _context.MandatoryPayments.ToListAsync()) :
                          Problem("Entity set 'BudgetPlanningContext.MandatoryPayments'  is null.");
        }

        // GET: MandatoryPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MandatoryPayments == null)
            {
                return NotFound();
            }

            var mandatoryPayment = await _context.MandatoryPayments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mandatoryPayment == null)
            {
                return NotFound();
            }

            return View(mandatoryPayment);
        }

        // GET: MandatoryPayments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MandatoryPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Sum,Comment")] MandatoryPayment mandatoryPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mandatoryPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mandatoryPayment);
        }

        // GET: MandatoryPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MandatoryPayments == null)
            {
                return NotFound();
            }

            var mandatoryPayment = await _context.MandatoryPayments.FindAsync(id);
            if (mandatoryPayment == null)
            {
                return NotFound();
            }
            return View(mandatoryPayment);
        }

        // POST: MandatoryPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Sum,Comment")] MandatoryPayment mandatoryPayment)
        {
            if (id != mandatoryPayment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mandatoryPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MandatoryPaymentExists(mandatoryPayment.Id))
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
            return View(mandatoryPayment);
        }

        // GET: MandatoryPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MandatoryPayments == null)
            {
                return NotFound();
            }

            var mandatoryPayment = await _context.MandatoryPayments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mandatoryPayment == null)
            {
                return NotFound();
            }

            return View(mandatoryPayment);
        }

        // POST: MandatoryPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MandatoryPayments == null)
            {
                return Problem("Entity set 'BudgetPlanningContext.MandatoryPayments'  is null.");
            }
            var mandatoryPayment = await _context.MandatoryPayments.FindAsync(id);
            if (mandatoryPayment != null)
            {
                _context.MandatoryPayments.Remove(mandatoryPayment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MandatoryPaymentExists(int id)
        {
          return (_context.MandatoryPayments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
