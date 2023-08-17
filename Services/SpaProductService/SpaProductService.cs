using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;
using SpaBookingApp.Services;


namespace SpaBookingApp.Services.SpaProductService
{
    public class SpaProductService : ISpaProductService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public SpaProductService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetSpaProductDto>>> AddProduct([FromForm] AddSpaProductDto newProduct)
        {
            var serviceResponse = new ServiceResponse<List<GetSpaProductDto>>();
            var product = _mapper.Map<SpaProduct>(newProduct);

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

            _context.Update(product);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.SpaProducts
                .Include(p => p.Category)
                .Select(p => _mapper.Map<GetSpaProductDto>(p))
                .ToListAsync();

            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetSpaProductDto>>> DeleteProduct(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetSpaProductDto>>();
            try
            {
                var product = await _context.SpaProducts.FirstOrDefaultAsync(p => p.Id == id);
                if (product is null)
                {
                    throw new Exception($"Product with ID '{id}' not found");
                }

                _context.SpaProducts.Remove(product);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.SpaProducts
                    .Include(p => p.Category)
                    .Select(p => _mapper.Map<GetSpaProductDto>(p))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSpaProductDto>>> GetAllProducts(string? search, string? category,
    int? minPrice, int? maxPrice, string? sortBy, string? sortOrder, int pageIndex, int pageSize)
        {
            var serviceResponse = new ServiceResponse<List<GetSpaProductDto>>();
            var dbProducts = await _context.SpaProducts
                .Include(p => p.Category)
                .ToListAsync();

            var allProducts = _mapper.Map<List<GetSpaProductDto>>(dbProducts);

            // Bước 1: Lọc các sản phẩm dựa vào các tiêu chí tìm kiếm (nếu có)
            var filteredProducts = allProducts;

            if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(category) || minPrice.HasValue || maxPrice.HasValue)
            {
                filteredProducts = allProducts
                    .Where(product =>
                        (string.IsNullOrEmpty(search) || product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(category) || product.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase)) &&
                        (!minPrice.HasValue || product.Price >= minPrice.Value) &&
                        (!maxPrice.HasValue || product.Price <= maxPrice.Value)
                    )
                    .ToList();
            }

            // Bước 2: Kiểm tra kết quả và cập nhật ServiceResponse
            if (filteredProducts.Any())
            {
                // Bước 3: Sắp xếp theo các trường được chỉ định bởi sortBy và sortOrder
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortBy.ToLower() == "name")
                    {
                        filteredProducts = string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "asc"
                            ? filteredProducts.OrderBy(p => p.Name).ToList()
                            : filteredProducts.OrderByDescending(p => p.Name).ToList();
                    }
                    else if (sortBy.ToLower() == "category")
                    {
                        filteredProducts = string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "asc"
                            ? filteredProducts.OrderBy(p => p.CategoryName).ToList()
                            : filteredProducts.OrderByDescending(p => p.CategoryName).ToList();
                    }
                    else if (sortBy.ToLower() == "price")
                    {
                        filteredProducts = string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "asc"
                            ? filteredProducts.OrderBy(p => p.Price).ToList()
                            : filteredProducts.OrderByDescending(p => p.Price).ToList();
                    }
                }
                else
                {
                    // Xử lý trường hợp mặc định, sắp xếp theo ID khi sortBy không được cung cấp
                    filteredProducts = string.IsNullOrEmpty(sortOrder) || sortOrder.ToLower() == "desc"
                        ? filteredProducts.OrderByDescending(p => p.Id).ToList()
                        : filteredProducts.OrderBy(p => p.Id).ToList();

                }

                // Bước 4: Thực hiện phân trang và lấy danh sách sản phẩm cho trang hiện tại
                var pagedProducts = filteredProducts.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                // Tạo đối tượng PageInfo và cập nhật vào serviceResponse
                var pageInfo = new PageInformation
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = filteredProducts.Count,
                    TotalPages = (int)Math.Ceiling((double)filteredProducts.Count / pageSize)
                };

                serviceResponse.Success = true;
                serviceResponse.Data = pagedProducts;
                serviceResponse.PageInformation = pageInfo;
            }
            else
            {
                serviceResponse.Success = true;
                serviceResponse.Message = "Not Found";
                serviceResponse.Data = new List<GetSpaProductDto>();
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSpaProductDto>> GetProductById(int id)
        {
            var serviceResponse = new ServiceResponse<GetSpaProductDto>();
            var dbProduct = await _context.SpaProducts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbProduct is null)
            {
                throw new Exception($"Product with ID '{id}' not found");
            }

            var getProductDto = _mapper.Map<GetSpaProductDto>(dbProduct);
            getProductDto.PosterName = dbProduct.PosterName;
            serviceResponse.Data = getProductDto;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSpaProductDto>> UpdateProduct([FromForm] UpdateSpaProductDto updatedProduct)
        {
            var serviceResponse = new ServiceResponse<GetSpaProductDto>();
            try
            {
                var product = await _context.SpaProducts
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
                serviceResponse.Data = _mapper.Map<GetSpaProductDto>(product);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSpaProductDto>>> GetProductsByCategoryId(int categoryId)
        {
            var serviceResponse = new ServiceResponse<List<GetSpaProductDto>>();
            var category = await _context.Categories
                .Include(c => c.SpaProducts)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Category with ID '{categoryId}' not found";
                return serviceResponse;
            }

            var products = category.SpaProducts;
            serviceResponse.Data = _mapper.Map<List<GetSpaProductDto>>(products);

            return serviceResponse;
        }

    }
}
