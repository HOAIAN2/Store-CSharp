
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Store.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

using System.Threading.Tasks;

namespace Store.Controllers;

public class Admin : BaseController
{
    private int perPage = 21;
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }



    public async Task<ActionResult> Listproduct(string page)
    {
        int pageNumber = 1;
        List<Product>? products;

        int.TryParse(page, out pageNumber);
        if (pageNumber == 0) pageNumber = 1;

        var queryProducts = dbContext.Products;

        var total = queryProducts.Count();
        var from = (pageNumber - 1) * this.perPage + 1;
        var to = from + this.perPage - 1;

        if (pageNumber > 0) products = await queryProducts.Skip((pageNumber - 1) * this.perPage).Take(this.perPage).ToListAsync();
        else products = await queryProducts.Take(this.perPage).ToListAsync();

        var categorys = await dbContext.Categories.ToListAsync();
        ViewData["category"] = categorys;
        ViewData["products"] = products;
        ViewData["total"] = total;
        ViewData["from"] = from;
        ViewData["to"] = to;
        ViewData["previousUrl"] = from != 1 ? $"/Admin/Listproduct?page={pageNumber - 1}" : null;
        ViewData["nextUrl"] = to < total ? $"/Admin/Listproduct?page={pageNumber + 1}" : null;
        return View();
    }

    public class MyFormModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Add other properties as needed
    }

    private readonly IWebHostEnvironment _webHostEnvironment;

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {


        return Json("sdfsf");
    }


    public async Task<ActionResult> ProductDetail(int id)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product != null)
        {
            product.ViewCount++;
            await dbContext.SaveChangesAsync();
        }
        var products = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        return Json(products);
    }

    public async Task<ActionResult> Editproduct(int id, string ProductName, int Price, int Quantity)
    {
        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null) Redirect("/Auth/Login");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null && user.RoleId == 1)
        {
            try
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    product.ProductName = ProductName;
                    product.Price = Price;
                    product.Quantity = Quantity;
                    dbContext.Products.Update(product);
                    await dbContext.SaveChangesAsync();
                    return Json(product);
                }
            }
            catch (System.Exception)
            {
                return Json("fale");
                throw;
            }
        }
        return Redirect("/Auth/Login");
    }

    public async Task<IActionResult> Delete(int id, string redirect)
    {

        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null) return Redirect("/Auth/Login");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null && user.RoleId == 1)
        {
            try
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

                if (product != null)
                {
                    dbContext.Products.Remove(product);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception)
            {
                ViewData["error"] = $"có lỗi xãy ra";
            }
        }
        else
        {
            ViewData["error"] = $"có lỗi xãy ra";
        }
        return Redirect(redirect);
    }


}