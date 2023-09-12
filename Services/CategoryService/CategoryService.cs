using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CategoryService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> AddCategory(AddCategoryDto newCategory)
        {
            var serviceResponse = new ServiceResponse<List<GetCategoryDto>>();

            // Check if the category already exists
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == newCategory.Name);
            if (existingCategory != null)
            {
                // If the category already exists, you can handle the error here or return an error message
                serviceResponse.Success = false;
                serviceResponse.Message = "Category already exists.";
                return serviceResponse;
            }

            var category = _mapper.Map<Category>(newCategory);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Categories
                .Select(c => _mapper.Map<GetCategoryDto>(c))
                .ToListAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> DeleteCategory(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCategoryDto>>();
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category is null)
                {
                    throw new Exception($"Category with ID '{id}' not found");
                }


                category.IsDeleted = true;
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Categories
                    .Select(c => _mapper.Map<GetCategoryDto>(c))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategories(string? searchByName, int pageIndex)
        {
            int pageSize = 10;
            var serviceResponse = new ServiceResponse<List<GetCategoryDto>>();

            IQueryable<Category> query = _context.Categories;

            query = query.Where(c => !c.IsDeleted);

            // Fetch all categories from database
            var allCategories = await query.ToListAsync();


            // Filter by category name
            if (!string.IsNullOrEmpty(searchByName))
            {
                allCategories = allCategories
                    .Where(c => c.Name.StartsWith(searchByName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Order and paginate the filtered categories
            var pagedCategories = allCategories
                .OrderByDescending(s => s.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = allCategories.Count;

            // Create PageInformation
            var pageInfo = new PageInformation
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            // Mapping Category entities to GetCategoryDto
            serviceResponse.Data = pagedCategories.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();

            serviceResponse.PageInformation = pageInfo;

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCategoryDto>> GetCategoryById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCategoryDto>();
            var dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (dbCategory is null)
            {
                throw new Exception($"Category with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetCategoryDto>(dbCategory);
            return serviceResponse;
        }

        // public async Task<ServiceResponse<List<GetProductDto>>> GetProductsByCategoryId(int categoryId)
        // {
        //     var serviceResponse = new ServiceResponse<List<GetProductDto>>();
        //     try
        //     {
        //         var category = await _context.Categories
        //             .Include(c => c.Products)
        //             .FirstOrDefaultAsync(c => c.Id == categoryId);

        //         if (category is null)
        //         {
        //             throw new Exception($"Category with ID '{categoryId}' not found");
        //         }

        //         var productDtos = category.Products.Select(p => new GetProductDto
        //         {
        //             Id = p.Id,
        //             Name = p.Name,
        //             Price = p.Price,
        //             QuantityInStock = p.QuantityInStock,
        //             // CategoryId = p.CategoryId,
        //             CategoryName = category.Name
        //         }).ToList();

        //         serviceResponse.Data = productDtos;
        //     }
        //     catch (Exception ex)
        //     {
        //         serviceResponse.Success = false;
        //         serviceResponse.Message = ex.Message;
        //     }

        //     return serviceResponse;
        // }

        public async Task<ServiceResponse<GetCategoryDto>> UpdateCategory(UpdateCategoryDto updatedCategory)
        {
            var serviceResponse = new ServiceResponse<GetCategoryDto>();
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == updatedCategory.Id);
                if (category is null)
                {
                    throw new Exception($"Category with ID '{updatedCategory.Id}' not found");
                }

                // Kiểm tra xem tên mới đã tồn tại cho một danh mục khác chưa
                var existingCategoryWithSameName = await _context.Categories.FirstOrDefaultAsync(c => c.Name == updatedCategory.Name && c.Id != updatedCategory.Id);
                if (existingCategoryWithSameName != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Category with name '{updatedCategory.Name}' already exists";
                    return serviceResponse;
                }

                _mapper.Map(updatedCategory, category);
                category.Name = updatedCategory.Name;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCategoryDto>(category);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
