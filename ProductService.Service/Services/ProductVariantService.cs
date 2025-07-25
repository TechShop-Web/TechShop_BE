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

        //public async Task<ProductVariant?> GetByIdAsync(int id)
        //{
        //    return await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
        //}

        public async Task<dynamic?> GetByIdAsync(int id)
        {
            var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
            if (variant == null)
            {
                return null;
            }

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(variant.ProductId);
            if (product == null)
            {
                return null;
            }

            var result = new
            {
                productName = product.Name,
                productId = product.Id,
                variant.Id,
                variant.ConfigLabel,
                variant.Price,
                variant.Stock,
                variant.CreatedAt
            };

            return result;
        }





        public async Task<ProductVariant> CreateAsync(ProductVariant variant)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(variant.ProductId);
            if (product == null)
            {
                throw new ArgumentException("ProductId is invalid or does not exist.");
            }

            if (string.IsNullOrEmpty(variant.ConfigLabel))
            {
                throw new ArgumentException("ConfigLabel cannot be empty.");
            }

            if (variant.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than 0.");
            }

            if (variant.Stock <= 0)
            {
                throw new ArgumentException("Stock must be greater than 0.");
            }

            await _unitOfWork.ProductVariantRepository.AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();

            return variant;
        }


        public async Task<bool> UpdateAsync(ProductVariant variant)
        {
            var existingVariant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(variant.Id);
            if (existingVariant == null)
            {
                throw new ArgumentException("ProductVariant not found.");
            }

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(variant.ProductId);
            if (product == null)
            {
                throw new ArgumentException("ProductId is invalid or does not exist.");
            }

            if (string.IsNullOrEmpty(variant.ConfigLabel))
            {
                throw new ArgumentException("ConfigLabel cannot be empty.");
            }

            if (variant.Price < 0)
            {
                throw new ArgumentException("Price cannot be negative.");
            }

            if (variant.Stock < 0)
            {
                throw new ArgumentException("Stock cannot be negative.");
            }

            existingVariant.ConfigLabel = variant.ConfigLabel;
            existingVariant.Price = variant.Price;
            existingVariant.Stock = variant.Stock;
            existingVariant.CreatedAt = variant.CreatedAt;

            _unitOfWork.ProductVariantRepository.Update(existingVariant);
            await _unitOfWork.SaveChangesAsync();
            return true;
            //return await _unitOfWork.SaveChangesAsync()>0;  
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
            if (variant == null) return false;

            _unitOfWork.ProductVariantRepository.Delete(variant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetVariantsByProductIdWithProductNameAsync(int productId)
        {

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return null;
            }

            var variants = await _unitOfWork.ProductVariantRepository.FindAsync(v => v.ProductId == productId);
            var result = new
            {
                productName = product.Name,
                productId = product.Id,
                variants = variants.Select(v => new
                {
                    v.Id,
                    v.ConfigLabel,
                    v.Price,
                    v.Stock,
                    v.CreatedAt
                }).ToList()
            };

            return new[] { result };
        }

    }

}
