using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ScrapyardApp.Controllers
{
    public class BaseController : Controller
    {
        protected string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        protected string GetCurrentUserEmail() => User.Identity?.Name ?? "Unknown";
    }
}