using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SD_Ecommerce.Data;
using SD_Ecommerce.Models;

namespace SD_Ecommerce.Areas.Admin.Controllers
{   [Area("Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IWebHostEnvironment _he;

        public ProductController(ApplicationDbContext db,IWebHostEnvironment he )
        {
            _db = db;
            _he = he;
        }

        public IActionResult Index()
        {
            return View(_db.Products.Include(c=>c.ProductTypes).ToList());
        }
        //Post Index action method
        [HttpPost]
        public IActionResult Index(Decimal? lowPrice, Decimal? largePrice)
        {
            var products = _db.Products.Include(c => c.ProductTypes).Where(c=>c.Price>=lowPrice&&c.Price<=largePrice).ToList();
            if (lowPrice == null || lowPrice == null)
            {
                products = _db.Products.Include(c => c.ProductTypes).ToList();

            }

            return View(products);
        }


        //Get CreateProduct Method
        public IActionResult CreateProduct()
        {
            ViewData["productTypeId"] = new SelectList(_db.ProductTypes.ToList(),"Id","ProductType");
            return View();
        }

        // Post Create Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Products product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var searchProduct = _db.Products.FirstOrDefault(c=>c.Name==product.Name);
                if (searchProduct != null)
                {
                    ViewBag.message = "This product already exists";
                    ViewData["productTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");
                    return View(product);
                }
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath+"/images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    product.Image ="Images/"+ image.FileName;
                }
                if (image == null)
                {
                    product.Image = "Images/NIF.jpg";
                }
          
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        //Get Edit Action Method
        public ActionResult Edit(int? id)
        {
            ViewData["productTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if(product== null)
            {
                return NotFound();
            }
            return View(product);
        }
        //Post Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit(Products Products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    Products.Image = "Images/" + image.FileName;
                }
                if (image == null)
                {
                    Products.Image = "Images/NIF.jpg";
                }

                _db.Products.Update(Products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Products);

        }
        //Get Details action Method
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
            
        }
        //Get Delete Action Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Post Delete Action Method
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult>DeleteCinfirm(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.FirstOrDefault( c=> c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
