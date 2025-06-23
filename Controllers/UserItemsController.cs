using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;
using ZstdSharp.Unsafe;
using System.Text.RegularExpressions;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserItemsController : MyBaseController
    {
        // GET: api/UserItems
        [HttpGet]
        public IActionResult GetUserItems()
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            string? search = Request.Query["search"];
            
            return Ok(new UserRepository().getUsers(search).ToArray());
        }


        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public IActionResult GetUserItem(int id)
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            return Ok(new UserRepository().getUserItem(id));
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutUserItem(int id, UserItem userItem)
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            var repository = new UserRepository();

            var error = ValidateItem(repository, userItem, id);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }
            UserItem oldItem = repository.getUserItem(id);
            userItem.Password = oldItem.Password;

            return Ok(repository.updateUserItem(userItem, id));
        }
        
        [HttpPut("updateDocument/{id}")]
        public IActionResult PutUserItemDocumentId(int id, UserItem item)
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            var repository = new UserRepository();

            var oldItem = repository.getUserItem(id);

            if (id != item.Id)
            {
                return BadRequest(GetError("Id error"));
}
            try {   
                oldItem.Document_id = item.Document_id;
            }
            catch(Exception e) {
                return BadRequest(GetError(e.Message));
                //not sure this is optimal
            }

            return Ok(repository.updateUserItem(oldItem, id));
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostUserItem(UserItem userItem)
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            UserRepository repository = new();
            
            var error = ValidateItem(repository, userItem);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.addUserItem(userItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUserItem(int id)
        {
            BeforeAction();
            var errorStatus = checkAdmin();
            if ((errorStatus) != null)
            {
                return errorStatus;
            }

            return Ok(new UserRepository().deleteUserItem(id));
        }

        private string? ValidateItemGeneral(UserRepository repository, UserItem item)
        {
            if (!isAdmin())
            {
                return "Немає доступу";
            }
            if (string.IsNullOrEmpty(item.Login))
            {
                return "Логін пустий";
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return "Ім'я пусте";
            }
            if (string.IsNullOrEmpty(item.Email))
            {
                return "Е-мейл пустий";
            }

            if (!repository.validateUnique(item.Login, item.Id))
            {
                return "Цей логін вже використований";
            }

            if (!repository.validateUnique(item.Email, item.Id))
            {
                return "Цей е-мейл вже використований";
            }

            if (!Regex.IsMatch(item.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return "Неправильний е-мейл";
            }

            if (!string.IsNullOrEmpty(item.Number) && !Regex.IsMatch(item.Number, @"^\+?\d{10,15}$"))
            {
                return "Неправильний номер";
            }

            return null;
        }

        private string? ValidateItem(UserRepository repository, UserItem item)
        {
            var error = ValidateItemGeneral(repository, item);
            if (error != null)
            {
                return error;
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                return "Пароль пустий";
            }

            return null;
        }

        private string? ValidateItem(UserRepository repository, UserItem item, int id)
        {
            if (id != item.Id)
            {
                return "Помилка id";
            }

            var error = ValidateItemGeneral(repository, item);
            if (error != null)
            {
                return error;
            }

            if (repository.getUserItem(id) == null)
            {
                return "Не знайдено";
            }

            return null;
        }
    }
}
