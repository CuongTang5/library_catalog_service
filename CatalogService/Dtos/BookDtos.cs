using System.Collections.Generic;

namespace CatalogService.Dtos
{
    public class CreateBookDto
    {
        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public string NhaSanXuat { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public string? ImageUrl { get; set; }
        public string? MoTa { get; set; }
        public string? Isbn { get; set; }
        public string? TheLoai { get; set; }
        public int? NamXuatBan { get; set; }
        public string? TomTat { get; set; }
    }

    public class UpdateBookDto
    {
        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public string NhaSanXuat { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public string? ImageUrl { get; set; }
        public string? MoTa { get; set; }
        public string? Isbn { get; set; }
        public string? TheLoai { get; set; }
        public int? NamXuatBan { get; set; }
        public string? TomTat { get; set; }
    }

    public class BookResponseDto
    {
        public int Id { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;
        public string NhaSanXuat { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public int SoBanDaMuon { get; set; }
        public int SoBanConLai { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? MoTa { get; set; }
        public string? Isbn { get; set; }
        public string? TheLoai { get; set; }
        public int? NamXuatBan { get; set; }
        public string? TomTat { get; set; }
        public double DanhGiaTrungBinh { get; set; }
        public int SoLuotDanhGia { get; set; }
        public List<object>? LatestReviews { get; set; }
    }
}
