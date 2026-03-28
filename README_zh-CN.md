<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI 是一个基于 [ABP](https://github.com/abpframework/abp) 框架、使用 [Radzen Blazor](https://github.com/radzenhq/radzen-blazor) 组件库构建的 Blazor Server UI 主题。

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

[English](README.md) | 简体中文

## 目录

- [在线体验](#-在线体验)
- [快速开始](#-快速开始)
- [使用数据字典模块](#-使用数据字典模块)
- [界面预览](#-界面预览)

## ❤️ 在线体验
[http://111.230.87.81:20103/](http://111.230.87.81:20103/)

用户名:  **test**

密码:  **1q2w#E***

## 🌱 快速开始

### 集成步骤

#### 1. 使用 ABP CLI 创建一个新的 Blazor Server 应用，例如项目名称为 CRM
```shell
abp new CRM -u blazor-server -dbms PostgreSQL -m none --theme leptonx-lite -csf
```

#### 2. 在 `CRM.Blazor` 项目中安装 `AbpRadzen.Blazor.Server.UI` 包
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

#### 3. 移除 `CRM.Blazor` 项目中与 `leptonx-lite` 主题相关的 NuGet 包和代码
主要包括清理 `CRMBlazorModule` 类中的相关配置，并删除 `Pages` 目录中默认生成的 Razor 页面文件。

#### 4. 配置 Abp RadzenUI
在 `ConfigService` 方法中添加 `ConfigureAbpRadzenUI`：
```csharp
private void ConfigureAbpRadzenUI()
{
    // Configure AbpRadzenUI
    Configure<AbpRadzenUIOptions>(options =>
    {
        // 这句代码非常重要。它会将当前 Blazor Web 项目中的 Razor 页面组件
        // 注册到 AbpRadzenUI 的 Router 中，从而使这些页面可以被正常访问。
        options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

        // 其他可选配置
        //options.TitleBar = new TitleBarSettings
        //{
        //    ShowLanguageMenu = false, // 是否显示多语言菜单
        //    Title = "CRM", // 标题栏名称，通常为系统名称
        //    ShowBreadcrumb = true, // 是否在标题栏显示面包屑导航（默认值：true）
        //};
        //options.LoginPage = new LoginPageSettings
        //{
        //    LogoPath = "xxx/xx.png" // 登录页 Logo 图片路径
        //};
        //options.Theme = new ThemeSettings
        //{
        //    Default = "material",
        //    EnablePremiumTheme = true,
        //};

        // 配置第三方登录服务商图标
        options.ExternalLogin.Providers.Add(new ExternalLoginProvider("AzureOpenId", "images/microsoft-logo.svg"));
    });

    // 多租户配置，这会影响登录页是否显示租户切换信息
    Configure<AbpMultiTenancyOptions>(options =>
    {
        options.IsEnabled = MultiTenancyConsts.IsEnabled;
    });

    // Configure AbpLocalizationOptions
    Configure<AbpLocalizationOptions>(options =>
    {
        // 配置多语言资源。你的资源类型需要继承 AbpRadzenUIResource，
        // 以便复用当前 UI 包内置的本地化文本。
        var crmResource = options.Resources.Get<CRMResource>();
        crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

        // 自定义语言列表
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

最后在 `OnApplicationInitialization` 方法末尾加入以下代码，以启用 RadzenUI：
```csharp
app.UseRadzenUI();
```

更多配置方式可以参考本项目示例中的 [CRMBlazorWebModule](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/CRMBlazorWebModule.cs)。

#### 5. 配置侧边栏菜单
当你新增 Razor 页面组件后，需要在 [CRMMenuContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/Menus/CRMMenuContributor.cs) 中完成菜单配置，这样页面才会显示在左侧导航菜单中。

#### 6. 配置第三方登录
如果你希望集成第三方登录，例如 Azure AD，整体配置并不复杂。只需先在配置文件中加入以下内容，再在 Web 项目的 Module 中完成认证注册，即可启用第三方登录能力。你也可以在 sample 示例中查看完整用法。
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

#### 7. 配置参数设置页面
在系统开发过程中，我们经常需要维护一些系统级或业务级参数，例如邮件服务、短信服务等。ABP 框架提供了 [Settings](https://abp.io/docs/latest/framework/infrastructure/settings?_redirected=B8ABF606AA1BDF5C629883DF1061649A) 机制，用于便捷地保存和读取这些配置。在此基础上，本 UI 组件进一步提供了统一的设置页承载能力。按照下面的步骤操作后，你自定义的配置组件会自动以 Tab 的形式出现在设置页面中：

##### （1）创建参数配置服务，例如：[AccountSettingsAppService](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Application/AccountSettingsAppService.cs)
##### （2）创建参数配置 Blazor 组件，例如：[AccountSettingComponent](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Components/Pages/Setting/AccountSettingComponent.razor)
##### （3）定义参数配置 Contributor，实现接口 `ISettingComponentContributor`。该 Contributor 负责注册你的配置组件，例如：[AccountPageContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Blazor/SettingManagement/AccountPageContributor.cs)
##### （4）最后将 Contributor 添加到 Module 配置中
```chsarp
Configure<SettingManagementComponentOptions>(options =>
{
    options.Contributors.Add(new EmailingPageContributor());
    options.Contributors.Add(new TimeZonePageContributor());
    options.Contributors.Add(new AccountPageContributor());
});
```

#### 8. 第一次运行示例程序前，请不要忘记执行数据库迁移

## 📚 使用数据字典模块

数据字典模块已集成在 UI 包中，提供开箱即用的字典类型与字典项管理页面。

### 模块说明

#### 1. 安装包
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

如果你只需要数据字典实体与 EF Core 映射能力，也可以单独安装以下包：
```shell
dotnet add package AbpRadzen.DataDictionary.EntityFrameworkCore
```

#### 2. 注册 DbContext
完整 UI 包已经在 `AbpRadzenUIModule` 中注册了 `DataDictionaryDbContext`。如果你仅在自己的项目中集成 EF Core 包，则需要在业务 DbContext 中补充 `DbSet`，并调用 `ConfigureDataDictionary()`。

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

#### 3. 菜单和多语言
数据字典页面的路由为 `/data-dictionary`。使用完整 UI 包时，菜单贡献者与多语言资源均已配置完成；如果你是在自己的应用模块中按需组合使用，请确保本地化资源继承自 `AbpRadzenUIResource`。

#### 4. 执行数据库变更
数据字典模块会创建以下两张表：

- `DataDictionaryTypes`
- `DataDictionaryItems`

接入模块后，按照常规流程创建并执行 EF Core Migration 即可。

#### 5. 使用说明

- 字典项通过 `DataDictionaryTypeId` 与字典类型关联，但 EF Core 映射不会创建数据库级外键。
- 删除字典类型时，应用服务会同步删除其下的字典项。
- 内置页面采用主从 DataGrid 布局，选中字典类型后，右侧字典项列表会自动刷新。

## 🎨 界面预览

### 1. 登录页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2. 列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3. 带筛选器的列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

### 4. 主题切换
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/switch-theme.png)

### 5. 组织机构
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/ou.png)

