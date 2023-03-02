using Auction.Sourcing.Data.Interface;
using Auction.Sourcing.Entities;
using Auction.Sourcing.Settings;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;

namespace Auction.Sourcing.Data
{
    public class SourcingContext : ISourcingContext
    {
        public SourcingContext(ISourcingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            Auctions = database.GetCollection<Entities.Auction>(nameof(Entities.Auction));
            Bids = database.GetCollection<Bid>(nameof(Bid));

            SourcingContextSeed.SeedData(Auctions);
        }

        public IMongoCollection<Entities.Auction> Auctions { get; }

        public IMongoCollection<Bid> Bids { get; }
    }
}
