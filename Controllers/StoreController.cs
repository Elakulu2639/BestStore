﻿using System.Security.Claims;
using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;
        private readonly RecommendationService recommendationService;
        public StoreController(ApplicationDbContext context, RecommendationService recommendationService)
        {
            this.context = context;
            this.recommendationService = recommendationService;
        }

        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;

            // search functionality
            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(search));
            }


            // filter functionality
            if (brand != null && brand.Length > 0)
            {
                query = query.Where(p => p.Brand.Contains(brand));
            }

            if (category != null && category.Length > 0)
            {
                query = query.Where(p => p.Category.Contains(category));
            }

            // sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                // newest products first
                query = query.OrderByDescending(p => p.Id);
            }



            // pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };

            return View(storeSearchModel);
        }


        public IActionResult Details(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
                return RedirectToAction("Index");

            string? userId = null;
            if (User.Identity.IsAuthenticated)
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Try personalized first (if userId is null, service treats as global)
            var recommendations = recommendationService.GetFrequentlyBoughtTogether(id, userId);
            // Always fallback to global if none found
            if (!recommendations.Any())
                recommendations = recommendationService.GetFrequentlyBoughtTogether(id, null);

            ViewData["Recommendations"] = recommendations;
            return View(product);
        }
    }
}