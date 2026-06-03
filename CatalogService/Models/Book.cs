namespace CatalogService.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string TenSach { get; set; } = string.Empty;

        public string TacGia { get; set; } = string.Empty;

        public string NhaSanXuat { get; set; } = string.Empty;

        public int SoLuong { get; set; }

        public int SoBanDaMuon { get; set; }

        public string? ImageUrl { get; set; }

        public string? MoTa { get; set; }

        public string? Isbn { get; set; }

        public double DanhGiaTrungBinh { get; set; } = 0;

        public int SoLuotDanhGia { get; set; } = 0;

        public string? TheLoai { get; set; }

        public int SoBanConLai
        {
            get
            {
                return SoLuong - SoBanDaMuon;
            }
        }

        public string TrangThai
        {
            get
            {
                return SoBanConLai > 0
                    ? "Có thể mượn"
                    : "Hết sách";
            }
        }
    }
}