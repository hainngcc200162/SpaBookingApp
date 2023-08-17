using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SpaBookingApp.Services.SpaProductService
{
    public interface ISpaProductService
    {
        Task<ServiceResponse<List<GetSpaProductDto>>> GetAllProducts(string? search, string? category,
    int? minPrice, int? maxPrice, string? sortBy, string? sortOrder, int pageIndex, int pageSize);
        Task<ServiceResponse<GetSpaProductDto>> GetProductById(int id);
        Task<ServiceResponse<List<GetSpaProductDto>>> AddProduct([FromForm] AddSpaProductDto newProduct);
        Task<ServiceResponse<GetSpaProductDto>> UpdateProduct([FromForm] UpdateSpaProductDto updatedProduct);
        Task<ServiceResponse<List<GetSpaProductDto>>> DeleteProduct(int id);
        // Task<ServiceResponse<List<GetProductDto>>> SortProducts(string sortBy);
    }
}