using System;
using System.Collections.Generic;

namespace CatalogService.Models
{
    public class InventoryImportReceipt
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // Manual / Excel
        public string? Note { get; set; }
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }

        public ICollection<InventoryImportReceiptItem> Items { get; set; } = new List<InventoryImportReceiptItem>();
    }
}
