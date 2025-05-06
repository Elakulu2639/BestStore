using System.Collections.Generic;
using BestStore.Models;

namespace BestStore.Models
{
    public class DashboardVM
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int ActiveClients { get; set; }
        public int NewCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public int HighValueCustomers { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<ProductSales> TopProducts { get; set; } = new List<ProductSales>();
        public List<SalesData> SalesOverTime { get; set; } = new List<SalesData>();
        public List<OrderStatusData> OrderStatusDistribution { get; set; } = new List<OrderStatusData>();
    }

    public class ProductSales
    {
        public Product Product { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class SalesData
    {
        public string Period { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderStatusData
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}