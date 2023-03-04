using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Auction.Sourcing.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auction.Sourcing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(IAuctionRepository auctionRepository, ILogger<AuctionController> logger)
        {
            _auctionRepository = auctionRepository;
            _logger = logger;
        }

        // Status 200 olduğunda döneceğimiz veri tipini belirttik. Onun haricinde başka bir şey dönmeyeceğim diyorum.
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Entities.Auction>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Entities.Auction>>> GetAuctions()
        {
            var auctions = await _auctionRepository.GetAuctions();
            return Ok(auctions);
        }

        [HttpGet("{id:length(24)}", Name = "GetAuction")]
        [ProducesResponseType(typeof(Entities.Auction), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Entities.Auction>> GetAction(string id)
        {
            var auction = await _auctionRepository.GetAuction(id);

            if (auction == null)
            {
                _logger.LogError($"Auction with id: {id} hasn't been found in database");
                return NotFound();
            }
            return Ok(auction);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Entities.Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entities.Auction>> CreateAuction([FromBody] Entities.Auction auction)
        {
            await _auctionRepository.Create(auction);

            return CreatedAtRoute("GetAuction", new { id = auction.Id }, auction);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Entities.Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entities.Auction>> UpdateAuction([FromBody] Entities.Auction auction)
        {
            return Ok(await _auctionRepository.Update(auction));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Entities.Auction), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entities.Auction>> DeleteAuction(string id)
        {
            return Ok(await _auctionRepository.Delete(id));
        }
    }
}
