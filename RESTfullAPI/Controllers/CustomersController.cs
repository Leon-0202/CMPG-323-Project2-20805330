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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly project2sqldbContext _context;

        public CustomersController(project2sqldbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            var customers = await _context.Customers.ToListAsync();

            if (customers == null || !customers.Any())
            {
                return NotFound();
            }

            // Create a list of CustomerDTO objects for response
            var customerDTOs = customers.Select(customer => new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                CustomerTitle = customer.CustomerTitle,
                CustomerName = customer.CustomerName,
                CustomerSurname = customer.CustomerSurname,
                CellPhone = customer.CellPhone
            }).ToList();

            return customerDTOs;
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(short id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Create a CustomerDTO object for response
            var customerDTO = new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                CustomerTitle = customer.CustomerTitle,
                CustomerName = customer.CustomerName,
                CustomerSurname = customer.CustomerSurname,
                CellPhone = customer.CellPhone
            };

            return customerDTO;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(short id, CustomerDTO customerDTO)
        {
            if (id != customerDTO.CustomerId)
            {
                return BadRequest();
            }

            if (!CustomerExists(id))
            {
                return NotFound();
            }

            // Update customer properties and save changes
            var customer = await _context.Customers.FindAsync(id);
            customer.CustomerTitle = customerDTO.CustomerTitle;
            customer.CustomerName = customerDTO.CustomerName;
            customer.CustomerSurname = customerDTO.CustomerSurname;
            customer.CellPhone = customerDTO.CellPhone;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            var customer = new Customer
            {
                CustomerId = customerDTO.CustomerId,
                CustomerTitle = customerDTO.CustomerTitle,
                CustomerName = customerDTO.CustomerName,
                CustomerSurname = customerDTO.CustomerSurname,
                CellPhone = customerDTO.CellPhone
            };

            _context.Customers.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }

            // Create the corresponding DTO for the response
            var createdCustomerDTO = new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                CustomerTitle = customerDTO.CustomerTitle,
                CustomerName = customer.CustomerName,
                CustomerSurname = customer.CustomerSurname,
                CellPhone = customer.CellPhone
            };

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, createdCustomerDTO);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(short id)
        {
            if (!CustomerExists(id))
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PATCH: api/Customers/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCustomer(short id, JsonPatchDocument<Customer> patchDocument)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(customer, ModelState);

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
                if (!CustomerExists(id))
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

        private bool CustomerExists(short id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
