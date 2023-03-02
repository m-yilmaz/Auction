using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Auction.Sourcing.Data.Interface;
using Auction.Sourcing.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;

namespace Auction.Sourcing.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly ISourcingContext _sourcingContext;

        public AuctionRepository(ISourcingContext sourcingContext)
        {
            _sourcingContext = sourcingContext;
        }
        public async Task Create(Entities.Auction auction)
        {
            await _sourcingContext.Auctions.InsertOneAsync(auction);
        }

        public async Task<bool> Delete(string id)
        {
            // MongoDb'ye ilgili koşulları sağlayan entity'i silmek istediğimizi, filter tanımlayıp. karşılık gelen dökümana
            // ilgili işlemi yap diyoruz.

            FilterDefinition<Entities.Auction> filterDefinition = Builders<Entities.Auction>.Filter.Eq(x => x.Id, id);
            DeleteResult deleteResult = await _sourcingContext.Auctions.DeleteOneAsync(filterDefinition);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Entities.Auction> GetAuction(string id)
        {
            return await _sourcingContext.Auctions.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Entities.Auction> GetAuctionByName(string name)
        {
            FilterDefinition<Entities.Auction> filter = Builders<Entities.Auction>.Filter.Eq(x => x.Name, name);
            return await _sourcingContext.Auctions.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Entities.Auction>> GetAuctions()
        {
            return await _sourcingContext.Auctions.Find(x => true).ToListAsync();
        }

        public async Task<bool> Update(Entities.Auction auction)
        {
            var updateResult = await _sourcingContext.Auctions.ReplaceOneAsync(x => x.Id.Equals(auction.Id), auction);
            return updateResult.IsAcknowledged && updateResult.MatchedCount > 0;
        }
    }
}
