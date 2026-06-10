namespace CatalogService.Models
{
    public class StockImportReceipt
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public DateTime ImportDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public string? Note { get; set; }

        public string Status { get; set; } = "Pending"; // Pending / Approved / Cancelled

        public DateTime CreatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public ICollection<StockImportItem> Items { get; set; } = new List<StockImportItem>();
    }
}
