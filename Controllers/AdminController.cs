
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Store.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;


namespace Store.Controllers;

public class Admin : BaseController
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}