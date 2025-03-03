using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {
        private readonly IProductRepository _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            string? brand,
            string? type,
            string? sort)
        {
            return Ok(await _repo.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _repo.AddProduct(product);

            if (await _repo.SaveChangesAsync())
            {
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            return BadRequest("Failed to create product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            _repo.UpdateProduct(product);

            if (await _repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            if (product is null)
                return NotFound();

            _repo.DeleteProduct(product);

            if (await _repo.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to delete product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await _repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await _repo.GetTypesAsync());
        }

        private bool ProductExists(int id)
        {
            return _repo.ProductExists(id);
        }
    }
}
