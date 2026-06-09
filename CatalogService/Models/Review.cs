namespace CatalogService.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string? UserId { get; set; }
        public string? CardNumber { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
