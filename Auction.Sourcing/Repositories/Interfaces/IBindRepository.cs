﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.Sourcing.Entities;

namespace Auction.Sourcing.Repositories.Interfaces
{
    public interface IBidRepository
    {
        Task SendBid(Bid bid);

        Task<List<Bid>> GetBidsByAuctionId(string id);

        Task<Bid> GetWinnerBid(string id);

    }
}
