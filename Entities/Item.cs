using System;

namespace Catalog.Entities
{
  // Record type replaces class by adding
  // better immutable behavior and use use value-based equality.

  public record Item
  {
    // init replaces "set" and and "private set".
    // It allows to define a value only once during initialization.
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTimeOffset CreatedDate { get; init; }

    public Item()
    {
      Id = Guid.NewGuid();
      CreatedDate = DateTimeOffset.UtcNow;
    }
  }
}