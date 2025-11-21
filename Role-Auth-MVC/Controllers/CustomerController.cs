using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Role_Auth_MVC.Controllers;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}