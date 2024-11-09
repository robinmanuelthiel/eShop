using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using eShop.Catalog.API.Infrastructure;
using eShop.Catalog.API.Model;
using eShop.Catalog.API.IntegrationEvents;
using eShop.EventBus.Events;
using eShop.Catalog.API.Services;
using eShop.Catalog.API.IntegrationEvents.Events;

namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog").HasApiVersion(1.0);

        // Routes for querying catalog items.
        api.MapGet("/items", GetAllItems);
        api.MapGet("/items/by", GetItemsByIds);
        api.MapGet("/items/{id:int}", GetItemById);
        api.MapGet("/items/by/{name:minlength(1)}", GetItemsByName);
        api.MapGet("/items/{catalogItemId:int}/pic", GetItemPictureById);

        // Routes for resolving catalog items using AI.
        api.MapGet("/items/withsemanticrelevance/{text:minlength(1)}", GetItemsBySemanticRelevance);

        // Routes for resolving catalog items by type and brand.
        api.MapGet("/items/type/{typeId}/brand/{brandId?}", GetItemsByBrandAndTypeId);
        api.MapGet("/items/type/all/brand/{brandId:int?}", GetItemsByBrandId);
        api.MapGet("/catalogtypes", GetAllCatalogTypes);
        api.MapGet("/catalogbrands", GetAllCatalogBrands);

        // Routes for modifying catalog items.
        api.MapPut("/items", UpdateItem);
        api.MapPost("/items", CreateItem);
        api.MapDelete("/items/{id:int}", DeleteItemById);

        return app;
    }

    public static async Task<Results<Ok<PaginatedItems<CatalogItem>>, BadRequest<string>>> GetAllItems(
        [AsParameters] PaginationRequest paginationRequest,
        [FromServices] CatalogContext context,
        [FromServices] ICatalogAI catalogAI,
        [FromServices] ILogger<CatalogItem> logger)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await context.CatalogItems
            .LongCountAsync();

        var itemsOnPage = await context.CatalogItems
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds(
        [FromServices] CatalogContext context,
        int[] ids)
    {
        var items = await context.CatalogItems.Where(item => ids.Contains(item.Id)).ToListAsync();
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<CatalogItem>, NotFound, BadRequest<string>>> GetItemById(
        [FromServices] CatalogContext context,
        int id)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var item = await context.CatalogItems.Include(ci => ci.CatalogBrand).SingleOrDefaultAsync(ci => ci.Id == id);

        if (item == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(item);
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] PaginationRequest paginationRequest,
        [FromServices] CatalogContext context,
        string name)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await context.CatalogItems
            .Where(c => c.Name.StartsWith(name))
            .LongCountAsync();

        var itemsOnPage = await context.CatalogItems
            .Where(c => c.Name.StartsWith(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Results<NotFound, PhysicalFileHttpResult>> GetItemPictureById(
        [FromServices] CatalogContext context,
        [FromServices] IWebHostEnvironment environment,
        int catalogItemId)
    {
        var item = await context.CatalogItems.FindAsync(catalogItemId);


        if (item is null)
        {
            return TypedResults.NotFound();
        }

        var path = GetFullPath(environment.ContentRootPath, item.PictureFileName);

        string imageFileExtension = Path.GetExtension(item.PictureFileName);
        string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);
        DateTime lastModified = File.GetLastWriteTimeUtc(path);

        return TypedResults.PhysicalFile(path, mimetype, lastModified: lastModified);
    }

    public static async Task<Results<BadRequest<string>, RedirectToRouteHttpResult, Ok<PaginatedItems<CatalogItem>>>> GetItemsBySemanticRelevance(
        [AsParameters] PaginationRequest paginationRequest,
        [FromServices] CatalogContext context,
        [FromServices] ICatalogAI catalogAI,
        [FromServices] ILogger<CatalogItem> logger,
        string text)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        if (!catalogAI.IsEnabled)
        {
            logger.LogInformation("AI services are disabled. Using text-based search instead.");
            return await GetItemsByName(paginationRequest, context, text);
        }

        // Create an embedding for the input search
        var vector = await catalogAI.GetEmbeddingAsync(text);
        if (vector is null)
        {
            logger.LogWarning("Failed to generate embedding for search text. Falling back to text search.");
            return await GetItemsByName(paginationRequest, context, text);
        }

        // Get the total number of items with valid embeddings
        var totalItems = await context.CatalogItems
            .Where(c => c.Embedding != null)
            .LongCountAsync();

        if (totalItems == 0)
        {
            logger.LogWarning("No items with embeddings found. Falling back to text search.");
            return await GetItemsByName(paginationRequest, context, text);
        }

        // Get the next page of items, ordered by most similar (smallest distance) to the input search
        List<CatalogItem> itemsOnPage;
        if (logger.IsEnabled(LogLevel.Debug))
        {
            var itemsWithDistance = await context.CatalogItems
                .Where(c => c.Embedding != null)  // Only consider items with embeddings
                .Select(c => new {
                    Item = c,
                    Distance = c.Embedding!.CosineDistance(vector!) // We can use ! here because we filtered nulls above
                })
                .OrderBy(c => c.Distance)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            logger.LogDebug("Results from {text}: {results}", text, string.Join(", ", itemsWithDistance.Select(i => $"{i.Item.Name} => {i.Distance}")));

            itemsOnPage = itemsWithDistance.Select(i => i.Item).ToList();
        }
        else
        {
            itemsOnPage = await context.CatalogItems
                .Where(c => c.Embedding != null)  // Only consider items with embeddings
                .OrderBy(c => c.Embedding!.CosineDistance(vector!)) // We can use ! here because we filtered nulls above
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
        }

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [FromServices] CatalogContext context,
        int typeId,
        int? brandId)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var root = (IQueryable<CatalogItem>)context.CatalogItems;
        root = root.Where(c => c.CatalogTypeId == typeId);
        if (brandId is not null)
        {
            root = root.Where(c => c.CatalogBrandId == brandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandId(
        [AsParameters] PaginationRequest paginationRequest,
        [FromServices] CatalogContext context,
        int? brandId)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var root = (IQueryable<CatalogItem>)context.CatalogItems;

        if (brandId is not null)
        {
            root = root.Where(ci => ci.CatalogBrandId == brandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Results<Created, NotFound<string>>> UpdateItem(
        [FromServices] CatalogContext context,
        [FromServices] ICatalogAI catalogAI,
        [FromServices] ICatalogIntegrationEventService eventService,
        [FromBody] CatalogItem productToUpdate)
    {
        var catalogItem = await context.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

        if (catalogItem == null)
        {
            return TypedResults.NotFound($"Item with id {productToUpdate.Id} not found.");
        }

        // Update current product
        var catalogEntry = context.Entry(catalogItem);
        catalogEntry.CurrentValues.SetValues(productToUpdate);

        catalogItem.Embedding = await catalogAI.GetEmbeddingAsync(catalogItem);

        var priceEntry = catalogEntry.Property(i => i.Price);

        if (priceEntry.IsModified) // Save product's data and publish integration event through the Event Bus if price has changed
        {
            //Create Integration Event to be published through the Event Bus
            var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, priceEntry.OriginalValue);

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            await eventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

            // Publish through the Event Bus and mark the saved event as published
            await eventService.PublishThroughEventBusAsync(priceChangedEvent);
        }
        else // Just save the updated product because the Product's Price hasn't changed.
        {
            await context.SaveChangesAsync();
        }
        return TypedResults.Created($"/api/catalog/items/{productToUpdate.Id}");
    }

    public static async Task<Results<Created, BadRequest<string>>> CreateItem(
        [FromServices] CatalogContext context,
        [FromServices] ICatalogAI catalogAI,
        [FromServices] ILogger<CatalogItem> logger,
        [FromBody] CatalogItem product)
    {
        // Validate brand and type existence
        var brand = await context.CatalogBrands.FindAsync(product.CatalogBrandId);
        var type = await context.CatalogTypes.FindAsync(product.CatalogTypeId);

        if (brand is null)
        {
            return TypedResults.BadRequest($"CatalogBrand with id {product.CatalogBrandId} not found.");
        }

        if (type is null)
        {
            return TypedResults.BadRequest($"CatalogType with id {product.CatalogTypeId} not found.");
        }

        var item = new CatalogItem
        {
            Id = product.Id,
            CatalogBrandId = product.CatalogBrandId,
            CatalogTypeId = product.CatalogTypeId,
            CatalogBrand = brand,  // Set required navigation property
            CatalogType = type,    // Set required navigation property
            Description = product.Description,
            Name = product.Name,
            PictureFileName = product.PictureFileName,
            Price = product.Price,
            AvailableStock = product.AvailableStock,
            RestockThreshold = product.RestockThreshold,
            MaxStockThreshold = product.MaxStockThreshold
        };

        // Only try to get embedding if AI service is enabled
        if (catalogAI.IsEnabled)
        {
            item.Embedding = await catalogAI.GetEmbeddingAsync(item);
            if (item.Embedding is null)
            {
                logger.LogWarning("Failed to generate embedding for new catalog item {ItemName}", item.Name);
            }
        }

        context.CatalogItems.Add(item);
        await context.SaveChangesAsync();

        return TypedResults.Created($"/api/catalog/items/{item.Id}");
    }

    public static async Task<Results<NoContent, NotFound>> DeleteItemById(
        [FromServices] CatalogContext context,
        int id)
    {
        var item = context.CatalogItems.SingleOrDefault(x => x.Id == id);

        if (item is null)
        {
            return TypedResults.NotFound();
        }

        context.CatalogItems.Remove(item);
        await context.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<Ok<List<CatalogType>>> GetAllCatalogTypes(
        [FromServices] CatalogContext context)
    {
        var types = await context.CatalogTypes
            .OrderBy(x => x.Type)
            .ToListAsync();
        return TypedResults.Ok(types);
    }

    public static async Task<Ok<List<CatalogBrand>>> GetAllCatalogBrands(
        [FromServices] CatalogContext context)
    {
        var brands = await context.CatalogBrands
            .OrderBy(x => x.Brand)
            .ToListAsync();
        return TypedResults.Ok(brands);
    }

    private static string GetImageMimeTypeFromImageFileExtension(string extension) => extension switch
    {
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".bmp" => "image/bmp",
        ".tiff" => "image/tiff",
        ".wmf" => "image/wmf",
        ".jp2" => "image/jp2",
        ".svg" => "image/svg+xml",
        ".webp" => "image/webp",
        _ => "application/octet-stream",
    };

    public static string GetFullPath(string contentRootPath, string pictureFileName) =>
        Path.Combine(contentRootPath, "Pics", pictureFileName);
}
