using BestStore.Models;
using BestStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BestStore.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 5;
        public AdminOrdersController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Add Dashboard action
        public IActionResult Dashboard()
        {
            var vm = new DashboardVM();

            // Total Revenue
            vm.TotalRevenue = context.Orders
                .Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice) + o.ShippingFee);

            // Total Orders
            vm.TotalOrders = context.Orders.Count();

            // Average Order Value
            vm.AverageOrderValue = vm.TotalOrders > 0
                ? vm.TotalRevenue / vm.TotalOrders
                : 0;

            // Recent Orders (last 5)
            vm.RecentOrders = context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToList();

            // Top Selling Products
            vm.TopProducts = context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new ProductSales
                {
                    Product = g.First().Product,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(ps => ps.TotalSold)
                .Take(5)
                .ToList();

            // Sales Over Time (last 30 days)
            var startDate = DateTime.Now.AddDays(-30);
            vm.SalesOverTime = context.Orders
                .Where(o => o.CreatedAt >= startDate)
                .AsEnumerable()
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesData
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    Amount = g.Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice))
                })
                .OrderBy(s => s.Period)
                .ToList();

            return View(vm);
        }

        public IActionResult Index(int pageIndex)
        {
            IQueryable<Order> query = context.Orders.Include(o => o.Client)
                .Include(o => o.Items).OrderByDescending(o => o.Id);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }


            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var orders = query.ToList();

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }


        public IActionResult Details(int id)
        {
            var order = context.Orders.Include(o => o.Client).Include(o => o.Items)
                .ThenInclude(oi => oi.Product).FirstOrDefault(o => o.Id == id);


            if (order == null)
            {
                return RedirectToAction("Index");
            }


            ViewBag.NumOrders = context.Orders.Where(o => o.ClientId == order.ClientId).Count();

            return View(order);
        }


        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }


            if (payment_status == null && order_status == null)
            {
                return RedirectToAction("Details", new { id });
            }

            if (payment_status != null)
            {
                order.PaymentStatus = payment_status;
            }

            if (order_status != null)
            {
                order.OrderStatus = order_status;
            }

            context.SaveChanges();


            return RedirectToAction("Details", new { id });
        }
    }
}
