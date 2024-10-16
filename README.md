<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI is a UI theme built on the Abp framework and developed using the Radzen Blazor component.

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

## 🎨Page display

### 1.The login page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2.The list page
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3.The other list page with datagrid filter
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

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

### 6. Don't forget migrate your database when you first run the app
