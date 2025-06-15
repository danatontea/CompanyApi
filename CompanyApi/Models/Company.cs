using System.ComponentModel.DataAnnotations;

namespace CompanyApi.Models
{
    public class Company
    {

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string StockTicker { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Exchange { get; set; } = string.Empty;

        [Required]
        [StringLength(12)]
        [RegularExpression(@"^[A-Za-z]{2}[A-Za-z0-9]{9}[0-9]$",
            ErrorMessage = "ISIN must be 12 characters: 2 letters followed by 9 alphanumeric characters and 1 digit")]
        public string Isin { get; set; } = string.Empty;

        [StringLength(500)]
        [Url]
        public string? Website { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
