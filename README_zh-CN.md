<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI 是使用[Radzen Blazor](https://github.com/radzenhq/radzen-blazor)组件库开发的基于[ABP](https://github.com/abpframework/abp)框架的Blazor Server模式的UI主题.

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

[English](README.md) | 简体中文

## ❤️体验地址
[http://111.230.87.81:20103/](http://111.230.87.81:20103/)

用户名:  **test**

密码:  **1q2w#E***

## 🎨部分页面展示

### 1.登录页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2.列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3.带有Filter的列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

### 4.其他主题
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/switch-theme.png)

### 5.组织机构
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/ou.png)

## 🌱如何集成

### 1. 使用ABP CLI工具创建一个新的Abp Blazor Server应用，例如项目名称叫CRM
```shell
abp new CRM -u blazor-server -dbms PostgreSQL -m none --theme leptonx-lite -csf
```

### 2. 在 `CRM.Blazor` 项目安装`AbpRadzen.Blazor.Server.UI`包
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

### 3. 移除`CRM.Blazor`项目中与`leptonx-lite`主题相关的nuget包和代码
主要是 `CRMBlazorModule` 类中的代码，删除`Pages`目录中自带的razor页面文件

### 4. 对 Abp RadzenUI 进行配置
将 `ConfigureAbpRadzenUI` 方法添加到`ConfigService`方法中
```csharp
private void ConfigureAbpRadzenUI()
{
    // Configure AbpRadzenUI
    Configure<AbpRadzenUIOptions>(options =>
    {
        // 这句代码很重要，它会将你在Blazor Web项目中新建的razor页面组件添加到Router中，这样就可以访问到了
        options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

        // 配置页面标题栏
        //options.TitleBar = new TitleBarSettings
        //{
        //    ShowLanguageMenu = false, // 是否显示多语言按钮菜单
        //    Title = "CRM" // 标题栏名称：一般是系统名称
        //};
        //options.LoginPage = new LoginPageSettings
        //{
        //    LogoPath = "xxx/xx.png" // 登录页面的logo图片
        //};
        //options.Theme = new ThemeSettings
        //{
        //    Default = "material",
        //    EnablePremiumTheme = true,
        //};

        // 配置第三方登录服务商icon
        options.ExternalLogin.Providers.Add(new ExternalLoginProvider("AzureOpenId", "images/microsoft-logo.svg"));
    });

    // 多租户配置, 这个会影响到登录页面是否展示租户信息
    Configure<AbpMultiTenancyOptions>(options =>
    {
        options.IsEnabled = MultiTenancyConsts.IsEnabled;
    });

    // Configure AbpLocalizationOptions
    Configure<AbpLocalizationOptions>(options =>
    {
        // 配置多语言资源，需要继承AbpRadzenUIResource，它包含了需要用到的多语言信息
        var crmResource = options.Resources.Get<CRMResource>();
        crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

        // 配置多语言菜单中显示的语言
        options.Languages.Clear();
        options.Languages.Add(new LanguageInfo("en", "en", "English"));
        options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
        options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
    });

    // 配置侧边栏菜单
    Configure<AbpNavigationOptions>(options =>
    {
        options.MenuContributors.Add(new CRMMenuContributor());
    });
}
```

最后在`OnApplicationInitialization`方法的最后添加以下代码，使用RadzenUI
```csharp
app.UseRadzenUI();
```

关于更多的配置可以参考本项目的示例代码：[CRMBlazorWebModule](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/CRMBlazorWebModule.cs)

### 5. 配置侧边栏菜单
当你添加了新的razor页面组件后，需要在[CRMMenuContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/Menus/CRMMenuContributor.cs)类文件中进行配置，这样它就会显示在页面的侧边栏菜单中

### 6. 配置第三方登录
如果你想集成第三方登录，比如AzureAD，配置很简单，只要你在配置文件中做以下配置，然后在你的Web项目的Module中进行以下配置，你就能使用第三方登录功能了。你能在sample示例中找到如何使用。
```json
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "<your-tenant-id>",
  "ClientId": "<your-client-id>",
  "ClientSecret": "<your-client-secret>",
  "CallbackPath": "/signin-azuread-oidc"
}
```

