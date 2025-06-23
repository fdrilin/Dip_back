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
using System.Text.Json;

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
            
            string? tags = Request.Query["tags"];
            string? search = Request.Query["search"];
            Console.WriteLine(search);

            return Ok(new BookingRepository().getBookings(currentUser, tags, search).ToArray());
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
            bookingItem.ResourceId = dbItem.ResourceId;
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
                    return BadRequest(GetError("Предмет вже підтверджений"));
                }
                Console.WriteLine("admin:" + isAdmin());
                if (oldItem.Canceled == 1 && !isAdmin())
                {
                    return BadRequest(GetError("Не можливо відмінити відміну"));
                }
                oldItem.Canceled = item.Canceled;
            }
            if (singleType == "rented") {
                if (oldItem.Canceled == 1)
                {
                    return BadRequest(GetError("Бронювання відмінено"));
                }
                oldItem.Rented = item.Rented;
            }
            if (singleType == "returned") {
                if (oldItem.Rented != 1)
                {
                    return BadRequest(GetError("Бронювання не підверджено"));
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
            BookingRepository repository = new();
            var errorStatus = checkLoggedIn();
            if (errorStatus != null)
            {
                return errorStatus;
            }

            var error = ValidateItem(repository, bookingItem);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            Console.WriteLine(bookingItem.ResourceTypeId);
            ResourceItem? resourceItem = new ResourceRepository().getAvailableResource(bookingItem.BeginDate, bookingItem.EndDate, bookingItem.ResourceTypeId);
            if (resourceItem == null)
            {
                return BadRequest(GetError("Немає доступних ресурсів"));
            }
            bookingItem.ResourceId = resourceItem.Id;

            bookingItem.UserId = currentUser.Id;
            bookingItem.Rented = bookingItem.Returned = bookingItem.Canceled = 0;

            Console.WriteLine(JsonSerializer.Serialize(resourceItem));
            Console.WriteLine(JsonSerializer.Serialize(bookingItem));

            return Ok(repository.addBookingItem(bookingItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBookingItem(int id)
        {
            BeforeAction();
            return Ok(new BookingRepository().deleteBookingItem(id));
        }

        private string? ValidateItemGeneral(BookingRepository repository, BookingItem item, int excludeId = 0) 
        {
            //if(item.ResourceId == 0) { return "Resource not defined";}
            string[] beginDateArrayStr = item.BeginDate.Split(" ")[0].Split("-");
            string[] endDateArrayStr = item.EndDate.Split(" ")[0].Split("-");
            if(beginDateArrayStr[0] == "") { 
                return "Рік початку незаповнений";
            }
            if(beginDateArrayStr[1] == "") { 
                return "Місяць початку незаповнений";
            }
            if(beginDateArrayStr[2] == "") { 
                return "День початку незаповнений";
            }
            if(endDateArrayStr[0] == "") { 
                return "Рік кінця незаповнений";
            }
            if(endDateArrayStr[1] == "") { 
                return "Місяць кінця незаповнений";
            }
            if(endDateArrayStr[2] == "") { 
                return "День кінця незаповнений";
            }

            List<int> beginDateArray = new();
            foreach (var s in beginDateArrayStr) 
            {
                if (int.TryParse(s, out int value))
                {
                    beginDateArray.Add(value);
                }
                else 
                {
                    return "Помилка дати";
                }
            }
            List<int> endDateArray = new();
            foreach (var s in endDateArrayStr) 
            {
                if (int.TryParse(s, out int value))
                {
                    endDateArray.Add(value);
                }
                else 
                {
                    return "Помилка дати";
                }
            }

            if(!DateTime.TryParse(string.Join("-", beginDateArray), out DateTime beginDate))
            {
                return "Неможлива дата початку";
            }
            if(!DateTime.TryParse(string.Join("-", endDateArray), out DateTime endDate))
            {
                return "Неможлива дата кінця";
            }

            if (endDate < beginDate)
            {
                return "Початкова дата пізнаше кінцевої";
            }

            return null;
        }

        private string? ValidateItem(BookingRepository repository, BookingItem item) 
        {
            var error = ValidateItemGeneral(repository, item);
            if (error != null) 
            {
                return error;
            }
            
            if (DateTime.Now > DateTime.Parse(item.BeginDate))
            {
                return "Дата початку занадто рання";
            }

            if (item.ResourceTypeId == 0)
            {
                return "Тип ресурсу не вибрано";
            }

            return null;
        }

        private string? ValidateItem(BookingRepository repository, BookingItem item, int id) 
        {
            if (id != item.Id)
            {
                return "Помилка id";
            }

            var error = ValidateItemGeneral(repository, item, id);
            if (error != null) 
            {
                return error;
            }
            
            if (
                DateTime.Now > DateTime.Parse(item.BeginDate)
                && DateTime.Parse(repository.getBookingItem(item.Id).BeginDate) > DateTime.Parse(item.BeginDate) 
            )
            {
                return "Дата початку занадто рання";
            }

            if (repository.getBookingItem(id) == null)
            {
                return "Не знайдено";
            }

            if (new ResourceRepository().isAvailableSpecific(DateTime.Parse(item.BeginDate).ToString("yyyy-MM-dd"), DateTime.Parse(item.EndDate).ToString("yyyy-MM-dd"), item.Id, item.ResourceId))
            {
                return "Бронювання на ций час вже існує";
            }

            return null;
        }
    }
}
