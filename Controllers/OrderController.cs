
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
        if (userJson == null) return Redirect("/Auth/Login");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
            if (order != null)
            {
                List<OrderItem> listproduct = new List<OrderItem>();
                var data = dbContext.OrderItems.Include(or => or.Product).Where(or => or.OrderId == order.Id).ToArray();
                foreach (var item in data)
                {
                    listproduct.Add(item);
                }
                var total = dbContext.OrderItems.Include(or => or.Product).Where(or => or.OrderId == order.Id).Sum(or => or.Price * or.Quantity);
                ViewData["total"] = total;
                ViewData["data"] = listproduct.OrderByDescending(p => p.Order.OrderDate).ToList();
                ViewData["type"] = "bought";
            }
            else
            {
                var already_pay = dbContext.Orders.Where(or => or.UserId == user.Id && or.Paid).ToArray();
                List<OrderItem> listproduct = new List<OrderItem>();
                foreach (var item in already_pay)
                {
                    var data = dbContext.OrderItems.Include(or => or.Product).Where(or => or.OrderId == item.Id).ToArray();
                    foreach (var item2 in data)
                    {
                        listproduct.Add(item2);
                    }
                }
                ViewData["type"] = "already_bought";
                ViewData["data"] = listproduct.OrderByDescending(p => p.Order.OrderDate).ToList();
            }
        }
        return View();
    }
    public async Task<IActionResult> AddOrder(int id, int quantity)
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null) return Redirect("/Auth/Login");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null)
        {
            try
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
                    if (order != null)
                    {
                        var neworderitem = new OrderItem();
                        neworderitem.OrderId = order.Id;
                        neworderitem.Quantity = quantity;
                        neworderitem.ProductId = product.Id;
                        neworderitem.Price = product.Price;
                        dbContext.OrderItems.Add(neworderitem);
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        var neworder = new Order();
                        neworder.OrderDate = DateTime.Now;
                        neworder.UserId = user.Id;
                        neworder.PaidMethodId = null;
                        neworder.Paid = false;
                        neworder.VoucherId = null;
                        dbContext.Orders.Add(neworder);
                        await dbContext.SaveChangesAsync();
                        var neworderitem = new OrderItem();
                        neworderitem.OrderId = neworder.Id;
                        neworderitem.Quantity = quantity;
                        neworderitem.ProductId = product.Id;
                        neworderitem.Price = product.Price;
                        dbContext.OrderItems.Add(neworderitem);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    return Redirect("/Order/Index");
                }
            }
            catch (System.Exception)
            {
                return Redirect("/Order/Index");
            }
        }

        return Redirect("/Order/Index");
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
                Console.WriteLine(123);
                return Redirect("/Order/Index");
            }
        }
        else
        {
            return Redirect("#");
        }
        return Redirect("/Order/Index");
    }

    public async Task<IActionResult> addQuantity(int id, string type)
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson != null)
        {
            var user = JsonSerializer.Deserialize<User>(userJson);
            if (user != null)
            {
                var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.UserId == user.Id && !order.Paid);
                if (order != null)
                {
                    var orderitem = await dbContext.OrderItems.FirstOrDefaultAsync(or => or.OrderId == order.Id && or.ProductId == id);
                    if (orderitem != null)
                    {
                        if (type == "dec")
                        {
                            orderitem.Quantity = orderitem.Quantity + 1;
                        }
                        else if (type == "inc")
                        {
                            orderitem.Quantity = orderitem.Quantity - 1;
                        }
                        else
                        {
                            return Redirect("/Order/Index");
                        }
                        dbContext.OrderItems.Update(orderitem);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
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
                        order.PaidMethodId = Paymentmethod == 1 || Paymentmethod == 2 ? Paymentmethod : 1;
                        dbContext.Orders.Update(order);
                        await dbContext.SaveChangesAsync();
                        var orderitems = await dbContext.OrderItems.Where(o => o.OrderId == order.Id).ToListAsync();
                        foreach (var item in orderitems)
                        {
                            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                product.Quantity = product.Quantity - item.Quantity;
                                product.SoldQuantity += item.Quantity;
                                dbContext.Products.Update(product);
                                await dbContext.SaveChangesAsync();
                            }
                        }

                    }
                }
                catch (System.Exception)
                {
                    return Redirect("/Order/Index");
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