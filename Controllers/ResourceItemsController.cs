using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;

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
            return (new ResourceRepository().getResources().ToArray());
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
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResourceItem(long id, ResourceItem resourceItem)
        {
            if (id != resourceItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(resourceItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResourceItem>> PostResourceItem(ResourceItem resourceItem)
        {
            //uncomment when connecting to mysql
            //_context.TodoItem.Add(todoItem);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            Console.WriteLine(resourceItem);
            return CreatedAtAction(nameof(Get), new { id = resourceItem.Id }, resourceItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResourceItem(long id)
        {
            var resourceItem = await _context.ResourceItem.FindAsync(id);
            if (resourceItem == null)
            {
                return NotFound();
            }

            _context.TodoItem.Remove(resourceItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResouceItemExists(long id)
        {
            return _context.ResourceItem.Any(e => e.Id == id);
        }*/
    }
}
