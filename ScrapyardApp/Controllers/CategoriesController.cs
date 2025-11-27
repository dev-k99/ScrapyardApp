// Controllers/CategoriesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Data;
using ScrapyardApp.Models;

namespace ScrapyardApp.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ScrapyardDbContext _context;

        public CategoriesController(ScrapyardDbContext context)
        {
            _context = context;
        }

        // GET: List
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET Create form
        public IActionResult Create()
        {
            return View();
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Category '{category.Name}' added!";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // Edit, Delete, Details — you can keep or delete them. Minimal version below:
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Delete (optional — safe to keep)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat != null)
            {
                _context.Categories.Remove(cat);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}