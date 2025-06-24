using ProductService.Repository.Models;
using ProductService.Repository.Repositories;

namespace ProductService.Service.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            return await _unitOfWork.ProductVariantRepository.GetAllAsync();
        }

        public async Task<ProductVariant?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
        }

        public async Task<ProductVariant> CreateAsync(ProductVariant variant)
        {
            await _unitOfWork.ProductVariantRepository.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return variant;
        }

        public async Task<bool> UpdateAsync(ProductVariant variant)
        {
            _unitOfWork.ProductVariantRepository.Update(variant);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
            if (variant == null) return false;

            _unitOfWork.ProductVariantRepository.Delete(variant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
