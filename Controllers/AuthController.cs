
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
        }

        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Register(string first_name, string last_name, string username, string password, DateTime birth, string gender, string address, string email, string phoneNumber)
    {
        //chưa test
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        if (user != null)
        {
            return View();
        }
        else
        {
            var hashedPassword = BC.HashPassword(password);
            var newUser = new User
            {
                RoleId = 2,
                Username = username,
                FirstName = first_name,
                LastName = last_name,
                BirthDate = birth,
                Gender = gender,
                Address = address,
                Email = email,
                PhoneNumber = phoneNumber,
                Password = hashedPassword
            };
            var create = dbContext.Users.Add(newUser);
            if (create != null)
            {
                return Redirect("/");
            }
            else
            {
                return View();
            }
        }
    }
    public IActionResult Logout(string redirect)
    {
        HttpContext.Session.Remove("user");
        return Redirect(redirect);
    }
}