using Domain.Entities;
using Domain.Entities.Orders;

namespace Domain.UnitTests.Entities.Orders;

public class OrderSdsRequirementsTests
{
    [Fact]
    public void GetProductsRequiringSafetyDataSheets_WhenRequiredProductExists_ReturnsProduct()
    {
        var requiredProduct = CreateProduct(1, requiresSds: true);
        var order = CreateOrderWithItems(CreateItem(1, requiredProduct));

        var result = order.GetProductsRequiringSafetyDataSheets();

        Assert.Equal(new[] { requiredProduct }, result);
    }

    [Fact]
    public void GetProductsRequiringSafetyDataSheets_WhenItemIsDeleted_IgnoresProduct()
    {
        var requiredProduct = CreateProduct(1, requiresSds: true);
        var deletedItem = CreateItem(1, requiredProduct);
        deletedItem.DeletedAt = DateTime.UtcNow;
        var order = CreateOrderWithItems(deletedItem);

        var result = order.GetProductsRequiringSafetyDataSheets();

        Assert.Empty(result);
    }

    [Fact]
    public void GetProductsRequiringSafetyDataSheets_WhenProductAppearsMoreThanOnce_ReturnsProductOnce()
    {
        var requiredProduct = CreateProduct(1, requiresSds: true);
        var order = CreateOrderWithItems(
            CreateItem(1, requiredProduct),
            CreateItem(2, requiredProduct));

        var result = order.GetProductsRequiringSafetyDataSheets();

        Assert.Equal(new[] { requiredProduct }, result);
    }

    [Fact]
    public void GetProductsRequiringSafetyDataSheets_WhenProductsDoNotRequireSds_ReturnsEmpty()
    {
        var product = CreateProduct(1, requiresSds: false);
        var order = CreateOrderWithItems(CreateItem(1, product));

        var result = order.GetProductsRequiringSafetyDataSheets();

        Assert.Empty(result);
    }

    private static Order CreateOrderWithItems(params OrderItem[] items)
    {
        var order = new Order();

        foreach (var item in items)
        {
            order.OrderItems.Add(item);
        }

        return order;
    }

    private static OrderItem CreateItem(int orderItemId, Product product)
    {
        return new OrderItem
        {
            OrderItemId = orderItemId,
            ProductId = product.ProductId,
            Product = product
        };
    }

    private static Product CreateProduct(int productId, bool requiresSds)
    {
        return new Product
        {
            ProductId = productId,
            RequiresSds = requiresSds
        };
    }
}
