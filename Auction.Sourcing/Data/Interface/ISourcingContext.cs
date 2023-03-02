using Auction.Sourcing.Entities;
using MongoDB.Driver;

namespace Auction.Sourcing.Data.Interface
{
    public interface ISourcingContext
    {
        public IMongoCollection<Entities.Auction> Auctions { get; }
        public IMongoCollection<Bid> Bids { get; }
    }
}
