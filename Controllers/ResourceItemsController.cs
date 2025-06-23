using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceItemsController : MyBaseController
    {
        //private readonly ResourceContext _context;

        public ResourceItemsController(/*ResourceContext context*/)
        {
            //_context = context;
        }

        [HttpGet]
        public IActionResult GetResourceItems()
        {
            BeforeAction();

            string? search = Request.Query["search"];
            int? resourceTypeId = int.TryParse(Request.Query["resource_type_id"], out var id) ? id : (int?)null;

            return Ok(
                new ResourceRepository().getResources(
                    search,
                    isAdmin(),
                    resourceTypeId
                ).ToArray()
            );
        }

        [HttpGet("{id}")]
        public IActionResult GetResourceItem(int id)
        {
            BeforeAction();
            return Ok(new ResourceRepository().getResourceItem(id));
        }

        [HttpPut("{id}")]
        public IActionResult PutResourceItem(int id, ResourceItem resourceItem)
        {
            BeforeAction();
            var repository = new ResourceRepository();

            var error = ValidateItem(repository, resourceItem, id);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.updateResourceItem(resourceItem, id));
        }

        [HttpPut("available/{id}")]
        public IActionResult PutResourceItemAvailable(int id, ResourceItem item)
        {
            BeforeAction();
            var repository = new ResourceRepository();

            var oldItem = repository.getResourceItem(id);

            if (id != item.Id)
            {
                return BadRequest(GetError("Id error"));
            }
            try {
                oldItem.Available = item.Available;
            }
            catch(Exception e) {
                return BadRequest(GetError(e.Message));
                //not sure this is optimal
            }

            return Ok(repository.updateResourceItem(oldItem, id));
        }

        [HttpPost]
        public IActionResult PostResourceItem(ResourceItem resourceItem)
        {
            BeforeAction();
            ResourceRepository repository = new();

            var error = ValidateItem(repository, resourceItem);
            if (error != null) 
            {
                return BadRequest(GetError(error));
            }

            return Ok(repository.addResourceItem(resourceItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public IActionResult DeleteResourceItem(int id)
        {
            BeforeAction();
            return Ok(new ResourceRepository().deleteResourceItem(id));
        }

        private string? ValidateItem(ResourceRepository repository, ResourceItem item) 
        {
            if (!isAdmin())
            {
                return "Немає доступу";
            }
            if (string.IsNullOrEmpty(item.SerialNo))
            {
                return "Серійний номер не заповнено";
            }
            Console.WriteLine(item.ResourceTypeId);
            if (item.ResourceTypeId == 0 || item.ResourceTypeId == null) {
                return "Ресурс тип не заповнено";
            }

            if (!repository.validateUnique(item.SerialNo, item.Id))
            {
                return "Техніка з таким серійним номер вже існує";
            }

            return null;
        }

        private string? ValidateItem(ResourceRepository repository, ResourceItem resourceItem, int id) 
        {
            if (id != resourceItem.Id)
            {
                return "Помилка id";
            }

            var error = ValidateItem(repository, resourceItem);
            if (error != null) 
            {
                return error;
            }

            if(repository.getResourceItem(id) == null) 
            {
                return "Не знайдено";
            }

            return null;
        }
    }
}
