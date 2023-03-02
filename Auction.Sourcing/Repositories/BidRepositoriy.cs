using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Auction.Sourcing.Data.Interface;
using Auction.Sourcing.Entities;
using Auction.Sourcing.Repositories.Interfaces;
using MongoDB.Driver;

namespace Auction.Sourcing.Repositories
{
    public class BidRepositoriy : IBidRepository
    {
        private readonly ISourcingContext _context;

        public BidRepositoriy(ISourcingContext context)
        {
            _context = context;
        }
        public async Task<List<Bid>> GetBidsByAuctionId(string id)
        {
            FilterDefinition<Bid> filter = Builders<Bid>.Filter.Eq(x => x.AuctionId, id);

            List<Bid> bids = await _context.Bids.Find(filter).ToListAsync();

            bids = bids.OrderByDescending(x => x.CreatedAt)
                       .GroupBy(x => x.SellerUserName)
                       .Select(x => new Bid()
                       {
                           AuctionId = x.FirstOrDefault().AuctionId,
                           Price = x.FirstOrDefault().Price,
                           CreatedAt = x.FirstOrDefault().CreatedAt,
                           SellerUserName = x.FirstOrDefault().SellerUserName,
                           ProductId = x.FirstOrDefault().ProductId,
                           Id = x.FirstOrDefault().Id
                       }).ToList();

            return bids;
        }

        public async Task<Bid> GetWinnerBid(string id)
        {
            List<Bid> bids = await GetBidsByAuctionId(id);

            return bids.OrderByDescending(x => x.Price).FirstOrDefault();
        }

        public async Task SendBid(Bid bid)
        {
            await _context.Bids.InsertOneAsync(bid);
        }
    }
}
