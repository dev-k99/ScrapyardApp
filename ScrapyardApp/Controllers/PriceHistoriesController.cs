using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Data;
using ScrapyardApp.Models;
using System.Threading.Tasks;

namespace ScrapyardApp.Controllers
{
    [Authorize]
    public class PriceHistoriesController : Controller
    {
        private readonly ScrapyardDbContext _context;

        public PriceHistoriesController(ScrapyardDbContext context)
        {
            _context = context;
        }

        // GET: PriceHistories
        public async Task<IActionResult> Index()
        {
            var priceHistories = await _context.PriceHistories
                .Include(ph => ph.ScrapItem)
                .ToListAsync();
            return View(priceHistories);
        }

        // GET: PriceHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories
                .Include(ph => ph.ScrapItem)
                .FirstOrDefaultAsync(ph => ph.Id == id);
            if (priceHistory == null)
            {
                return NotFound();
            }

            return View(priceHistory);
        }

        // GET: PriceHistories/Create
        public IActionResult Create()
        {
            ViewBag.ScrapItems = _context.ScrapItems
                .Select(item => new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                })
                .ToList();

            return View();
        }

        // POST: PriceHistories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScrapItemId,PricePerKg,EffectiveDate")] PriceHistory priceHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View(priceHistory);
        }

        // GET: PriceHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories.FindAsync(id);
            if (priceHistory == null)
            {
                return NotFound();
            }
            ViewBag.ScrapItems = _context.ScrapItems
               .Select(item => new SelectListItem
               {
                   Value = item.Id.ToString(),
                   Text = item.Name
               })
               .ToList();
            return View(priceHistory);
        }

        // POST: PriceHistories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScrapItemId,PricePerKg,EffectiveDate")] PriceHistory priceHistory)
        {
            if (id != priceHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceHistoryExists(priceHistory.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View(priceHistory);
        }

        // GET: PriceHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceHistory = await _context.PriceHistories
                .Include(ph => ph.ScrapItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceHistory == null)
            {
                return NotFound();
            }

            return View(priceHistory);
        }

        // POST: PriceHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceHistory = await _context.PriceHistories.FindAsync(id);
            if (priceHistory != null)
            {
                _context.PriceHistories.Remove(priceHistory);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PriceHistoryExists(int id)
        {
            return _context.PriceHistories.Any(e => e.Id == id);
        }
    }
}