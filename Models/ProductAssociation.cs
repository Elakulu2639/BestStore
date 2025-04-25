namespace BestStore.Models
{
    public class ProductAssociation
    {
        public int ProductId { get; set; }
        public int CoPurchaseProductId { get; set; }
        public float Score { get; set; }
    }
}
