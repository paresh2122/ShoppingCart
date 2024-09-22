using System.Diagnostics;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;


namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitofwork;
    private readonly ICacheService _cacheService;


    public HomeController(ILogger<HomeController> logger,IUnitOfWork unitofwork, ICacheService cacheService)
    {
        _logger = logger;
        _unitofwork = unitofwork;
        _cacheService = cacheService;
        
    }

    public async Task< IActionResult> Index()
    {
        var claimsIdentity=(ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if(claim !=null){
            HttpContext.Session.SetInt32(SD.SessionCart,_unitofwork.ShoppingCart.GetAll(u=>u.ApplicationUserId == claim.Value).Count());
        }
        //IEnumerable<Product> productList = _unitofwork.Product.GetAll(includeProperties: "Category,ProductImages");
        //return View(productList);
        string cacheKey = "productListCache";
        var productList = await _cacheService.GetAsync<IEnumerable<Product>>(cacheKey);

        // If the cache is empty, fetch the data from the database
        if (productList == null)
        {
            productList = _unitofwork.Product.GetAll(includeProperties: "Category,ProductImages");

            // Set cache options (e.g., expire after 5 minutes)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            // Cache the product list
            await _cacheService.SetAsync(cacheKey, productList, cacheOptions);
        }
        return View(productList);




    }
    public IActionResult Details(int productid)
    {
        ShoppingCart cart = new()
        {
            Product = _unitofwork.Product.Get(u => u.Id == productid, includeProperties: "Category,ProductImages"),
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
            _unitofwork.Save();
        }
        else
        {
            _unitofwork.ShoppingCart.Add(shoppingCart);
            _unitofwork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,_unitofwork.ShoppingCart.GetAll(u=>u.ApplicationUserId == userId).Count());
        }
        TempData["success"] = "Cart updated successfully";
        
        
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
