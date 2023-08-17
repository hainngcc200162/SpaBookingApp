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

                _context.Categories.Remove(category);
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
            int pageSize = 4;
            var serviceResponse = new ServiceResponse<List<GetCategoryDto>>();

            IQueryable<Category> query = _context.Categories;

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
