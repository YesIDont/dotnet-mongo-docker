using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Entities;

namespace Catalog.Repositories
{
  public class InMemoryItemsRepository : IItemsRepository
  {
    // new() replaces the old syntax: new List<Item>, because
    // we already now the type and typing that was redundant
    private readonly List<Item> items = new()
    {
      new Item() { Name = "Potion", Price = 50 },
      new Item() { Name = "Iron Sword", Price = 20 },
      new Item() { Name = "Bronze Shield", Price = 10 },
    };

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
      return await Task.FromResult(items);
    }

    public async Task<Item> GetItemAsync(Guid id)
    {
      var item = items.Where(items => items.Id == id).SingleOrDefault();
      return await Task.FromResult(item);
    }

    public async Task CreateItemAsync(Item item)
    {
      items.Add(item);
      await Task.CompletedTask;
    }

    public async Task UpdateItemAsync(Item editedItem)
    {
      var index = items.FindIndex(item => item.Id == editedItem.Id);
      items[index] = editedItem;
      await Task.CompletedTask;
    }

    public async Task DeleteItemAsync(Guid id)
    {
      var index = items.FindIndex(item => item.Id == id);
      items.RemoveAt(index);
      await Task.CompletedTask;
    }
  }
}