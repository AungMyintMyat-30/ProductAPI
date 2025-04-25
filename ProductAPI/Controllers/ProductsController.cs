using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Interfaces;
using ProductCore.Entities;
using ProductCore.Helper;
using ProductCore.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProductAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service) => _service = service;

        /// <summary>
        /// Product lists
        /// </summary>
        /// <param name="skipRows"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("{skipRows}/{pageSize}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(int skipRows, int pageSize)
        {
            PaginationResultModel<List<Product>> result = await _service.GetAllAsync(skipRows, pageSize);
            return ResponseHelper.OK_Result(new { result }, null);
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null) return ResponseHelper.NotFound_Request(null, "Product not found!");

            return ResponseHelper.OK_Result(new { result }, "Product retrieved successfully!");
        }

        /// <summary>
        /// Product create
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromForm] Product product)
        {
            var result = await _service.GetByIdAsync(product.Id);
            if (result is not null) return ResponseHelper.Bad_Request(null, "Product is already exist!");

            var created = await _service.CreateAsync(product);
            if (created is null) return ResponseHelper.Bad_Request(null, "Product cannot added!");
            return ResponseHelper.Created_Result("api/product", "Product have been added successfully!");
        }

        /// <summary>
        /// Product update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id) return ResponseHelper.Bad_Request(null, "Product id must be same!");

            var updated = await _service.UpdateAsync(product);
            if (updated is false) return ResponseHelper.NotFound_Request(null, "Product not found!");

            return ResponseHelper.OK_Result(new { updated }, "Product updated successfully!");
        }

        /// <summary>
        /// Product delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (deleted is false) return ResponseHelper.NotFound_Request(null, "Product not found!");
            return ResponseHelper.OK_Result(new { deleted }, "Product deleted successfully!");
        }
    }
}
