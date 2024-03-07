using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitilizer
{
    public static async Task InitDb(WebApplication app){
        await DB.InitAsync("SearchDb", MongoClientSettings
        .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

    await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

        var count = await DB.CountAsync<Item>();

        // if (count == 0) {
        //     System.Console.WriteLine("No data found - seeding started ");
        //     var itemData = await File.ReadAllTextAsync("Data/auction.json");

        //     var opt = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

        //     var items = JsonSerializer.Deserialize<List<Item>>(itemData, opt);

        //     await DB.SaveAsync(items);
        //}

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemForSearchDb();

        System.Console.WriteLine(items.Count + " returned from the auction service");

        if(items.Count > 0) await DB.SaveAsync(items);
    }
}
