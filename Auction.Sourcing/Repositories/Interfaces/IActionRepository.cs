using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auction.Sourcing.Repositories.Interfaces
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Entities.Auction>> GetAuctions();
        Task<Entities.Auction> GetAuction(string id);
        Task<Entities.Auction> GetAuctionByName(string name);
        Task Create(Entities.Auction auction);
        Task<bool> Update(Entities.Auction auction);
        Task<bool> Delete(string id);
    }
}
