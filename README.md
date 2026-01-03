<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI is a UI theme built on the [Abp](https://github.com/abpframework/abp) framework and developed using the [Radzen Blazor](https://github.com/radzenhq/radzen-blazor) component.

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

English | [简体中文](README_zh-CN.md)

## ❤️Demo Site
[http://111.230.87.81:20103/](http://111.230.87.81:20103/)

UserName:  **test**

Password:  **1q2w#E***

## 🎨Page display

### 1.The login page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2.The list page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3.The other list page with datagrid filter
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

### 4.Theme switch
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/switch-theme.png)

### 5.Organization units
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/ou.png)

## 🌱How to use

### 1. Create new solution by abp cli
```shell
abp new CRM -u blazor-server -dbms PostgreSQL -m none --theme leptonx-lite -csf
```

### 2. Install `AbpRadzen.Blazor.Server.UI` on your `CRM.Blazor` project
```shell
dotnet add package AbpRadzen.Blazor.Server.UI
```

### 3. Remove the nuget packages and code associated with the leptonx-lite theme
This is mainly the code in the `CRMBlazorModule` class and delete files in the Pages directory

### 4. Config Abp RadzenUI
Add the `ConfigureAbpRadzenUI` method on your `ConfigService` method
```csharp
private void ConfigureAbpRadzenUI()
{
    // Configure AbpRadzenUI
    Configure<AbpRadzenUIOptions>(options =>
    {
        // this is very imporant to set current web application's pages to the AbpRadzenUI module
        options.RouterAdditionalAssemblies = [typeof(Home).Assembly];

        // other settings
        //options.TitleBar = new TitleBarSettings
        //{
        //    ShowLanguageMenu = false,
        //    Title = "CRM"
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

        // configure external login provider icon
        options.ExternalLogin.Providers.Add(new ExternalLoginProvider("AzureOpenId", "images/microsoft-logo.svg"));
    });

    // Configure AbpMultiTenancyOptions, this will affect login page that whether need to switch tenants
    Configure<AbpMultiTenancyOptions>(options =>
    {
        options.IsEnabled = MultiTenancyConsts.IsEnabled;
    });

    // Configure AbpLocalizationOptions
    Configure<AbpLocalizationOptions>(options =>
    {
        // set AbpRadzenUIResource as BaseTypes for your application's localization resources
        var crmResource = options.Resources.Get<CRMResource>();
        crmResource.AddBaseTypes(typeof(AbpRadzenUIResource));

        // if you don't want to use the default language list, you can clear it and add your own languages
        options.Languages.Clear();
        options.Languages.Add(new LanguageInfo("en", "en", "English"));
        options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
        options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
    });

    // Configure your web application's navigation menu
    Configure<AbpNavigationOptions>(options =>
    {
        options.MenuContributors.Add(new CRMMenuContributor());
    });
}
```

then add the following code on your `OnApplicationInitialization` method
```csharp
app.UseRadzenUI();
```

yuo can refer to the sample code [CRMBlazorWebModule](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/CRMBlazorWebModule.cs)

### 5. Config Menu
When you add razor page and need config menu , you should edit the [CRMMenuContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/samples/CRM.Blazor.Web/Menus/CRMMenuContributor.cs) class 

### 6. Config External Login
If you want to integrate third-party authentication such as Azure AD, it's quite straightforward. Simply update your configuration file as shown below, and add the necessary setup in your web project’s module. Once completed, third-party login functionality will be enabled and ready to use.You can refer to the sample project to see how it is implemented in practice.
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
### 7. Configuration Settings Page
In the process of system development, we often need to configure certain system or business parameters, such as email service providers, SMS service providers, etc. Usually, corresponding pages are developed to configure these parameters. The Abp framework provides [Settings](https://abp.io/docs/latest/framework/infrastructure/settings?_redirected=B8ABF606AA1BDF5C629883DF1061649A), which makes it convenient to save and manage such settings. Based on this, this UI component allows you to easily create configuration pages for unified management. By following the steps below, your custom configuration component will be automatically added as a tab item on the settings page:

#### (1) Create your parameter configuration service, for example: [AccountSettingsAppService](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Application/AccountSettingsAppService.cs)

#### (2) Create your parameter configuration Blazor component, for example: [AccountSettingComponent](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Components/Pages/Setting/AccountSettingComponent.razor)

#### (3) Define your parameter configuration Contributor by implementing the interface `ISettingComponentContributor`. This contributor is mainly used to add your parameter configuration Blazor component, for example: [AccountPageContributor](https://github.com/ShaoHans/Abp.RadzenUI/blob/main/src/Abp.Blazor.Server.RadzenUI/Blazor/SettingManagement/AccountPageContributor.cs)

#### (4) Finally, add your Contributor to the Module configuration


### 8. Don't forget migrate your database when you first run the app
