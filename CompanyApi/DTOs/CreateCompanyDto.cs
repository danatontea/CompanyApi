using System.ComponentModel.DataAnnotations;

namespace CompanyApi.DTOs
{
    public class CreateCompanyDto
    {
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
    }

    public class UpdateCompanyDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string StockTicker { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Exchange { get; set; } = string.Empty;

        [StringLength(500)]
        [Url]
        public string? Website { get; set; }
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StockTicker { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string Isin { get; set; } = string.Empty;
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
