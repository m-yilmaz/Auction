using Auction.Sourcing.Entities;
using AutoMapper;
using EventBusRabbitMQ.Events;

namespace Auction.Sourcing.Mapping
{
    public class SourcingMapping : Profile
    {
        public SourcingMapping()
        {
            CreateMap<OrderCreateEvent, Bid>().ReverseMap();
        }
    }
}
