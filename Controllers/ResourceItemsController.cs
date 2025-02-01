using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ResourceItemsController : ControllerBase
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
            return new ResourceRepository().getResources().ToArray();
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
        public ResourceItem PostResourceItem(ResourceItem resourceItem)
        {
            Console.WriteLine(resourceItem);
            return new ResourceRepository().addResourceItem(resourceItem);
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
