using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace SpaBookingApp.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;

        public CartService(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<CartDto> AddToCart(string productIdentifiers)
        {
            // Lấy giỏ hàng hiện tại từ cache
            if (!_cache.TryGetValue("cart", out CartDto cartDto))
            {
                cartDto = new CartDto();
                cartDto.CartItems = new List<CartItemDto>();
            }

            var productDictionary = OrderHelper.GetProductDictionary(productIdentifiers);

            foreach (var pair in productDictionary)
            {
                int spaproductId = pair.Key;
                var product = await _context.SpaProducts.FindAsync(spaproductId);

                if (product == null)
                {
                    continue;
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                var existingCartItem = cartDto.CartItems.FirstOrDefault(item => item.SpaProduct.Id == spaproductId);

                if (existingCartItem != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                    existingCartItem.Quantity += pair.Value;
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                    var cartItemDto = new CartItemDto();
                    cartItemDto.SpaProduct = product;
                    cartItemDto.Quantity = pair.Value;

                    cartDto.CartItems.Add(cartItemDto);
                }
            }

            // Tính toán tổng giá trị của giỏ hàng
            // cartDto.SubTotal = cartDto.CartItems.Sum(item => item.Product.Price * item.Quantity);
            // cartDto.ShippingFee = OrderHelper.ShippingFee;
            // cartDto.TotalPrice = cartDto.SubTotal + cartDto.ShippingFee;

            // Lưu giỏ hàng mới vào cache
            _cache.Set("cart", cartDto);

            return cartDto;
        }


        public async Task<CartDto> GetCart()
        {
            // Lấy giỏ hàng từ cache
            if (_cache.TryGetValue("cart", out CartDto cartDto))
            {
                // Tính toán tổng giá trị của giỏ hàng
                cartDto.SubTotal = cartDto.CartItems.Sum(item => item.SpaProduct.Price * item.Quantity);
                cartDto.ShippingFee = OrderHelper.ShippingFee;
                cartDto.TotalPrice = cartDto.SubTotal + cartDto.ShippingFee;

                return cartDto;
            }

            // Nếu không có giỏ hàng trong cache, trả về giỏ hàng rỗng
            return new CartDto();
        }

        public async Task<CartDto> UpdateCart(string productIdentifiers)
        {
            if (!_cache.TryGetValue("cart", out CartDto cartDto))
            {
                cartDto = new CartDto();
            }

            var productDictionary = OrderHelper.GetProductDictionary(productIdentifiers);

            // Xoá các sản phẩm không có trong danh sách productIdentifiers ra khỏi giỏ hàng
            var productIdsInCart = cartDto.CartItems.Select(item => item.SpaProduct.Id).ToList();
            var productIdsToUpdate = productDictionary.Keys.ToList();
            var productsToRemove = productIdsInCart.Except(productIdsToUpdate).ToList();
            cartDto.CartItems.RemoveAll(item => productsToRemove.Contains(item.SpaProduct.Id));

            foreach (var pair in productDictionary)
            {
                int spaproductId = pair.Key;
                var product = await _context.SpaProducts.FindAsync(spaproductId);

                if (product == null)
                {
                    continue;
                }

                var existingCartItem = cartDto.CartItems.FirstOrDefault(item => item.SpaProduct.Id == spaproductId);

                if (existingCartItem != null)
                {
                    if (pair.Value > 0)
                    {
                        existingCartItem.Quantity = pair.Value;
                    }
                    else
                    {
                        cartDto.CartItems.Remove(existingCartItem);
                    }
                }
                else
                {
                    if (pair.Value > 0)
                    {
                        var cartItemDto = new CartItemDto();
                        cartItemDto.SpaProduct = product;
                        cartItemDto.Quantity = pair.Value;

                        cartDto.CartItems.Add(cartItemDto);
                    }
                }
            }

            // Xóa các sản phẩm có số lượng <= 0 khỏi giỏ hàng
            cartDto.CartItems.RemoveAll(item => item.Quantity <= 0);

            _cache.Set("cart", cartDto);

            return await Task.FromResult(cartDto);
        }


        public async Task<CartDto> ClearCart()
        {
            // Xóa giỏ hàng trong cache
            _cache.Remove("cart");

            return new CartDto();
        }

    }
}
