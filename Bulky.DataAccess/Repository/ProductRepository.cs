﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;   
        }
        public void Update(Product obj)
        {
            var objFromdb=_db.Products.FirstOrDefault(u=>u.Id==obj.Id);
            if(objFromdb != null)
            {
                objFromdb.Title=obj.Title;
                objFromdb.ISBN = obj.ISBN;
                objFromdb.Price = obj.Price;
                objFromdb.Price50 = obj.Price50;
                objFromdb.ListPrice = obj.ListPrice;
                objFromdb.Price100 = obj.Price100;
                objFromdb.Description = obj.Description;
                objFromdb.CategoryId = obj.CategoryId;
                objFromdb.Author = obj.Author;
                objFromdb.ProductImages=obj.ProductImages;
                // if(obj.ImageUrl!= null)
                // {
                //     objFromdb.ImageUrl=obj.ImageUrl;
                // }


            }
            
        }
    }
}
