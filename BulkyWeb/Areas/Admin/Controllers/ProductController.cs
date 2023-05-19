using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        
        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objCategory = _unitOfWork.Product.GetAll().ToList();
            IEnumerable<SelectListItem> CategoryList= _unitOfWork.Category.GetAll().Select(u=>new SelectListItem
            {
                Text=u.Name,
                Value=u.Id.ToString(),
            });
            return View(objCategory);
        }
        public IActionResult Create()
        {
            ProductVM productvm = new ProductVM()
            {
                CategoryList= _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
            Product = new Product()
        };
           
            return View(productvm);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productvm)
        {
           

            if (ModelState.IsValid)
            {

                _unitOfWork.Product.Add(productvm.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productvm.CategoryList=_unitOfWork.Category.GetAll().Select(u=>new SelectListItem
                {
                    Text=u.Name,
                    Value=u.Id.ToString()
                });
                return View(productvm);
            }
            


        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productfromdb = _unitOfWork.Product.Get(u => u.Id == id);
            //Category? categoryfromdb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            //Category? categoryfromdb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
            if (productfromdb == null) { return NotFound(); }
            return View(productfromdb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {

                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated successfully";
                return RedirectToAction("Index");
            }
            return View();


        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productfromdb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productfromdb == null) { return NotFound(); }
            return View(productfromdb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            Product obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted successfully";
            return RedirectToAction("Index");



        }
    }
}
