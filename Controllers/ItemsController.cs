using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DTOs;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
  [ApiController]
  // e.g. GET /items
  [Route("[controller]")] // instead of controller name you can specify your route
  public class ItemsController : ControllerBase
  {
    private readonly IItemsRepository Repository;

    public ItemsController(IItemsRepository repository)
    {
      Repository = repository;
    }

    // GET /items
    [HttpGet]
    public async Task<IEnumerable<ItemDTO>> GetAllItemsAsync()
    {
      var items = (await Repository.GetAllItemsAsync())
        .Select(item => item.AsDTO());
      return items;
    }

    // GET /items/id
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDTO>> GetItemAsync(Guid id)
    {
      var item = await Repository.GetItemAsync(id);
      if (item is null)
      {
        return NotFound();
      }

      return item.AsDTO();
    }

    // POST /items
    [HttpPost]
    public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO itemDTO)
    {
      Item item = new() { Name = itemDTO.Name, Price = itemDTO.Price };
      await Repository.CreateItemAsync(item);

      return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDTO());
    }

    // PUT /items/id
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDTO itemDTO)
    {
      var existingItem = await Repository.GetItemAsync(id);

      if (existingItem is null)
      {
        return NotFound();
      }

      Item updatedItem = existingItem with
      {
        Name = itemDTO.Name,
        Price = itemDTO.Price
      };

      await Repository.UpdateItemAsync(updatedItem);

      return NoContent();
    }

    // DELETE /items/id
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItem(Guid id)
    {
      var existingItem = await Repository.GetItemAsync(id);

      if (existingItem is null)
      {
        return NotFound();
      }
      await Repository.DeleteItemAsync(id);

      return NoContent();
    }
  }
}
