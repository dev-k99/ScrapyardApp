using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Data;
using ScrapyardApp.Models;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ScrapyardApp.Controllers
{
    [Authorize]
    public class ScrapItemsController : Controller
    {
        private readonly ScrapyardDbContext _context;

        public ScrapItemsController(ScrapyardDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.ScrapItems.Include(si => si.Category).ToListAsync());
        }

        // GET: ScrapItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrapItem = await _context.ScrapItems
                .Include(si => si.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scrapItem == null)
            {
                return NotFound();
            }

            return View(scrapItem);
        }

        // GET: ScrapItems/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: ScrapItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Weight,PricePerKg,Quantity,Description,CategoryId")] ScrapItem scrapItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scrapItem);
                await _context.SaveChangesAsync();

                // Log the action
                var auditLog = new AuditLog
                {
                    Action = "Create",
                    Entity = "ScrapItem",
                    EntityId = scrapItem.Id,
                    UserId = User.Identity.Name,
                    ActionDate = DateTime.Now,
                    Details = $"Created scrap item: {scrapItem.Name}, Weight: {scrapItem.Weight}kg, Price: {scrapItem.PricePerKg}/kg"
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(scrapItem);
        }

        // GET: ScrapItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrapItem = await _context.ScrapItems.FindAsync(id);
            if (scrapItem == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(scrapItem);
        }

        // POST: ScrapItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Weight,PricePerKg,Quantity,Description,CategoryId")] ScrapItem scrapItem)
        {
            if (id != scrapItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalScrapItem = await _context.ScrapItems.AsNoTracking().FirstOrDefaultAsync(si => si.Id == id);
                    _context.Update(scrapItem);
                    await _context.SaveChangesAsync();

                    // Log price change if PricePerKg has changed
                    if (originalScrapItem.PricePerKg != scrapItem.PricePerKg)
                    {
                        var priceHistory = new PriceHistory
                        {
                            ScrapItemId = scrapItem.Id,
                            PricePerKg = scrapItem.PricePerKg,
                            EffectiveDate = DateTime.Now
                        };
                        _context.PriceHistories.Add(priceHistory);
                        await _context.SaveChangesAsync();
                    }

                    // Log the action
                    var auditLog = new AuditLog
                    {
                        Action = "Edit",
                        Entity = "ScrapItem",
                        EntityId = scrapItem.Id,
                        UserId = User.Identity.Name,
                        ActionDate = DateTime.Now,
                        Details = $"Edited scrap item: {scrapItem.Name}, Weight: {scrapItem.Weight}kg, Price: {scrapItem.PricePerKg}/kg"
                    };
                    _context.AuditLogs.Add(auditLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ScrapItems.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(scrapItem);
        }

        // GET: ScrapItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scrapItem = await _context.ScrapItems
                .Include(si => si.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scrapItem == null)
            {
                return NotFound();
            }

            return View(scrapItem);
        }

        // POST: ScrapItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scrapItem = await _context.ScrapItems.FindAsync(id);
            if (scrapItem != null)
            {
                _context.ScrapItems.Remove(scrapItem);
                await _context.SaveChangesAsync();

                // Log the action
                var auditLog = new AuditLog
                {
                    Action = "Delete",
                    Entity = "ScrapItem",
                    EntityId = id,
                    UserId = User.Identity.Name,
                    ActionDate = DateTime.Now,
                    Details = $"Deleted scrap item: {scrapItem.Name}"
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}