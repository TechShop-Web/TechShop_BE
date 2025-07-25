using ProductService.Repository.Models;
using ProductService.Repository.Repositories;

namespace ProductService.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.ProductRepository.GetByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("CategoryId is invalid or does not exist.");
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product name cannot be empty.");
            }

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await _unitOfWork.ProductRepository.GetByIdAsync(product.Id);
            if (existingProduct == null)
            {
                throw new ArgumentException("Product not found.");
            }

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("CategoryId is invalid or does not exist.");
            }

            if (string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product name cannot be empty.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Brand = product.Brand;
            existingProduct.CreatedAt = product.CreatedAt;
            existingProduct.CategoryId = product.CategoryId;

            _unitOfWork.ProductRepository.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.ProductRepository.Delete(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
