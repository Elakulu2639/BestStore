using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestStore.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }

        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }

        // navigation property
        public Product Product { get; set; } = new Product();
    }
}
