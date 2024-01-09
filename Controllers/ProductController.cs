using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Microsoft.EntityFrameworkCore;

namespace Store.Controllers;

public class ProductController : BaseController
{
    private int perPage = 21;
    public async Task<IActionResult> Index()
    {
        var hotProducts = await dbContext.Products.OrderBy(p => Guid.NewGuid()).Take(5).ToListAsync();
        var newProducts = await dbContext.Products.OrderByDescending(p => p.Id).Take(2).ToListAsync();
        var featuredProducts = await dbContext.Products.OrderByDescending(p => p.ViewCount).Take(2).ToListAsync();
        ViewData["hotProducts"] = hotProducts;
        ViewData["newProducts"] = newProducts;
        ViewData["featuredProducts"] = featuredProducts;
        return View();
    }
    public async Task<ActionResult> ProductDetail(int id)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product != null)
        {
            product.ViewCount++;
            await dbContext.SaveChangesAsync();
        }
        var products = await dbContext.Products.OrderBy(p => Guid.NewGuid()).Take(5).ToListAsync();
        ViewData["recommendProducts"] = products;
        ViewData["product"] = product;
        ViewBag.Title = product?.ProductName;
        return View();
    }
    public async Task<ActionResult> Search([FromQuery] string name, [FromQuery] string page)
    {
        int pageNumber = 1;
        List<Product>? products;

        int.TryParse(page, out pageNumber);
        if (pageNumber == 0) pageNumber = 1;

        var queryProducts = dbContext.Products
        .Where(p => p.ProductName!.Contains(name));

        var total = queryProducts.Count();
        var from = (pageNumber - 1) * this.perPage + 1;
        var to = from + this.perPage - 1;

        if (pageNumber > 0) products = await queryProducts.Skip((pageNumber - 1) * this.perPage).Take(this.perPage).ToListAsync();
        else products = await queryProducts.Take(this.perPage).ToListAsync();
        ViewData["products"] = products;
        ViewData["currentSearchParam"] = name;
        ViewData["currentPage"] = name;
        ViewData["Title"] = name;
        ViewData["total"] = total;
        ViewData["from"] = from;
        ViewData["to"] = to;

        ViewData["previousUrl"] = from != 1 ? $"/Product/Search?name={name}&page={pageNumber - 1}" : null;
        ViewData["nextUrl"] = to < total ? $"/Product/Search?name={name}&page={pageNumber + 1}" : null;
        // return Json(products);
        return View();
    }
}