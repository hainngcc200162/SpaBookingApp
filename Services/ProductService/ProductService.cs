using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ServiceResponse<List<GetProductDto>>> AddProduct([FromForm] AddProductDto newProduct)
        {
            var serviceResponse = new ServiceResponse<List<GetProductDto>>();
            var product = _mapper.Map<Product>(newProduct);

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == newProduct.CategoryId);
            if (category != null)
            {
                product.Category = category;
            }

            if (newProduct.Poster != null)
            {
                var fileName = Path.GetFileName(newProduct.Poster.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await newProduct.Poster.CopyToAsync(stream);
                }

                product.PosterName = "/images/" + fileName;
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
            getProductDto.PosterName = dbProduct.PosterName;
            serviceResponse.Data = getProductDto;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetProductDto>> UpdateProduct([FromForm] UpdateProductDto updatedProduct)
        {
            var serviceResponse = new ServiceResponse<GetProductDto>();
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);
                if (product is null)
                {
                    throw new Exception($"Product with ID '{updatedProduct.Id}' not found");
                }

                _mapper.Map(updatedProduct, product);

                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == updatedProduct.CategoryId);
                if (category != null)
                {
                    product.Category = category;
                }

                if (updatedProduct.Poster != null)
                {
                    var fileName = Path.GetFileName(updatedProduct.Poster.FileName);
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await updatedProduct.Poster.CopyToAsync(stream);
                    }

                    product.PosterName = "/images/" + fileName;
                }

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
