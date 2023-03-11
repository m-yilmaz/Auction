using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Auction.Sourcing.Entities;
using Auction.Sourcing.Repositories.Interfaces;
using AutoMapper;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auction.Sourcing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;

        private readonly EventBusRabbitMQProducer _eventBus;

        private readonly IMapper _mapper;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(
            IAuctionRepository auctionRepository,
            IBidRepository bidRepository,
            EventBusRabbitMQProducer eventBus,
            IMapper mapper,
            ILogger<AuctionController> logger)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
            _eventBus = eventBus;
            _mapper = mapper;
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


        [HttpPost("CompleteAuction")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<ActionResult> CompleteAuction(string id)
        {
            Entities.Auction auction = await _auctionRepository.GetAuction(id);
            if (auction == null)
                return NotFound();

            if (auction.Status != (int)Status.Active)
            {
                _logger.LogError("Auction can not be completed");
                return BadRequest();
            }

            Bid bid = await _bidRepository.GetWinnerBid(id);
            if (bid == null)
                return NotFound();

            OrderCreateEvent eventMessage = _mapper.Map<OrderCreateEvent>(bid);
            eventMessage.Quantity = auction.Quantity;

            auction.Status = (int)Status.Closed;

            bool updateResponse = await _auctionRepository.Update(auction);

            if (!updateResponse)
            {
                _logger.LogError("Auction can not updated");
                return BadRequest();
            }

            try
            {
                _eventBus.Publish(EventBusConstants.OrderCreateQueue, eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {EventId} from {AppName}", eventMessage.Id, "Sourcing");
                throw;
            }

            return Accepted();
        }

        [HttpPost("TestEvent")]
        public ActionResult<OrderCreateEvent> TestEvent()
        {
            OrderCreateEvent eventMessage = new()
            {
                Quantity = 100,
                AuctionId = "dummy1",
                SellerUserName = "test@test.com",
                Price = 10,
                ProductId = "dummy_product_1"
            };

            try
            {
                _eventBus.Publish(EventBusConstants.OrderCreateQueue, eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {EventId} from {AppName}", eventMessage.Id, "Sourcing");
                throw;
            }

            return Accepted(eventMessage);
        }
    }
}