```csharp
private void ConfigureOidcAuthentication(
    ServiceConfigurationContext context,
    IConfiguration configuration
)
{
    if (configuration.GetSection("AzureAd").Exists())
    {
        context
            .Services.AddAuthentication()
            .AddOpenIdConnect(
                "AzureOpenId",
                "Azure Active Directory",
                options =>
                {
                    options.Authority =
                        $"{configuration["AzureAd:Instance"]}{configuration["AzureAd:TenantId"]}/v2.0/";
                    options.ClientId = configuration["AzureAd:ClientId"];
                    options.ClientSecret = configuration["AzureAd:ClientSecret"];
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.CallbackPath = configuration["AzureAd:CallbackPath"];
                    options.RequireHttpsMetadata = false;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignInScheme = IdentityConstants.ExternalScheme;

                    options.Scope.Add("email");
                }
            );
    }
}
```

### 7. 配置参数设置页面
在系统开发过程中，我们经常需要进行一些系统参数或业务参数的配置，例如邮箱服务商、短信服务商等等，通常会开发对应页面进行参数的配置，Abp框架提供了[Settings](https://abp.io/docs/latest/framework/infrastructure/settings?_redirected=B8ABF606AA1BDF5C629883DF1061649A)可以方便对参数进行保存设置，在此基础上，本UI组件可以方便的对这些配置建立页面统一管理，遵循以下步骤将自动将你创建的配置组件添加到设置页面的tab项中：
#### （1）创建你的参数配置服务，例如：[AccountSettingsAppService](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Application/AccountSettingsAppService.cs)
#### （2）创建你的参数配置blazor组件，例如：[AccountSettingComponent](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Components/Pages/Setting/AccountSettingComponent.razor)
#### （3）定义你的参数配置Contributor，实现接口ISettingComponentContributor，该Contributor主要是添加你的参数配置blazor组件，例如：[AccountPageContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Blazor/SettingManagement/AccountPageContributor.cs)
#### （4）最后将你的Contributor添加到Module配置中
```chsarp
Configure<SettingManagementComponentOptions>(options =>
{
    options.Contributors.Add(new EmailingPageContributor());
    options.Contributors.Add(new TimeZonePageContributor());
    options.Contributors.Add(new AccountPageContributor());
});
```

### 8. 第一次运行示例程序的时候不要忘了执行迁移代码

## 📚数据字典模块

数据字典模块已经集成在 UI 包中，提供了开箱即用的字典类型和字典项维护页面。

### 1. 安装包
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

如果你只需要数据字典实体和 EF Core 映射，也可以单独安装下面这个包：
```shell
dotnet add package AbpRadzen.DataDictionary.EntityFrameworkCore
```

### 2. 注册 DbContext
完整 UI 包已经在 `AbpRadzenUIModule` 中注册了 `DataDictionaryDbContext`。如果你只集成 EF Core 包到自己的项目中，需要在业务 DbContext 中添加 `DbSet` 并调用 `ConfigureDataDictionary()`。

```csharp
public class MyProjectDbContext : AbpDbContext<MyProjectDbContext>
{
    public DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureDataDictionary();
    }
}
```

### 3. 菜单和多语言
数据字典页面路由为 `/data-dictionary`。使用完整 UI 包时，菜单贡献者和多语言资源已经配置完成；如果你是在自己的应用模块中组合使用，请确保你的本地化资源继承了 `AbpRadzenUIResource`。

### 4. 执行数据库变更
数据字典模块会创建两张表：

- `DataDictionaryTypes`
- `DataDictionaryItems`

接入模块后，按常规方式创建并执行 EF Core Migration 即可。

### 5. 使用说明

- 字典项通过 `DataDictionaryTypeId` 关联字典类型，但 EF Core 映射不会创建数据库外键。
- 删除字典类型时，会在应用服务中同步删除其下的字典项。
- 内置页面采用主从 DataGrid 布局，选中字典类型后会自动刷新右侧字典项列表。

