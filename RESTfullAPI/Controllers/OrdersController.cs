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
    public class OrdersController : ControllerBase
    {
        private readonly project2sqldbContext _context;

        public OrdersController(project2sqldbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            // Create a list of OrderDTO objects for response
            var orderDTOs = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                DeliveryAddress = order.DeliveryAddress
        }).ToList();

            return orderDTOs;
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(short id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Create an OrderDTO object for response
            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                DeliveryAddress = order.DeliveryAddress
            };

            return orderDTO;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(short id, OrderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            if (_context.Customers == null || _context.Products == null)
            {
                return Problem("Entity sets are null.");
            }

            var customer = await _context.Customers.FindAsync(orderDTO.CustomerId);
            if (customer == null)
            {
                return NotFound($"Customer with ID {orderDTO.CustomerId} not found.");
            }

            // Update existing order properties
            existingOrder.OrderDate = orderDTO.OrderDate;
            existingOrder.CustomerId = orderDTO.CustomerId;
            existingOrder.DeliveryAddress = orderDTO.DeliveryAddress;

            // Remove existing order details
            var existingOrderDetails = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            _context.OrderDetails.RemoveRange(existingOrderDetails);

            // Add updated order details
            foreach (var orderDetailDTO in orderDTO.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetailDTO.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {orderDetailDTO.ProductId} not found.");
                }

                var orderDetail = new OrderDetail
                {
                    OrderId = id,
                    OrderDetailsId = orderDetailDTO.OrderId,
                    ProductId = orderDetailDTO.ProductId,
                    Quantity = orderDetailDTO.Quantity,
                    Discount = orderDetailDTO.Discount
                };

                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDTO orderDTO)
        {
            if (_context.Customers == null || _context.Products == null)
            {
                return Problem("Entity sets are null.");
            }

            var customer = await _context.Customers.FindAsync(orderDTO.CustomerId);
            if (customer == null)
            {
                return NotFound($"Customer with ID {orderDTO.CustomerId} not found.");
            }

            var order = new Order
            {
                OrderId = orderDTO.OrderId,
                OrderDate = orderDTO.OrderDate,
                CustomerId = orderDTO.CustomerId,
                DeliveryAddress = orderDTO.DeliveryAddress
            };

            foreach (var orderDetailDTO in orderDTO.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetailDTO.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {orderDetailDTO.ProductId} not found.");
                }

                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    OrderDetailsId = orderDetailDTO.OrderDetailsId,
                    ProductId = orderDetailDTO.ProductId,
                    Quantity = orderDetailDTO.Quantity,
                    Discount = orderDetailDTO.Discount
                };

                _context.OrderDetails.Add(orderDetail);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(short id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            // Delete related OrderDetails first
            var relatedOrderDetails = _context.OrderDetails.Where(od => od.OrderId == id);
            _context.OrderDetails.RemoveRange(relatedOrderDetails);

            // Remove the order itself
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PATCH: api/Orders/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchOrder(short id, JsonPatchDocument<Order> patchDocument)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(order, ModelState);

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
                if (!OrderExists(id))
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

        private bool OrderExists(short id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
