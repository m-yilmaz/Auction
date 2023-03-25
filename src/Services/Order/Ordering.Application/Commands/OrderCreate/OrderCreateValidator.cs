using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Ordering.Application.Commands.OrderCreate
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateCommand>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.SellerUserName)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.ProductId)
                .NotEmpty();
        }
    }
}
