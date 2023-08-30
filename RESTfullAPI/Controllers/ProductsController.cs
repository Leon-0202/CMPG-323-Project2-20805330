using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfullAPI.Data_Transfer_Objects;
using RESTfullAPI.Models;

namespace RESTfullAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly project2sqldbContext _context;

        public ProductsController(project2sqldbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            // Create a list of ProductDTO objects for response
            var productDTOs = products.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                UnitsInStock = product.UnitsInStock
            }).ToList();

            return productDTOs;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(short id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Create a ProductDTO object for response
            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                UnitsInStock = product.UnitsInStock
            };

            return productDTO;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(short id, ProductDTO productDTO)
        {
            if (id != productDTO.ProductId)
            {
                return BadRequest();
            }

            if (!ProductExists(id))
            {
                return NotFound();
            }

            // Retrieve the existing product entity
            var product = await _context.Products.FindAsync(id);

            // Update the product properties from the DTO
            product.ProductName = productDTO.ProductName;
            product.ProductDescription = productDTO.ProductDescription;
            product.UnitsInStock = productDTO.UnitsInStock;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO productDTO)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'project2sqldbContext.Products' is null.");
            }

            // Create a Product entity from the DTO
            var product = new Product
            {
                ProductId = productDTO.ProductId,
                ProductName = productDTO.ProductName,
                ProductDescription = productDTO.ProductDescription,
                UnitsInStock = productDTO.UnitsInStock
            };

            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Create a ProductDTO object for the response
            var createdProductDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                UnitsInStock = product.UnitsInStock
            };

            return CreatedAtAction("GetProduct", new { id = createdProductDTO.ProductId }, createdProductDTO);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(short id)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PATCH: api/Products/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProduct(short id, JsonPatchDocument<Product> patchDocument)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(product, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //GET Products related to an Order
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsForOrder(short orderId)
        {
            var productsForOrder = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Select(od => od.Product)
                .ToListAsync();

            if (productsForOrder == null || !productsForOrder.Any())
            {
                return NotFound();
            }

            // Create a list of ProductDTO objects for response
            var productDTOs = productsForOrder.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                UnitsInStock = product.UnitsInStock
            }).ToList();

            return productDTOs;
        }

        private bool ProductExists(short id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
