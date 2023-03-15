using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories.Base;

namespace Ordering.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        // Solid'in liskov ve open/closed a uygun.
        Task<IEnumerable<Order>> GetOrdersBySellerUserName(string userName);
    }
}
