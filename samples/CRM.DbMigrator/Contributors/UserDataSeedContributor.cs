﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace CRM.DbMigrator.Contributors;

public class UserDataSeedContributor(IIdentityDataSeeder identityDataSeeder)
    : IDataSeedContributor,
        ITransientDependency
{
    protected IIdentityDataSeeder IdentityDataSeeder { get; } = identityDataSeeder;

    public async Task SeedAsync(DataSeedContext context)
    {
        foreach (var u in GetIdentityUsers())
        {
            await IdentityDataSeeder.SeedAsync(u.Email, "1q2w3E*", null, u.UserName);
        }
    }

    public static List<UserModel> GetIdentityUsers()
    {
        return new Faker<UserModel>()
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .Generate(400);
    }
}

public class UserModel
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
}