using System;
using System.Collections.Generic;
using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Services;

namespace OrchardCore.Commerce.Models
{
    /// <summary>
    /// Ashopping cart item
    /// </summary>
    [Serializable]
    public sealed class ShoppingCartItem : IEquatable<ShoppingCartItem>
    {
        /// <summary>
        /// Constructs a new shopping cart item
        /// </summary>
        /// <param name="quantity">The number of products</param>
        /// <param name="product">The product</param>
        public ShoppingCartItem(int quantity, string productSku, ISet<IProductAttributeValue> attributes = null)
        {
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity = quantity;
            ProductSku = productSku ?? throw new ArgumentNullException(nameof(productSku));
            Attributes = attributes ?? new HashSet<IProductAttributeValue>();
        }

        /// <summary>
        /// The number of products
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// The product SKU
        /// </summary>
        public string ProductSku { get; }

        /// <summary>
        /// The product attributes associated with this shopping cart line item
        /// </summary>
        public ISet<IProductAttributeValue> Attributes { get; }

        /// <summary>
        /// The available prices
        /// </summary>
        public IList<IPrice> Prices { get; } = new List<IPrice>();

        public override bool Equals(object obj)
            => !ReferenceEquals(null, obj)
            && (ReferenceEquals(this, obj) || Equals(obj as ShoppingCartItem));

        /// <summary>
        /// A string representation of the shopping cart item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Quantity + " x " + ProductSku
            + (Attributes.Count != 0 ? " (" + string.Join(", ", Attributes) + ")" : "");

        public bool Equals(ShoppingCartItem other)
            => other is null ? false : other.Quantity == Quantity && other.IsSameProductAs(this);

        public bool IsSameProductAs(ShoppingCartItem other)
            => ProductSku == other.ProductSku && Attributes.SetEquals(other.Attributes);

        public override int GetHashCode() => (ProductSku, Quantity, Attributes).GetHashCode();
    }
}
