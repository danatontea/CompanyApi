using CompanyApi.DTOs;

namespace CompanyApi.Abstractions
{
    public interface ICompanyService
    {
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto);
        Task<CompanyDto?> GetCompanyByIdAsync(int id);
        Task<CompanyDto?> GetCompanyByIsinAsync(string isin);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto?> UpdateCompanyAsync(int id, UpdateCompanyDto updateDto);
        Task<bool> DeleteCompanyAsync(int id);
        Task<bool> CompanyExistsByIsinAsync(string isin);
    }
}
