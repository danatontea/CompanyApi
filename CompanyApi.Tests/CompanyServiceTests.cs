using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Models;
using CompanyApi.Services;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Tests
{
    public class CompanyServiceTests : IDisposable
    {
        private readonly CompanyDbContext _context;
        private readonly CompanyServices _service;

        public CompanyServiceTests()
        {
            var options = new DbContextOptionsBuilder<CompanyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new CompanyDbContext(options);
            _service = new CompanyServices(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var companies = new[]
            {
                new Company
                {
                    Id = 1,
                    Name = "Test Company 1",
                    StockTicker = "TEST1",
                    Exchange = "NASDAQ",
                    Isin = "US1234567890",
                    Website = "https://test1.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 2,
                    Name = "Test Company 2",
                    StockTicker = "TEST2",
                    Exchange = "NYSE",
                    Isin = "US0987654321",
                    Website = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Companies.AddRange(companies);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllCompanies_ReturnsAllCompanies()
        {
            //Arrange & Act
            var companies = await _service.GetAllCompaniesAsync();
            //assert
            Assert.NotNull(companies);
            Assert.Equal(2, companies.Count());
            Assert.Contains(companies, c => c.Name == "Test Company 1");
            Assert.Contains(companies, c => c.Name == "Test Company 2");
        }
        [Fact]
        public async Task CreateCompanyAsync_DuplicateIsin_ThrowsInvalidOperationException()
        {
            // Arrange
            var createDto = new CreateCompanyDto
            {
                Name = "Duplicate Company",
                StockTicker = "DUP",
                Exchange = "NYSE",
                Isin = "US1234567890", // This ISIN already exists in test data
                Website = "https://duplicate.com"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateCompanyAsync(createDto));

            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ExistingId_ReturnsCompanyDto()
        {
            // Act
            var result = await _service.GetCompanyByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Company 1", result.Name);
            Assert.Equal("US1234567890", result.Isin);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _service.GetCompanyByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task GetCompanyByIsinAsync_ExistingIsin_ReturnsCompanyDto()
        {
            // Act
            var result = await _service.GetCompanyByIsinAsync("US1234567890");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Company 1", result.Name);
            Assert.Equal("US1234567890", result.Isin);
        }

        [Fact]
        public async Task GetCompanyByIsinAsync_NonExistingIsin_ReturnsNull()
        {
            // Act
            var result = await _service.GetCompanyByIsinAsync("XX9999999999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCompanyAsync_ExistingId_UpdatesCompany()
        {
            // Arrange
            var updateDto = new UpdateCompanyDto
            {
                Name = "Updated Company",
                StockTicker = "UPD",
                Exchange = "NASDAQ",
                Website = "https://updated.com"
            };
            // Act
            await _service.UpdateCompanyAsync(1, updateDto);
            // Assert
            var updatedCompany = await _service.GetCompanyByIdAsync(1);
            Assert.NotNull(updatedCompany);
            Assert.Equal("Updated Company", updatedCompany.Name);
            Assert.Equal("https://updated.com", updatedCompany.Website);
        }
        [Fact]
        public async Task UpdateCompanyAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var updateDto = new UpdateCompanyDto
            {
                Name = "Non-existing Company",
                StockTicker = "NONEXIST",
                Exchange = "NYSE",
                Website = "https://nonexisting.com"
            };
            // Act
            var result = await _service.UpdateCompanyAsync(999, updateDto);
            // & Assert
            Assert.Null(result); // Should return null since the company does not exist
        }
        [Fact]
        public async Task DeleteCompanyAsync_ExistingId_DeletesCompany()
        {
            // Act
            await _service.DeleteCompanyAsync(1);
            // Assert
            var deletedCompany = await _service.GetCompanyByIdAsync(1);
            Assert.Null(deletedCompany);
        }
        [Fact]
        public async Task DeleteCompanyAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int nonExistingId = 999;

            // Act
            var result = await _service.DeleteCompanyAsync(nonExistingId);

            // Assert
            Assert.False(result);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
