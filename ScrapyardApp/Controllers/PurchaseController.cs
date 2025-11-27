using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Data;
using ScrapyardApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapyardApp.Controllers
{
    [Authorize]
    public class PurchasesController : BaseController
    {
        private readonly ScrapyardDbContext _context;

        public PurchasesController(ScrapyardDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            return View(await _context.Purchases
                .Include(p => p.ScrapItem)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync());
        }
    

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.ScrapItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScrapItemId,WeightPurchased,PurchasePrice,PurchaseDate,Supplier")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                var scrapItem = await _context.ScrapItems.FindAsync(purchase.ScrapItemId);
                if (scrapItem != null)
                {
                    purchase.PurchaseDate = DateTime.Now;
                    scrapItem.Quantity += (int)purchase.WeightPurchased;

                    _context.Add(purchase);
                    _context.Update(scrapItem);
                    await _context.SaveChangesAsync();

                    
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Invalid scrap item.");
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View(purchase);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScrapItemId,WeightPurchased,PurchasePrice,PurchaseDate,Supplier")] Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalPurchase = await _context.Purchases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    var scrapItem = await _context.ScrapItems.FindAsync(purchase.ScrapItemId);

                    if (scrapItem != null)
                    {
                        double weightDifference = purchase.WeightPurchased - originalPurchase.WeightPurchased;
                        scrapItem.Quantity += (int)weightDifference;

                        _context.Update(purchase);
                        _context.Update(scrapItem);
                        await _context.SaveChangesAsync();

                     
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "Invalid scrap item.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Purchases.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.ScrapItem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.Include(p => p.ScrapItem).FirstOrDefaultAsync(p => p.Id == id);
            if (purchase != null)
            {
                var scrapItem = await _context.ScrapItems.FindAsync(purchase.ScrapItemId);
                if (scrapItem != null)
                {
                    scrapItem.Quantity -= (int)purchase.WeightPurchased;
                    _context.Update(scrapItem);
                }
                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();

          
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}