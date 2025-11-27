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
    public class SalesController : BaseController
    {
        private readonly ScrapyardDbContext _context;

        public SalesController(ScrapyardDbContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sales
                .Include(s => s.ScrapItem)
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.ScrapItem)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }
        

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            ViewBag.Customers = _context.Customers.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            if (ModelState.IsValid)
            {
                var scrapItem = await _context.ScrapItems.FindAsync(sale.ScrapItemId);
                var customer = await _context.Customers.FindAsync(sale.CustomerId);

                if (scrapItem == null || customer == null || scrapItem.Quantity < sale.WeightSold)
                {
                    ModelState.AddModelError("", "Invalid item, customer, or insufficient stock.");
                }
                else
                {
                    sale.InvoiceNumber = GenerateInvoiceNumber();
                    sale.TotalPrice = (decimal)sale.WeightSold * scrapItem.PricePerKg;
                    sale.SaleDate = DateTime.Now;

                    scrapItem.Quantity -= sale.WeightSold;

                    _context.Add(sale);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Sale {sale.InvoiceNumber} created!";
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.ScrapItems = await _context.ScrapItems.Select(s => new { s.Id, s.Name }).ToListAsync();
            ViewBag.Customers = await _context.Customers.Select(c => new { c.Id, c.Name }).ToListAsync();
            return View(sale);
        }

        private string GenerateInvoiceNumber()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var count = _context.Sales.Count(s => s.InvoiceNumber.StartsWith($"INV-{today}")) + 1;
            return $"INV-{today}-{count:D4}";
        }
        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            ViewBag.Customers = _context.Customers.ToList();
            return View(sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScrapItemId,CustomerId,WeightSold,TotalPrice,SaleDate,PaymentMethod,InvoiceNumber")] Sale sale)
        {
            if (id != sale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalSale = await _context.Sales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (_context.Sales.Any(s => s.InvoiceNumber == sale.InvoiceNumber && s.Id != id))
                    {
                        ModelState.AddModelError("InvoiceNumber", "Invoice number must be unique.");
                    }
                    else
                    {
                        var scrapItem = await _context.ScrapItems.FindAsync(sale.ScrapItemId);
                        if (scrapItem != null)
                        {
                            double weightDifference = sale.WeightSold - originalSale.WeightSold;
                            if (scrapItem.Quantity >= weightDifference)
                            {
                                sale.TotalPrice = (decimal)sale.WeightSold * scrapItem.PricePerKg;
                                scrapItem.Quantity -= (int)weightDifference;

                                _context.Update(sale);
                                _context.Update(scrapItem);
                                await _context.SaveChangesAsync();

                               
                                await _context.SaveChangesAsync();

                                return RedirectToAction(nameof(Index));
                            }
                            ModelState.AddModelError("", "Insufficient inventory for updated weight.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid scrap item.");
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Sales.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            ViewBag.Customers = _context.Customers.ToList();
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.ScrapItem)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.Include(s => s.ScrapItem).Include(s => s.Customer).FirstOrDefaultAsync(s => s.Id == id);
            if (sale != null)
            {
                var scrapItem = await _context.ScrapItems.FindAsync(sale.ScrapItemId);
                if (scrapItem != null)
                {
                    scrapItem.Quantity += (int)sale.WeightSold;
                    _context.Update(scrapItem);
                }
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();

                
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}