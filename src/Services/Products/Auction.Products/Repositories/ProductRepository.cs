using System.Collections.Generic;
using System.Threading.Tasks;
using Auction.Products.Data.Interfaces;
using Auction.Products.Entities;
using Auction.Products.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace Auction.Products.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly IProductContext _context;

        public ProductRepository(IProductContext productContext)
        {
            _context = productContext;
        }

        public async Task Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }
        public async Task<bool> Delete(string id)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Id, id);
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
        public async Task<Product> GetProductAsync(string id)
        {
            return await _context.Products.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

            return await _context.Products.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductByNameAsync(string name)
        {
            var filter = Builders<Product>.Filter.ElemMatch(x => x.Name, name);

            return await _context.Products.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.Find(x => true).ToListAsync();
        }
        public async Task<bool> Update(Product product)
        {
            var updateResult = await _context.Products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
