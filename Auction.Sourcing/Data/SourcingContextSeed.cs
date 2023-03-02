using System;
using System.Collections.Generic;
using Auction.Sourcing.Entities;
using MongoDB.Driver;

namespace Auction.Sourcing.Data
{
    public class SourcingContextSeed
    {

        public static void SeedData(IMongoCollection<Entities.Auction> auction)
        {
            bool exist = auction.Find(x => true).Any();
            if (!exist)
            {
                auction.InsertManyAsync(GetPreConfiguredAuctions());
            }
        }

        private IEnumerable<Entities.Auction> GetPreConfiguredAuctions()
        {
            return new List<Entities.Auction>()
            {
                new Entities.Auction()
                {
                    Name = "Auction 1",
                    Description = "Auction Desc 1",
                    CreatedAt = DateTime.Now,
                    StartedAt = DateTime.Now,
                    FinishedAt = DateTime.Now.AddDays(10),
                    ProductId = "60093337093d7352d5467341",
                    IncludedSellers = new List<string>()
                    {
                        "seller1@test.com",
                        "seller2@test.com",
                        "seller3@test.com"
                    },
                    Quantity = 5,
                    Status = (int)Status.Active
                },
                new Entities.Auction()
                {
                    Name = "Auction 2",
                    Description = "Auction Desc 2",
                    CreatedAt = DateTime.Now,
                    StartedAt = DateTime.Now,
                    FinishedAt = DateTime.Now.AddDays(10),
                    ProductId = "60093337093d7352d5467341",
                    IncludedSellers = new List<string>()
                    {
                        "seller1@test.com",
                        "seller2@test.com",
                        "seller3@test.com"
                    },
                    Quantity = 5,
                    Status = (int)Status.Active
                },
                new Entities.Auction()
                {
                    Name = "Auction 3",
                    Description = "Auction Desc 3",
                    CreatedAt = DateTime.Now,
                    StartedAt = DateTime.Now,
                    FinishedAt = DateTime.Now.AddDays(10),
                    ProductId = "60093337093d7352d5467341",
                    IncludedSellers = new List<string>()
                    {
                        "seller1@test.com",
                        "seller2@test.com",
                        "seller3@test.com"
                    },
                    Quantity = 5,
                    Status = (int)Status.Active
                }
            };
        }
    }
}
