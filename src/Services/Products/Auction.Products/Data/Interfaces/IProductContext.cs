using Auction.Products.Entities;
using MongoDB.Driver;

namespace Auction.Products.Data.Interfaces
{
    public interface IProductContext
    {
        // Mongo db 'nin context işlemlerini gerçekleştiricez. bunun için mongo client'e bağlanıcaz.
        // mongodb.driver ile bağlancaz.

        IMongoCollection<Product> Products { get; }

    }
}
