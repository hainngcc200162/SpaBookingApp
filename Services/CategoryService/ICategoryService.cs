using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SpaBookingApp.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategories();
        Task<ServiceResponse<GetCategoryDto>> GetCategoryById(int id);
        Task<ServiceResponse<List<GetCategoryDto>>> AddCategory(AddCategoryDto newCategory);
        Task<ServiceResponse<GetCategoryDto>> UpdateCategory(UpdateCategoryDto updatedCategory);
        Task<ServiceResponse<List<GetCategoryDto>>> DeleteCategory(int id);

        Task<ServiceResponse<List<GetProductDto>>> GetProductsByCategoryId(int categoryId);
    }
}