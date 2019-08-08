using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.ViewModels;

namespace OrchardCore.Commerce.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartPersistence _shoppingCartPersistence;
        private readonly IShoppingCartHelpers _shoppingCartHelpers;
        public ShoppingCartController(
            IShoppingCartPersistence shoppingCartPersistence,
            IShoppingCartHelpers shoppingCartHelpers)
        {
            _shoppingCartPersistence = shoppingCartPersistence;
            _shoppingCartHelpers = shoppingCartHelpers;
        }

        [HttpGet]
        [Route("cart")]
        public async Task<ActionResult> Index(string shoppingCartId = null)
        {
            IList<ShoppingCartItem> cart = await _shoppingCartPersistence.Retrieve(shoppingCartId);
            // TODO: retrieve other product info
            var model = new ShoppingCartViewModel {
                Id = shoppingCartId,
                Lines = cart.Select(item => new ShoppingCartLineViewModel {
                    ProductSku = item.ProductSku
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(ShoppingCartUpdateModel cart, string shoppingCartId)
        {
            var parsedCart = await _shoppingCartHelpers.ParseCart(cart);
            await _shoppingCartPersistence.Store(parsedCart, shoppingCartId);
            return RedirectToAction("Index", new { shoppingCartId = shoppingCartId });
        }

        [HttpGet]
        public async Task<IList<ShoppingCartItem>> Get(string shoppingCartId = null)
            => await _shoppingCartPersistence.Retrieve(shoppingCartId);

        [HttpPost]
        public async Task<IList<ShoppingCartItem>> AddItem(ShoppingCartLineUpdateModel line, string shoppingCartId = null)
        {
            ShoppingCartItem parsedLine = await _shoppingCartHelpers.ParseCartLine(line);
            IList<ShoppingCartItem> cart = await _shoppingCartPersistence.Retrieve(shoppingCartId);
            ShoppingCartItem existingItem = _shoppingCartHelpers.GetExistingItem(cart, parsedLine);
            if (existingItem != null)
            {
                int index = _shoppingCartHelpers.RemoveItem(cart, existingItem);
                cart.Insert(index, new ShoppingCartItem(existingItem.Quantity + line.Quantity, existingItem.ProductSku, existingItem.Attributes));
            }
            else
            {
                cart.Add(parsedLine);
            }
            await _shoppingCartPersistence.Store(cart, shoppingCartId);
            return cart;
        }

        [HttpPost]
        public async Task<IList<ShoppingCartItem>> RemoveItem(ShoppingCartLineUpdateModel line, string shoppingCartId = null)
        {
            ShoppingCartItem parsedLine = await _shoppingCartHelpers.ParseCartLine(line);
            IList<ShoppingCartItem> cart = await _shoppingCartPersistence.Retrieve(shoppingCartId);
            _shoppingCartHelpers.RemoveItem(cart, parsedLine);
            await _shoppingCartPersistence.Store(cart, shoppingCartId);
            return cart;
        }
    }
}
