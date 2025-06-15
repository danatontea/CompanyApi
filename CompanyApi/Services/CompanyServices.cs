using CompanyApi.Abstractions;
using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Services
{
    public class CompanyServices : ICompanyService
    {
        private readonly CompanyDbContext _context;

        public CompanyServices(CompanyDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto)
        {
            // Check if ISIN already exists
            if (await CompanyExistsByIsinAsync(createDto.Isin))
            {
                throw new InvalidOperationException($"A company with ISIN '{createDto.Isin}' already exists.");
            }

            var company = new Company
            {
                Name = createDto.Name,
                StockTicker = createDto.StockTicker,
                Exchange = createDto.Exchange,
                Isin = createDto.Isin,
                Website = createDto.Website,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return MapToDto(company);
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            return company != null ? MapToDto(company) : null;
        }

        public async Task<CompanyDto?> GetCompanyByIsinAsync(string isin)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Isin == isin);
            return company != null ? MapToDto(company) : null;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await _context.Companies
                .OrderBy(c => c.Name)
                .ToListAsync();
            return companies.Select(MapToDto);
        }

        public async Task<CompanyDto?> UpdateCompanyAsync(int id, UpdateCompanyDto updateDto)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return null;

            company.Name = updateDto.Name;
            company.StockTicker = updateDto.StockTicker;
            company.Exchange = updateDto.Exchange;
            company.Website = updateDto.Website;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(company);
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
                return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompanyExistsByIsinAsync(string isin)
        {
            return await _context.Companies.AnyAsync(c => c.Isin == isin);
        }

        private static CompanyDto MapToDto(Company company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                StockTicker = company.StockTicker,
                Exchange = company.Exchange,
                Isin = company.Isin,
                Website = company.Website,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };
        }
    }


    public static class CompanyEndpoints
    {
        public static void MapCompanyEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Company").WithTags(nameof(Company));

            group.MapGet("/", async (CompanyDbContext db) =>
            {
                return await db.Companies.ToListAsync();
            })
            .WithName("GetAllCompanies")
            .WithOpenApi();

            group.MapGet("/{id}", async Task<Results<Ok<Company>, NotFound>> (int id, CompanyDbContext db) =>
            {
                return await db.Companies.AsNoTracking()
                    .FirstOrDefaultAsync(model => model.Id == id)
                    is Company model
                        ? TypedResults.Ok(model)
                        : TypedResults.NotFound();
            })
            .WithName("GetCompanyById")
            .WithOpenApi();

            group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Company company, CompanyDbContext db) =>
            {
                var affected = await db.Companies
                    .Where(model => model.Id == id)
                    .ExecuteUpdateAsync(setters => setters
                      .SetProperty(m => m.Id, company.Id)
                      .SetProperty(m => m.Name, company.Name)
                      .SetProperty(m => m.StockTicker, company.StockTicker)
                      .SetProperty(m => m.Exchange, company.Exchange)
                      .SetProperty(m => m.Isin, company.Isin)
                      .SetProperty(m => m.Website, company.Website)
                      .SetProperty(m => m.CreatedAt, company.CreatedAt)
                      .SetProperty(m => m.UpdatedAt, company.UpdatedAt)
                      );
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("UpdateCompany")
            .WithOpenApi();

            group.MapPost("/", async (Company company, CompanyDbContext db) =>
            {
                db.Companies.Add(company);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/api/Company/{company.Id}", company);
            })
            .WithName("CreateCompany")
            .WithOpenApi();

            group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, CompanyDbContext db) =>
            {
                var affected = await db.Companies
                    .Where(model => model.Id == id)
                    .ExecuteDeleteAsync();
                return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
            })
            .WithName("DeleteCompany")
            .WithOpenApi();
        }
    }
}
