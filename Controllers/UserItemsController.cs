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

        private string? ValidateItem(UserRepository repository, UserItem item)
        {
            if (isAdmin())
            {
                return "admin only";
            }
            if (string.IsNullOrEmpty(item.Login))
            {
                return "Login empty";
            }
            if (string.IsNullOrEmpty(item.Password))
            {
                return "Password empty";
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return "Name empty";
            }
            if (string.IsNullOrEmpty(item.Email))
            {
                return "Email empty";
            }

            if (!repository.validateUnique(item.Login, item.Id))
            {
                return "This login is already used";
            }

            if (!repository.validateUnique(item.Email, item.Id))
            {
                return "This email is already used";
            }

            return null;
        }

        private string? ValidateItem(UserRepository repository, UserItem item, int id) 
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

            if(repository.getUserItem(id) == null) 
            {
                return "Item not found";
            }

            return null;
        }
    }
}
