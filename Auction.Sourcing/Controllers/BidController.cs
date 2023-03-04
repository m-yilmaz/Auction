using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Auction.Sourcing.Entities;
using Auction.Sourcing.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;

namespace Auction.Sourcing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BidController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;
        private readonly ILogger<BidController> _logger;

        public BidController(IBidRepository bidRepository, ILogger<BidController> logger)
        {
            _bidRepository = bidRepository;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendBid([FromBody] Bid bid)
        {
            await _bidRepository.SendBid(bid);

            return Ok();
        }

        [HttpGet("GetBidByAuctionId")]
        [ProducesResponseType(typeof(IEnumerable<Bid>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Bid>>> GetBidByAuctionId(string id)
        {
            IEnumerable<Bid> bids = await _bidRepository.GetBidsByAuctionId(id);

            return Ok();
        }

        [HttpGet("GetWinnerBid")]
        [ProducesResponseType(typeof(Bid), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Bid>> GetWinnerBid(string id)
        {
            Bid bid = await _bidRepository.GetWinnerBid(id);

            return Ok(bid);
        }

    }
}
