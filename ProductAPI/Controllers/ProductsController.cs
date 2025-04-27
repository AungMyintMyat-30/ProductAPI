using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Interfaces;
using ProductCore.Entities;
using ProductCore.Helper;
using ProductCore.Models;

namespace ProductAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService service, ILogger<ProductsController> logger) : ControllerBase
    {
        private readonly IProductService _service = service;
        private readonly ILogger<ProductsController> _logger = logger;

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
            _logger.LogInformation($"ProductsController.GetAll called with skipRows: {skipRows}, pageSize: {pageSize}");
            try
            {
                PaginationResultModel<List<Product>> result = await _service.GetAllAsync(skipRows, pageSize);
                return ResponseHelper.OK_Result(new { result }, "Products retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductsController.GetAll");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
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
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"ProductsController.GetById called with id: {id}");
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result is null)
                {
                    _logger.LogWarning($"Product with id: {id} not found.");
                    return ResponseHelper.NotFound_Request(null, "Product not found!");
                }

                return ResponseHelper.OK_Result(new { result }, "Product retrieved successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ProductsController.GetById for id: {id}");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
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
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromForm] Product product)
        {
            _logger.LogInformation($"ProductsController.Create called for product: {product.StockName}");
            try
            {
                var result = await _service.GetByIdAsync(product.Id);
                if (result is not null)
                {
                    _logger.LogWarning($"Product with id: {product.Id} already exists.");
                    return ResponseHelper.Bad_Request(null, "Product is already exist!");
                }

                var created = await _service.CreateAsync(product);
                if (created is null)
                {
                    _logger.LogError($"Failed to create product: {product.StockName}");
                    return ResponseHelper.Bad_Request(null, "Product cannot added!");
                }
                return ResponseHelper.Created_Result("api/products", "Product created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ProductsController.Create for product: {product.StockName}");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
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
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            _logger.LogInformation($"ProductsController.Update called for id: {id}, product: {product.StockName}");
            try
            {
                if (id != product.Id)
                {
                    _logger.LogWarning($"Product id in route ({id}) does not match product id in body ({product.Id}).");
                    return ResponseHelper.Bad_Request(null, "Product ID mismatch!");
                }
                var updated = await _service.UpdateAsync(product);
                if (!updated)
                {
                    _logger.LogWarning($"Product with id: {id} not found for update.");
                    return ResponseHelper.NotFound_Request(null, "Product not found!");
                }

                return ResponseHelper.OK_Result(new { updated }, "Product updated successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ProductsController.Update for id: {id}, product: {product.StockName}");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
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
        [ProducesResponseType(typeof(DefaultResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"ProductsController.Delete called for id: {id}");
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Product with id: {id} not found for deletion.");
                    return ResponseHelper.NotFound_Request(null, "Product not found!");
                }
                return ResponseHelper.OK_Result(new { deleted }, "Product deleted successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in ProductsController.Delete for id: {id}");
                return ResponseHelper.InternalServerError_Request(null, "Internal server error");
            }
        }
    }
}
