using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

            var errorStatus = checkLoggedIn();
            if (errorStatus != null)
            {
                return errorStatus;
            }

            return Ok(new BookingRepository().getBookings(currentUser).ToArray());
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
            var errorStatus = checkLoggedIn();
            if (errorStatus != null)
            {
                return errorStatus;
            }

            BookingRepository repository = new();
            var error = ValidateItem(repository, bookingItem, id);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            BookingItem dbItem = repository.getBookingItem(id);
            if (!isAdmin() && dbItem.UserId != currentUser.Id)
            {
                return StatusCode(403, GetError("Forbidden."));
            }
            bookingItem.UserId = dbItem.Id;
            bookingItem.Rented = dbItem.Rented;
            bookingItem.Returned = dbItem.Returned;
            bookingItem.Canceled = dbItem.Canceled;

            return Ok(repository.updateBookingItem(bookingItem, id));
        }

        //TODO: cancel booking, confirm rented and returned

        private IActionResult PutBookingItemSingle(int id, BookingItem item, string singleType)
        {
            BeforeAction();
            var repository = new BookingRepository();

            BookingItem? oldItem = repository.getBookingItem(id);

            if (id != item.Id)
            {
                return BadRequest(GetError("Id error"));
            }
            
            if (singleType == "cancel")
            {
                if (oldItem.Rented == 1 || oldItem.Returned == 1)
                {
                    return BadRequest(GetError("item already taken"));
                }
                oldItem.Canceled = item.Canceled;
            }
            if (singleType == "rented") {
                if (oldItem.Canceled == 1)
                {
                    return BadRequest(GetError("booking canceled"));
                }
                oldItem.Rented = item.Rented;
            }
            if (singleType == "returned") {
                if (oldItem.Rented != 1)
                {
                    return BadRequest(GetError("rent isn't in progress"));
                }
                oldItem.Returned = item.Returned;
            }

            return Ok(repository.updateBookingItem(oldItem, id));
        }

        [HttpPut("canceled/{id}")]
        public IActionResult PutBookingItemCanceled(int id, BookingItem item)
        {
            Console.WriteLine("canceling starting");
            return PutBookingItemSingle(id, item, "cancel");
        }

        [HttpPut("rented/{id}")]
        public IActionResult PutBookingItemRented(int id, BookingItem item)
        {
            return PutBookingItemSingle(id, item, "rented");
        }

        [HttpPut("returned/{id}")]
        public IActionResult PutBookingItemReturned(int id, BookingItem item)
        {
            return PutBookingItemSingle(id, item, "returned");
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostBookingItem(BookingItem bookingItem)
        {
            BeforeAction();
            var errorStatus = checkLoggedIn();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }
             
            BookingRepository repository = new();
            var error = ValidateItem(repository, bookingItem);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }
            
            bookingItem.UserId = currentUser.Id;
            bookingItem.Rented = bookingItem.Returned = bookingItem.Canceled = 0;

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
            if(item.ResourceId == 0) { 
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
