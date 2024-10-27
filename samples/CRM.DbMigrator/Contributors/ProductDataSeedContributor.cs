using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using CRM.Enums;
using CRM.Products;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CRM.DbMigrator.Contributors;

internal class ProductDataSeedContributor(IRepository<Product> repository)
    : IDataSeedContributor,
        ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        foreach (var p in GetProducts())
        {
            await repository.InsertAsync(p);
        }
    }

    public static List<Product> GetProducts()
    {
        var count = 1;
        var date = DateTime.Now;
        return new Faker<Product>()
            .RuleFor(p => p.Code, f => $"A{(count++).ToString().PadLeft(4, '0')}")
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Price, f => Convert.ToSingle(f.Commerce.Price()))
            .RuleFor(p => p.StockCount, f => f.Random.Int(0, 1000))
            .RuleFor(p => p.Status, f => f.PickRandom<ProductStatus>())
            .RuleFor(p => p.CreationTime, f => date.AddSeconds(-f.Random.Int(0, 1000000)))
            .Generate(700);
    }
}
