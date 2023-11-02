using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients;
public class CatalogClient
{
  private readonly HttpClient _client;

  public CatalogClient(HttpClient client)
  {
    _client = client;
  }

  public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
  {
    var items = await _client.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");

    return items;
  }
}
