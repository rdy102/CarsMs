using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        var acution = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if(context.Message.ItemSold)
        {
            acution.Winner = context.Message.Winner;
            acution.SoldAmount = (int)context.Message.Amount;
        }

        acution.Status = "Finished";

        await acution.SaveAsync();
    }
}
