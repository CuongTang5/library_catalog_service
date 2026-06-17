using System;

namespace CatalogService.Models
{
    public class InventoryBook
    {
        public int Id { get; set; }

        public string TenSach { get; set; } = string.Empty;

        public string TacGia { get; set; } = string.Empty;

        public string NhaSanXuat { get; set; } = string.Empty;

        public string? TheLoai { get; set; }

        public int SoLuongTonKho { get; set; }

        public string? ImageUrl { get; set; }

        public string? MoTa { get; set; }

        public string? Isbn { get; set; }

        public int? NamXuatBan { get; set; }

        public string? TomTat { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
