using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrapyardApp.Data;
using ScrapyardApp.Models;
using System.Threading.Tasks;

namespace ScrapyardApp.Controllers
{
    [Authorize]
    public class AuditLogsController : Controller
    {
        private readonly ScrapyardDbContext _context;

        public AuditLogsController(ScrapyardDbContext context)
        {
            _context = context;
        }

        // GET: AuditLogs
        public async Task<IActionResult> Index()
        {
            var auditLogs = await _context.AuditLogs
                .Include(al => al.User)
                .ToListAsync();
            return View(auditLogs);
        }

        // GET: AuditLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditLog = await _context.AuditLogs
                .Include(al => al.User)
                .FirstOrDefaultAsync(al => al.Id == id);
            if (auditLog == null)
            {
                return NotFound();
            }

            return View(auditLog);
        }
    }
}