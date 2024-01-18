
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Store.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

using System.IO;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Store.Controllers;

public class Admin : BaseController
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private int perPage = 21;

    public Admin(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

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

        var queryProducts = dbContext.Products.OrderByDescending(p => p.Id);

        var total = queryProducts.Count();
        var from = (pageNumber - 1) * this.perPage + 1;
        var to = from + this.perPage - 1;

        if (pageNumber > 0) products = await queryProducts.Skip((pageNumber - 1) * this.perPage).Take(this.perPage).ToListAsync();
        else products = await queryProducts.Take(this.perPage).ToListAsync();

        var categorys = await dbContext.Categories.ToListAsync();

        string message = TempData["Message"] as string;
        ViewData["test"] = message;
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

    public string GetWwwRootPath()
    {
        string wwwRootPath = _hostingEnvironment.WebRootPath;
        return $"{wwwRootPath}\\assets\\images\\";
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product, IFormFile files, string redirect)
    {
        if (files == null || product.CategoryId == null || product.Price == 0 || product.Quantity == 0)
        {
            TempData["Message"] = "fall";
            return Redirect(redirect);
        }


        var userJson = HttpContext.Session.GetString("user");
        if (userJson == null) Redirect("/Auth/Login");
        var user = JsonSerializer.Deserialize<User>(userJson);
        if (user != null && user.RoleId == 1)
        {
            string baseDirectory = $"{this.GetWwwRootPath()}product\\";
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            string uniqueFileName = DateTime.Now.Ticks + "_" + files.FileName;
            string filePath = Path.Combine(baseDirectory, uniqueFileName);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }
                var NewProduct = new Product();
                NewProduct.Images = uniqueFileName;
                NewProduct.ProductName = product.ProductName;
                NewProduct.Price = product.Price;
                NewProduct.Quantity = product.Quantity;
                dbContext.Products.Add(NewProduct);
                await dbContext.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                System.IO.File.Delete(filePath);
                TempData["Message"] = "fall";
                return Redirect("/Admin/Listproduct");
            }
        }
        else
        {
            TempData["Message"] = "fall";
            return Redirect("/Admin/Listproduct");
        }
        TempData["Message"] = "success";
        return Redirect("/Admin/Listproduct");
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
        TempData["Message"] = "success";
        return Redirect(redirect);
    }


}