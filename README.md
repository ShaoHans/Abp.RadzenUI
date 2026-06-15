<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI is a Blazor Server UI theme built on top of the [ABP](https://github.com/abpframework/abp) framework and crafted with the [Radzen Blazor](https://github.com/radzenhq/radzen-blazor) component library.

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

English | [简体中文](README_zh-CN.md)

## Contents

- [Try the Demo](#-try-the-demo)
- [Get Started](#-get-started)
- [Use the dotnet new Template](#use-the-dotnet-new-template)
- [Use the Linked Accounts Module](#-use-the-linked-accounts-module)
- [Use the Data Dictionary Module](#-use-the-data-dictionary-module)
- [Use the Messages Module](#-use-the-messages-module)
- [Preview the Interface](#-preview-the-interface)

## ❤️ Try the Demo
[http://111.230.87.81:20103/](http://111.230.87.81:20103/)

UserName:  **test**

Password:  **1q2w#E***

## 🌱 Get Started

### Published Packages

- `AbpRadzen.Blazor.Server.UI`: the full Blazor Server UI theme package with built-in pages, menus, localization, and host-side integration.
- `AbpRadzen.LinkAccounts`: the standalone linked-accounts application package.
- `AbpRadzen.EntityFrameworkCore`: the unified EF Core package for the built-in Data Dictionary and Messages modules.
- `AbpRadzen.Application.Contracts`, `AbpRadzen.Application`, `AbpRadzen.Domain`, `AbpRadzen.Domain.Shared`: layered building blocks for custom composition outside the full UI package.

### Use the dotnet new Template

This repository includes a `dotnet new` item template that generates the common integration files for an existing ABP Blazor Server web project, including the RadzenUI integration helper, menu contributor, minimal home page, and an integration checklist.

Install the template from the repository root:

```shell
dotnet new install .\templates\AbpRadzenUI.Integration
```

Go to your Blazor Server web project directory, for example `src\MyCompany.MyProject.Blazor`, and run:

```shell
dotnet new abp-radzenui-integration -n MyProject --rootNamespace MyCompany.MyProject.Blazor --title "My Project"
```

To enable Radzen premium themes:

```shell
dotnet new abp-radzenui-integration -n MyProject --rootNamespace MyCompany.MyProject.Blazor --title "My Project" --premiumTheme true
```

Parameters:

- `-n`: the short project name used for generated C# type names. Avoid dots.
- `--rootNamespace`: the real root namespace of the target Blazor Server web project.
- `--title`: the system title shown in the title bar and login page.
- `--premiumTheme`: enables Radzen premium themes.

The template generates:

- `RadzenUI/<ProjectName>RadzenUIIntegrationExtensions.cs`
- `Menus/<ProjectName>Menus.cs`
- `Menus/<ProjectName>MenuContributor.cs`
- `Components/Pages/Home.razor`
- `RadzenUI/RADZENUI-INTEGRATION.md`

After generation, complete the manual integration checklist:

```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

Add the module dependency to your web module:

```csharp
typeof(AbpRadzenUIModule)
```

Call the generated extension method in `ConfigureServices`:

```csharp
context.Services.AddRadzenUIIntegration<Home, MyProjectResource, MyProjectMenuContributor>();
```

Finally, call RadzenUI at the end of `OnApplicationInitialization`:

```csharp
app.UseRadzenUI();
```

See [docs/getting-started-template.md](docs/getting-started-template.md) for the full guide.

### Integration Steps

#### 1. Create a new solution with the ABP CLI
```shell
abp new CRM -u blazor-server -dbms PostgreSQL -m none --theme leptonx-lite -csf
```

#### 2. Install `AbpRadzen.Blazor.Server.UI` in your `CRM.Blazor` project
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

#### 3. Remove the NuGet packages and code related to the `leptonx-lite` theme
In practice, this mainly means cleaning up the code in `CRMBlazorModule` and removing the default Razor pages under the `Pages` directory.

#### 4. Configure Abp RadzenUI
Add a `ConfigureAbpRadzenUI` method inside your `ConfigService` method:
```csharp
private void ConfigureAbpRadzenUI()
{
    // Configure AbpRadzenUI
    Configure<AbpRadzenUIOptions>(options =>
    {
        // This is important. It registers the Razor pages from your current web application
        // so that they can be discovered by the router inside the AbpRadzenUI module.
        options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

        // Other optional settings
        //options.TitleBar = new TitleBarSettings
        //{
        //    ShowLanguageMenu = false,
        //    Title = "CRM",
        //    ShowBreadcrumb = true, // Show breadcrumb navigation in the title bar (default: true)
        //};
        //options.LoginPage = new LoginPageSettings
        //{
        //    LogoPath = "xxx/xx.png"
        //};
        //options.Theme = new ThemeSettings
        //{
        //    Default = "material",
        //    EnablePremiumTheme = true,
        //};

        // Configure the icon for external login providers
        options.ExternalLogin.Providers.Add(new ExternalLoginProvider("AzureOpenId", "images/microsoft-logo.svg"));
    });

    // Configure AbpMultiTenancyOptions. This controls whether tenant switching
    // is shown on the login page.
    Configure<AbpMultiTenancyOptions>(options =>
    {
        options.IsEnabled = MultiTenancyConsts.IsEnabled;
    });

    // Configure AbpLocalizationOptions
    Configure<AbpLocalizationOptions>(options =>
    {
        // Inherit from AbpRadzenUIResource so your application can reuse
        // the built-in localization texts provided by this UI package.
        var crmResource = options.Resources.Get<CRMResource>();
        crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

        // If you prefer a custom language list, clear the default set and add your own.
        options.Languages.Clear();
        options.Languages.Add(new LanguageInfo("en", "en", "English"));
        options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
        options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
    });

    // Configure your application's navigation menu
    Configure<AbpNavigationOptions>(options =>
    {
        options.MenuContributors.Add(new CRMMenuContributor());
    });
}
```

Then add the following line at the end of your `OnApplicationInitialization` method:
```csharp
app.UseRadzenUI();
```

For a complete example, refer to [CRMBlazorWebModule](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/CRMBlazorWebModule.cs).

#### 5. Configure the menu
Whenever you add a new Razor page and want it to appear in the sidebar, update the [CRMMenuContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/Menus/CRMMenuContributor.cs) class accordingly.

#### 6. Configure external login
If you want to integrate a third-party identity provider such as Azure AD, the setup is straightforward. Add the following configuration to your settings file, then register the authentication handler in your web module. You can also refer to the sample project for a working implementation.
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
#### 7. Configure the settings page
In real-world projects, it is common to manage system-level or business-level settings such as email providers, SMS providers, and similar options. ABP provides a convenient [Settings](https://abp.io/docs/latest/framework/infrastructure/settings?_redirected=B8ABF606AA1BDF5C629883DF1061649A) infrastructure for persisting and retrieving these values. Based on that mechanism, this UI package makes it easy to surface configuration components as tabs on a unified settings page.

Follow the steps below to add your own settings component:

##### (1) Create an application service for your settings, for example: [AccountSettingsAppService](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.RadzenUI.Application/AccountSettingsAppService.cs)

##### (2) Create a Blazor component for the settings UI, for example: [AccountSettingComponent](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Components/Pages/Setting/AccountSettingComponent.razor)

##### (3) Create a contributor by implementing `ISettingComponentContributor`. The contributor is responsible for registering your settings component, for example: [AccountPageContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Features/Settings/AccountPageContributor.cs)

##### (4) Finally, register the contributor in your module configuration

```csharp
Configure<SettingManagementComponentOptions>(options =>
{
    options.Contributors.Add(new EmailingPageContributor());
    options.Contributors.Add(new TimeZonePageContributor());
    options.Contributors.Add(new AccountPageContributor());
});
```

#### 8. Apply database migrations before the first run

## 🔗 Use the Linked Accounts Module

The Linked Accounts capability is now built into the full UI package and can also be consumed as a standalone application-layer package.

### What it provides

- Link another local or cross-tenant account from the current signed-in session.
- Switch between directly linked and indirectly reachable accounts without logging out manually.
- Keep a reversible session stack so the user can return to the previous account.
- Reuse ABP's built-in `IdentityLinkUser` and `IdentityLinkUserManager` instead of introducing a custom link table.

### Use it through the full UI package

If you install `AbpRadzen.Blazor.Server.UI`, the Linked Accounts pages, localization texts, controllers, and menu entry are already wired into the theme module. No extra package installation is required.

The built-in management page route is `/account/linked-accounts`.

### Use it as a standalone package

If you only need the linking and switching services in your own module, install the standalone package below:

```shell
dotnet add package AbpRadzen.LinkAccounts
```

Then depend on the module in your application module:

```csharp
[DependsOn(typeof(AbpRadzenUILinkAccountsModule))]
public class MyApplicationModule : AbpModule
{
}
```

The standalone package registers the following services for you:

- `ILinkedAccountAppService`
- `ILinkedAccountFlowStateStore`

### Notes about the authentication flow

- The first link operation still requires a real login for the target account.
- After the relationship is established, account switching relies on the existing link relationship plus a short-lived flow token to re-issue the authentication ticket.
- Flow tokens and linked-account sessions are stored in distributed cache and are shared in the host tenant scope so cross-tenant switching can complete correctly.

## 🧱 Shared EF Core Integration

The Data Dictionary and Messages modules now share the same standalone EF Core package and the same DbContext configuration entry.

### Install the package

If you use the full UI package, no extra installation is required:

```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

If you only need the entity definitions and EF Core mappings for these built-in modules, install the unified package below:

```shell
dotnet add package AbpRadzen.EntityFrameworkCore
```

### Register the DbContext

The full UI package already depends on `AbpRadzen.EntityFrameworkCore` and wires the unified EF module for you. If you integrate only the EF Core package into your own solution, add the required DbSets and call `ConfigureAbpRadzenUI()` inside your application's DbContext.

```csharp
public class MyProjectDbContext : AbpDbContext<MyProjectDbContext>
{
    public DbSet<DataDictionaryType> DataDictionaryTypes { get; set; }

    public DbSet<DataDictionaryItem> DataDictionaryItems { get; set; }

    public DbSet<UserMessage> UserMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAbpRadzenUI();
    }
}
```

## 📚 Use the Data Dictionary Module

The data dictionary module is included in the UI package and provides an out-of-the-box page for managing dictionary types and dictionary items.

### Module Guide

#### 1. Add localization and menu support
The built-in page route is `/data-dictionary`. When you use the full UI package, menu contributions and localization resources are already wired up. If you are composing your own application module, make sure your localization resource inherits from `AbpRadzenUIResource`.

#### 2. Apply database changes
The data dictionary module creates the following two tables:

- `DataDictionaryTypes`
- `DataDictionaryItems`

After integrating the module, create and apply your EF Core migrations in the usual way.

#### 3. Usage notes

- Dictionary items are associated with dictionary types through `DataDictionaryTypeId`, but the EF Core mapping does not create a database-level foreign key.
- Deleting a dictionary type will also remove its child dictionary items at the application-service level.
- The built-in page uses a master-detail DataGrid layout. Selecting a dictionary type automatically refreshes the item grid on the right.

## ✉️ Use the Messages Module

The messages module is included in the full UI package and provides a built-in in-app messaging experience with a header unread badge, a right-side inbox panel, a message center list page, and rich-text message details.

### Module Guide

#### 1. Built-in UI capabilities

- The built-in message center page route is `/messages`.
- The header includes an unread badge and a right-side inbox sidebar for quick access.
- Users can mark a single message or all current messages as read, and open full message details from the sidebar or the list page.
- The message center page supports filtering by title, read status, and message type.
- Message content supports HTML rendering in the detail view.

#### 2. Apply database changes
The message module creates a `UserMessages` table and related indexes for tenant, user, read status, message type, and creation time. After integrating the module, create and apply your EF Core migrations in the usual way.

#### 3. Usage notes

- Messages are isolated by current tenant and current user.
- Opening a message detail marks the message as read automatically.
- `MessageType` is modeled as a string instead of an enum so consuming applications can extend it without changing the shared module.
- The message type dropdown is provided through an overridable lookup service, so applications can replace the available options if needed.

## 🎨 Preview the Interface

### 1. Login page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2. List page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3. List page with DataGrid filters
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

### 4. Theme switching
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/switch-theme.png)

### 5. Organization units
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/ou.png)
