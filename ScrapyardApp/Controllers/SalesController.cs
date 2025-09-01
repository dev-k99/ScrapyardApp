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
    public class SalesController : Controller
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

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScrapItemId,CustomerId,WeightSold,TotalPrice,PaymentMethod,InvoiceNumber")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                if (_context.Sales.Any(s => s.InvoiceNumber == sale.InvoiceNumber))
                {
                    ModelState.AddModelError("InvoiceNumber", "Invoice number must be unique.");
                }
                else
                {
                    var scrapItem = await _context.ScrapItems.FindAsync(sale.ScrapItemId);
                    if (scrapItem != null && scrapItem.Quantity >= sale.WeightSold)
                    {
                        sale.TotalPrice = (decimal)sale.WeightSold * scrapItem.PricePerKg;
                        sale.SaleDate = DateTime.Now;
                        scrapItem.Quantity -= (int)sale.WeightSold;

                        _context.Add(sale);
                        _context.Update(scrapItem);
                        await _context.SaveChangesAsync();

                        // Log the action
                        var customer = await _context.Customers.FindAsync(sale.CustomerId);
                        var auditLog = new AuditLog
                        {
                            Action = "Create",
                            Entity = "Sale",
                            EntityId = sale.Id,
                            UserId = User.Identity.Name,
                            ActionDate = DateTime.Now,
                            Details = $"Created sale: Invoice {sale.InvoiceNumber}, Item: {scrapItem.Name}, Customer: {customer.Name}, Weight: {sale.WeightSold}kg"
                        };
                        _context.AuditLogs.Add(auditLog);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("", "Insufficient inventory or invalid item.");
                }
            }
            ViewBag.ScrapItems = _context.ScrapItems.ToList();
            ViewBag.Customers = _context.Customers.ToList();
            return View(sale);
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

                                // Log the action
                                var customer = await _context.Customers.FindAsync(sale.CustomerId);
                                var auditLog = new AuditLog
                                {
                                    Action = "Edit",
                                    Entity = "Sale",
                                    EntityId = sale.Id,
                                    UserId = User.Identity.Name,
                                    ActionDate = DateTime.Now,
                                    Details = $"Edited sale: Invoice {sale.InvoiceNumber}, Item: {scrapItem.Name}, Customer: {customer.Name}, Weight: {sale.WeightSold}kg"
                                };
                                _context.AuditLogs.Add(auditLog);
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

                // Log the action
                var auditLog = new AuditLog
                {
                    Action = "Delete",
                    Entity = "Sale",
                    EntityId = id,
                    UserId = User.Identity.Name,
                    ActionDate = DateTime.Now,
                    Details = $"Deleted sale: Invoice {sale.InvoiceNumber}, Item: {sale.ScrapItem.Name}, Customer: {sale.Customer.Name}"
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}