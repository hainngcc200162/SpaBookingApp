using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpaBookingApp.Services.SpaProductService;


namespace SpaBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaProductController : ControllerBase
    {
        private readonly ISpaProductService _spaproductService;

        public SpaProductController(ISpaProductService spaproductService)
        {
            _spaproductService = spaproductService;
        }


        [HttpGet("GetAll")]
        public async Task<ActionResult<List<GetSpaProductDto>>> Get(string? search, string? category,
        int? minPrice, int? maxPrice, string? sortOrder, string? sortBy, int pageIndex)
        {
            return Ok(await _spaproductService.GetAllProducts(search, category, minPrice, maxPrice, sortOrder, sortBy, pageIndex));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetSpaProductDto>>> GetSingle(int id)
        {
            return Ok(await _spaproductService.GetProductById(id));
        }


        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetSpaProductDto>>>> AddProduct([FromForm] AddSpaProductDto newProduct)
        {
            var serviceResponse = await _spaproductService.AddProduct(newProduct);
            return Ok(serviceResponse);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetSpaProductDto>>>> UpdateProduct([FromForm] UpdateSpaProductDto updatedProduct)
        {
            var response = await _spaproductService.UpdateProduct(updatedProduct);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetSpaProductDto>>> DeleteProduct(int id)
        {
            var response = await _spaproductService.DeleteProduct(id);
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
        //     var response = await _spaproductService.SearchProducts(search, category, minPrice, maxPrice);
        //     if (!response.Success)
        //         return BadRequest(response.Message);

        //     return Ok(response.Data);
        // }

        // [HttpGet("Sort")]
        // public async Task<ActionResult<List<GetProductDto>>> SortProducts(string sortBy)
        // {
        //     var response = await _spaproductService.SortProducts(sortBy);
        //     if (!response.Success)
        //         return BadRequest(response.Message);

        //     return Ok(response.Data);
        // }
    }
}