namespace CatalogService.Models
{
    public class InventoryImportReceiptItem
    {
        public int Id { get; set; }
        public int ReceiptId { get; set; }
        public int InventoryBookId { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Note { get; set; }

        public InventoryImportReceipt? Receipt { get; set; }
        public InventoryBook? InventoryBook { get; set; }
    }
}
