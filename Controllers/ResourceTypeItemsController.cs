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
    public class ResourceTypeItemsController : MyBaseController
    {
        //private readonly ResourceContext _context;

        public ResourceTypeItemsController()
        {
        }

        [HttpGet]
        public IActionResult GetResourceTypeItems()
        {
            BeforeAction();

            string? search = Request.Query["search"];
            string? tags = Request.Query["tags"];

            return Ok(new ResourceTypeRepository().getResourceTypes(search, tags, isAdmin()).ToArray());
        }

        [HttpGet("{id}")]
        public IActionResult GetResourceTypeItem(int id)
        {
            BeforeAction();
            return Ok(new ResourceTypeRepository().getResourceTypeItem(id));
        }

        [HttpPut("{id}")]
        public IActionResult PutResourceTypeItem(int id, ResourceTypeItem item)
        {
            BeforeAction();
            var repository = new ResourceTypeRepository();

            var error = ValidateItem(repository, item, id);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.updateResourceTypeItem(item, id));
        }

        [HttpPost]
        public IActionResult PostResourceTypeItem(ResourceTypeItem item)
        {
            BeforeAction();
            ResourceTypeRepository repository = new();

            var error = ValidateItem(repository, item);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.addResourceTypeItem(item));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteResourceTypeItem(int id)
        {
            BeforeAction();
            return Ok(new ResourceTypeRepository().deleteResourceTypeItem(id));
        }

        private string? ValidateItem(ResourceTypeRepository repository, ResourceTypeItem item) 
        {
            BeforeAction();
            if(string.IsNullOrEmpty(item.Title)) {
                return "title empty";
            }
            if(string.IsNullOrEmpty(item.Description)) { 
                return "description empty";
            }
            if (!repository.validateUnique(item.Title, item.Id))
            {
                return "Resource type with this title already exists";
            }

            return null;
        }

        private string? ValidateItem(ResourceTypeRepository repository, ResourceTypeItem item, int id) 
        {
            BeforeAction();
            if (id != item.Id)
            {
                return "Id error";
            }

            var error = ValidateItem(repository, item);
            if (error != null) 
            {
                return error;
            }

            if(repository.getResourceTypeItem(id) == null) 
            {
                return "Item not found";
            }

            return null;
        }
    }
}
