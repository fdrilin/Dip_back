using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;
using ZstdSharp.Unsafe;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingItemsController : MyBaseController
    {

        public BookingItemsController(){}

        // GET: api/ResourceItems
        [HttpGet]
        public IEnumerable<BookingItem> GetBookingItem()
        {
            return (new BookingRepository().getBookings().ToArray());
        }

/*
        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceItem>> GetResourceItem(long id)
        {
            var resourceItem = await _context.ResourceItem.FindAsync(id);

            if (resourceItem == null)
            {
                return NotFound();
            }

            return resourceItem;
        }*/

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookingItem(int id, BookingItem bookingItem)
        {
            if (id != bookingItem.Id)
            {
                return BadRequest();
            }

            Console.WriteLine(bookingItem);
            new BookingRepository().updateBookingItem(bookingItem, id);

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public BookingItem PostBookingItem(BookingItem BookingItem)
        {
            Console.WriteLine(BookingItem);
            return new BookingRepository().addBookingItem(BookingItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingItem(int id)
        {
            new BookingRepository().deleteBookingItem(id);

            return NoContent();
        }
    }
}
