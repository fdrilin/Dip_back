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
    public class ResourceItemsController : MyBaseController
    {
        //private readonly ResourceContext _context;

        public ResourceItemsController(/*ResourceContext context*/)
        {
            //_context = context;
        }

        // GET: api/ResourceItems
        [HttpGet]
        public IEnumerable<ResourceItem> GetResourceItem()
        {
            string? search = Request.Query["search"];
            
            return new ResourceRepository().getResources(search).ToArray();
        }


        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public ResourceItem GetResourceItem(int id)
        {
            return new ResourceRepository().getResourceItem(id);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ResourceItem PutResourceItem(int id, ResourceItem resourceItem)
        {
            Console.WriteLine(resourceItem);
            return new ResourceRepository().updateResourceItem(resourceItem, id);
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult PostResourceItem(ResourceItem resourceItem)
        {
            ResourceRepository repository = new();
             //var product = _productContext.Products.Find(id);
            //return product == null ? NotFound() : Ok(product);

            Console.WriteLine(resourceItem);
            if(resourceItem.Title == "") {
                
                return BadRequest(GetError("title empty"));
            }
            if(resourceItem.Description == "") {
                
                return BadRequest(GetError("description empty"));
            }
            if(!repository.validateUnique(resourceItem.Title)){
                return BadRequest(GetError("Resource with this title already exists"));
            }

            return Ok(repository.addResourceItem(resourceItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceItem(int id)
        {
            /*var resourceItem = await _context.ResourceItem.FindAsync(id);
            if (resourceItem == null)
            {
                return NotFound();
            }*/

            new ResourceRepository().deleteResourceItem(id);

            return NoContent();
        }

        /*private bool ResouceItemExists(long id)
        {
            return _context.ResourceItem.Any(e => e.Id == id);
        }*/
    }
}
