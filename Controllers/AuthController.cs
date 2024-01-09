using Microsoft.AspNetCore.Mvc;

namespace Store.Controllers;
public class AuthController : BaseController
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string returnUrl)
    {
        Console.WriteLine(username);
        return View();
    }
}