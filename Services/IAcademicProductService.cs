using api_demo.Dtos.AcademicProducts;
using api_demo.Models;

namespace api_demo.Services
{
    public interface IAcademicProductService
    {
        Task<IEnumerable<AcademicProductDto>> GetAllAcademicProductsAsync();
        Task<AcademicProductDto?> GetAcademicProductByIdAsync(Guid id);
        Task<IEnumerable<AcademicProductDto>> GetAcademicProductsByExpertAsync(Guid expertId);
        Task<AcademicProductDto> CreateAcademicProductAsync(CreateAcademicProductDto createDto, Guid expertId);
        Task<AcademicProductDto?> UpdateAcademicProductAsync(Guid id, UpdateAcademicProductDto updateDto);
        Task<bool> DeleteAcademicProductAsync(Guid id);
    }
}
