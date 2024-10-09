using System.Text;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace CRM.Data;

public class UserDataSeedContributor(IIdentityDataSeeder identityDataSeeder)
    : IDataSeedContributor,
        ITransientDependency
{
    protected IIdentityDataSeeder IdentityDataSeeder { get; } = identityDataSeeder;
    private static Random random = new();

    public async Task SeedAsync(DataSeedContext context)
    {
        for (int i = 0; i < 100; i++)
        {
            var userName = GenerateUsername();
            await IdentityDataSeeder.SeedAsync($"{userName}@crm.com", "1q2w3E*", null, userName);
        }
    }

    public static string GenerateUsername()
    {
        int length = random.Next(5, 11); // 随机生成长度在5到10之间
        var username = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            char letter = (char)random.Next('a', 'z' + 1); // 生成随机小写字母
            username.Append(letter);
        }

        // 可选：确保生成的用户名看起来像个名字（首字母大写）
        return char.ToUpper(username[0]) + username.ToString(1, username.Length - 1);
    }
}
