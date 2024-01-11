
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Store.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;


namespace Store.Controllers;
public class AuthController : BaseController
{
    [HttpGet]
    public IActionResult Login()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson != null) return Redirect("/");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string redirect)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);

        if (user != null)
        {
            var checkPassword = BC.Verify(password, user.Password);
            if (checkPassword)
            {
                var jsonStr = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString("user", jsonStr);
                return Redirect(redirect);
            }
            else ViewData["error"] = "Mật khẩu không chính xác";
        }
        else ViewData["error"] = $"Không tìm thấy tài khoản {username}";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson != null) return Redirect("/");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        ModelState.Remove("Role");
        ViewData["user"] = user;
        if (ModelState.IsValid)
        {
            var temp = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (temp != null)
            {
                ViewData["error"] = $"Tài khoản {user.Username} đã tồn tại";
                return View();
            }
            if (user.Email != null) temp = await dbContext.Users.FirstAsync(u => u.Email == user.Email);
            if (temp != null)
            {
                ViewData["error"] = "Email đã tồn tại";
                return View();
            }
            if (user.PhoneNumber != null) temp = await dbContext.Users.FirstAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (temp != null)
            {
                ViewData["error"] = "Số điện thoại đã tồn tại";
                return View();
            }

            // Save
            user.RoleId = 2;
            user.Password = BC.HashPassword(user.Password);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Login", "Auth");
        }
        ViewData["error"] = "Dữ liệu không hợp lệ";
        return View();
    }
    public IActionResult Logout(string redirect)
    {
        HttpContext.Session.Remove("user");
        return Redirect(redirect);
    }
}