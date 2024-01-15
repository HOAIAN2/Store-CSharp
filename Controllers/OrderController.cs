
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Store.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.VisualBasic;

namespace Store.Controllers;

public class OrderController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userJson = HttpContext.Session.GetString("user");
        var user = JsonSerializer.Deserialize<User>(userJson);
        var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
        var data = dbContext.OrderItems.Include(or => or.Product).Where(or => or.OrderId == order.Id).ToArray();
        var total = dbContext.OrderItems.Include(or => or.Product).Where(or => or.OrderId == order.Id).Sum(or => or.Price);
        ViewData["total"] = total;
        ViewData["data"] = data;
        return View();
    }
    public async Task<IActionResult> Delete(int id)
    {
        var userJson = HttpContext.Session.GetString("user");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null)
        {
            try
            {
                var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
                var check_product = await dbContext.OrderItems.FirstOrDefaultAsync(or => or.ProductId == id);
                if (check_product != null)
                {
                    dbContext.OrderItems.Remove(check_product);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception)
            {
                return Redirect("/Order/Index");
                throw;
            }
        }
        else
        {
            return Redirect("#");
        }
        return Redirect("/Order/Index");
    }
    public async Task<IActionResult> Pay(int Paymentmethod)
    {
        var userJson = HttpContext.Session.GetString("user");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (ModelState.IsValid)
        {
            if (user != null)
            {
                try
                {
                    var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
                    if (order != null)
                    {
                        order.Paid = true;
                        order.PaidMethodId = Paymentmethod;
                        dbContext.Orders.Update(order);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (System.Exception)
                {
                    return Redirect("/Order/Index");
                    throw;
                }
            }
            else
            {
                return Redirect("#");
            }
        }
        return Redirect("/Order/Index");
    }

}