using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public ProductService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> AddProduct(AddProductDto newProduct)
        {
            var serviceResponse = new ServiceResponse<List<GetProductDto>>();
            var product = _mapper.Map<Product>(newProduct);

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == newProduct.CategoryId);
            if (category != null)
            {
                product.Category = category;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Products
                .Include(p => p.Category)
                .Select(p => _mapper.Map<GetProductDto>(p))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> DeleteProduct(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetProductDto>>();
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product is null)
                {
                    throw new Exception($"Product with ID '{id}' not found");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Products
                    .Include(p => p.Category)
                    .Select(p => _mapper.Map<GetProductDto>(p))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> GetAllProducts()
        {
            var serviceResponse = new ServiceResponse<List<GetProductDto>>();
            var dbProducts = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            serviceResponse.Data = _mapper.Map<List<GetProductDto>>(dbProducts);

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProductDto>> GetProductById(int id)
        {
            var serviceResponse = new ServiceResponse<GetProductDto>();
            var dbProduct = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProduct is null)
            {
                throw new Exception($"Product with ID '{id}' not found");
            }

            var getProductDto = _mapper.Map<GetProductDto>(dbProduct);
            serviceResponse.Data = getProductDto;

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProductDto>> UpdateProduct(UpdateProductDto updatedProduct)
        {
            var serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);
                if (product is null)
                {
                    throw new Exception($"Product with ID '{updatedProduct.Id}' not found");
                }

                _mapper.Map(updatedProduct, product);

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.QuantityInStock = updatedProduct.QuantityInStock;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetProductDto>(product);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetProductDto>>> GetProductsByCategoryId(int categoryId)
        {
            var serviceResponse = new ServiceResponse<List<GetProductDto>>();
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Category with ID '{categoryId}' not found";
                return serviceResponse;
            }

            var products = category.Products;
            serviceResponse.Data = _mapper.Map<List<GetProductDto>>(products);

            return serviceResponse;
        }
    }
}
