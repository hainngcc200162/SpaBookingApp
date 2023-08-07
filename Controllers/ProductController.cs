using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<List<GetProductDto>>> Get(string? search, string? category,
        int? minPrice, int? maxPrice, string? sortOrder, string? sortBy, int pageIndex)
        {
            return Ok(await _productService.GetAllProducts(search, category, minPrice, maxPrice, sortOrder, sortBy, pageIndex));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetProductDto>>> GetSingle(int id)
        {
            return Ok(await _productService.GetProductById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetProductDto>>>> AddProduct([FromForm] AddProductDto newProduct)
        {
            var serviceResponse = await _productService.AddProduct(newProduct);
            return Ok(serviceResponse);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetProductDto>>>> UpdateProduct([FromForm] UpdateProductDto updatedProduct)
        {
            var response = await _productService.UpdateProduct(updatedProduct);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetProductDto>>> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProduct(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // [HttpGet("search")]
        // public async Task<IActionResult> SearchProducts(string? search, string? category,
        //     int? minPrice, int? maxPrice)
        // {
        //     var response = await _productService.SearchProducts(search, category, minPrice, maxPrice);
        //     if (!response.Success)
        //         return BadRequest(response.Message);

        //     return Ok(response.Data);
        // }

        // [HttpGet("Sort")]
        // public async Task<ActionResult<List<GetProductDto>>> SortProducts(string sortBy)
        // {
        //     var response = await _productService.SortProducts(sortBy);
        //     if (!response.Success)
        //         return BadRequest(response.Message);

        //     return Ok(response.Data);
        // }
    }
}