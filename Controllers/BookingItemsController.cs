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
        public IActionResult GetBookingItems()
        {
            //TODO: make so user can only see his, admin all, guest 403
            BeforeAction();
            return Ok(new BookingRepository().getBookings().ToArray());
        }

        [HttpGet("{id}")]
        public IActionResult GetBookingItem(int id)
        {
            BeforeAction();
            return Ok(new BookingRepository().getBookingItem(id));
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutBookingItem(int id, BookingItem bookingItem)
        {
            BeforeAction();
            BookingRepository repository = new();
            var error = ValidateItem(repository, bookingItem, id);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.updateBookingItem(bookingItem, id));
        }

        //TODO: cancel booking, confirm rented and returned

        private IActionResult PutBookingItemSingle(int id, BookingItem item, string singleType)
        {
            BeforeAction();
            var repository = new BookingRepository();

            var oldItem = repository.getBookingItem(id);

            if (id != item.Id)
            {
                return BadRequest(GetError("Id error"));
            }
            try {
                if (singleType == "cancel") {
                    oldItem.Canceled = item.Canceled;
                }
                if (singleType == "rented") {
                    oldItem.Rented = item.Rented;
                }
                if (singleType == "returned") {
                    oldItem.Returned = item.Returned;
                }
            }
            catch(Exception e) {
                return BadRequest(GetError(e.Message));
                //not sure this is optimal
            }

            return Ok(repository.updateBookingItem(oldItem, id));
        }

        [HttpPut("cancel/{id}")]
        public IActionResult PutBookingItemCanceled(int id, BookingItem item)
        {
            BeforeAction();
            return PutBookingItemSingle(id, item, "cancel");
        }

        [HttpPut("rented/{id}")]
        public IActionResult PutBookingItemRented(int id, BookingItem item)
        {
            BeforeAction();
            return PutBookingItemSingle(id, item, "rented");
        }

        [HttpPut("returned/{id}")]
        public IActionResult PutBookingItemReturned(int id, BookingItem item)
        {
            BeforeAction();
            return PutBookingItemSingle(id, item, "returned");
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostBookingItem(BookingItem bookingItem)
        {
            BeforeAction();
            BookingRepository repository = new();
            var error = ValidateItem(repository, bookingItem);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.addBookingItem(bookingItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBookingItem(int id)
        {
            BeforeAction();
            return Ok(new BookingRepository().deleteBookingItem(id));
        }

        private string? ValidateItem(BookingRepository repository, BookingItem item) 
        {
            if(item.UserId == null) {
                return "User not defined";
            }
            if(item.ResourceId == null) { 
                return "Resource not defined";
            }
            if(string.IsNullOrEmpty(item.BeginDate)) { 
                return "Begin date empty";
            }
            if(string.IsNullOrEmpty(item.EndDate)) { 
                return "End date empty";
            }

            return null;
        }

        private string? ValidateItem(BookingRepository repository, BookingItem item, int id) 
        {
            if (id != item.Id)
            {
                return "Id error";
            }

            var error = ValidateItem(repository, item);
            if (error != null) 
            {
                return error;
            }

            if(repository.getBookingItem(id) == null) 
            {
                return "Item not found";
            }

            return null;
        }
    }
}
