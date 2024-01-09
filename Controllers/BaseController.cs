namespace Store.Controllers;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Microsoft.EntityFrameworkCore;

[Controller]
public class BaseController : Controller
{
    // returns the current authenticated account (null if not logged in)
    public StoreContext dbContext = new StoreContext();
}