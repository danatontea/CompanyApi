using CompanyApi.Abstractions;
using CompanyApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(ICompanyService companyService, ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }



        /// <summary>
        /// Get a company by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    _logger.LogInformation("Company with ID {CompanyId} not found", id);
                    return NotFound(new { message = $"Company with ID {id} not found" });
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with ID {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the company" });
            }
        }

        /// <summary>
        /// Get a company by ISIN
        /// </summary>
        [HttpGet("by-isin/{isin}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyByIsin(
            [RegularExpression(@"^[A-Za-z]{2}[A-Za-z0-9]{9}[0-9]$")] string isin)
        {
            try
            {
                var company = await _companyService.GetCompanyByIsinAsync(isin);
                if (company == null)
                {
                    _logger.LogInformation("Company with ISIN {Isin} not found", isin);
                    return NotFound(new { message = $"Company with ISIN {isin} not found" });
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with ISIN {Isin}", isin);
                return StatusCode(500, new { message = "An error occurred while retrieving the company" });
            }
        }

        /// <summary>
        /// Get all companies
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
        {
            try
            {
                var companies = await _companyService.GetAllCompaniesAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all companies");
                return StatusCode(500, new { message = "An error occurred while retrieving companies" });
            }
        }

        /// <summary>
        /// Create a new company
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyDto createDto)
        {
            try
            {
                var company = await _companyService.CreateCompanyAsync(createDto);
                _logger.LogInformation("Created company with ID {CompanyId} and ISIN {Isin}", company.Id, company.Isin);
                return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create company: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, new { message = "An error occurred while creating the company" });
            }
        }


        /// <summary>
        /// Update an existing company
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromBody] UpdateCompanyDto updateDto)
        {
            try
            {
                var company = await _companyService.UpdateCompanyAsync(id, updateDto);
                if (company == null)
                {
                    _logger.LogInformation("Company with ID {CompanyId} not found for update", id);
                    return NotFound(new { message = $"Company with ID {id} not found" });
                }

                _logger.LogInformation("Updated company with ID {CompanyId}", id);
                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company with ID {CompanyId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the company" });
            }
        }


    }
}
