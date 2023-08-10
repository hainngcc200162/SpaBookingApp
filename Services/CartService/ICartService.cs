using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.CartService
{
    public interface ICartService
    {
        Task<CartDto> GetCart();
        Task<CartDto> AddToCart(string productIdentifiers);
        Task<CartDto> UpdateCart(string productIdentifiers);
        Task<CartDto> ClearCart();
    }
}