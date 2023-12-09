using System.Diagnostics;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitofwork;

    public HomeController(ILogger<HomeController> logger,IUnitOfWork unitofwork)
    {
        _logger = logger;
        _unitofwork = unitofwork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList=_unitofwork.Product.GetAll(includeProperties:"Category");
        return View(productList);
    }
    public IActionResult Details(int productid)
    {
        ShoppingCart cart = new()
        {
            Product = _unitofwork.Product.Get(u => u.Id == productid, includeProperties: "Category"),
            Count = 1,
            ProductId = productid
        };
        return View(cart);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity=(ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;
        ShoppingCart cartFromDb=_unitofwork.ShoppingCart.Get(u=>u.ApplicationUserId == userId && u.ProductId==shoppingCart.ProductId);
        if(cartFromDb != null)
        {
            cartFromDb.Count += shoppingCart.Count;
            _unitofwork.ShoppingCart.Update(cartFromDb);
        }
        else
        {
            _unitofwork.ShoppingCart.Add(shoppingCart);
        }
        TempData["success"] = "Cart updated successfully";
        
        _unitofwork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
