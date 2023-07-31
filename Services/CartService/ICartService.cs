using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.CartService
{
    public interface ICartService
    {
        Task<CartDto> GetCart(string productIdentifiers);
    }
}