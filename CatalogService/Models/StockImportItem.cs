namespace CatalogService.Models
{
    public class StockImportItem
    {
        public int Id { get; set; }

        public int ReceiptId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        public string Condition { get; set; } = "Good"; // Good / Damaged / Burned / Lost

        public string? Note { get; set; }

        public StockImportReceipt? Receipt { get; set; }

        public Book? Book { get; set; }
    }
}
