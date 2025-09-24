using Supabase;
using api_demo.Dtos.AcademicProducts;
using api_demo.Models;

namespace api_demo.Services
{
    public class AcademicProductService : IAcademicProductService
    {
        private readonly Client _supabase;

        public AcademicProductService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<AcademicProductDto>> GetAllAcademicProductsAsync()
        {
            var response = await _supabase
                .From<AcademicProduct>()
                .Get();

            return response.Models.Select(MapToDto);
        }

        public async Task<AcademicProductDto?> GetAcademicProductByIdAsync(Guid id)
        {
            var response = await _supabase
                .From<AcademicProduct>()
                .Where(p => p.Id == id)
                .Single();

            return response != null ? MapToDto(response) : null;
        }

        public async Task<IEnumerable<AcademicProductDto>> GetAcademicProductsByExpertAsync(Guid expertId)
        {
            var response = await _supabase
                .From<AcademicProduct>()
                .Where(p => p.ExpertId == expertId)
                .Get();

            return response.Models.Select(MapToDto);
        }

        public async Task<AcademicProductDto> CreateAcademicProductAsync(CreateAcademicProductDto createDto, Guid expertId)
        {
            var product = new AcademicProduct
            {
                Id = Guid.NewGuid(),
                ExpertId = expertId,
                Achievements = createDto.Achievements,
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var response = await _supabase
                .From<AcademicProduct>()
                .Insert(product);

            var createdProduct = response.Models.FirstOrDefault();
            if (createdProduct == null)
                throw new InvalidOperationException("Failed to create academic product");

            return MapToDto(createdProduct);
        }

        public async Task<AcademicProductDto?> UpdateAcademicProductAsync(Guid id, UpdateAcademicProductDto updateDto)
        {
            var response = await _supabase
                .From<AcademicProduct>()
                .Where(p => p.Id == id)
                .Single();

            if (response == null)
                return null;

            if (updateDto.Achievements != null)
                response.Achievements = updateDto.Achievements;
            
            if (!string.IsNullOrEmpty(updateDto.Title))
                response.Title = updateDto.Title;
            
            if (updateDto.Description != null)
                response.Description = updateDto.Description;

            response.UpdatedAt = DateTime.UtcNow;

            await response.Update<AcademicProduct>();

            return MapToDto(response);
        }

        public async Task<bool> DeleteAcademicProductAsync(Guid id)
        {
            var response = await _supabase
                .From<AcademicProduct>()
                .Where(p => p.Id == id)
                .Single();

            if (response == null)
                return false;

            await response.Delete<AcademicProduct>();
            return true;
        }

        private static AcademicProductDto MapToDto(AcademicProduct product)
        {
            return new AcademicProductDto
            {
                Id = product.Id,
                ExpertId = product.ExpertId,
                Achievements = product.Achievements,
                Title = product.Title,
                Description = product.Description,
                CreatedAt = product.CreatedAt
            };
        }
    }
}
