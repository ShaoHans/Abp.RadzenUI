<h1 align="center">Abp RadzenUI</h1>

<div align="center">

Abp RadzenUI 是使用[Radzen Blazor](https://github.com/radzenhq/radzen-blazor)组件库开发的基于[ABP](https://github.com/abpframework/abp)框架的Blazor Server模式的UI主题.

![build](https://github.com/ShaoHans/Abp.RadzenUI/actions/workflows/publish-nuget.yml/badge.svg)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/v/AbpRadzen.Blazor.Server.UI.svg?color=red)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![AbpRadzen.Blazor.Server.UI](https://img.shields.io/nuget/dt/AbpRadzen.Blazor.Server.UI.svg?color=yellow)](https://www.nuget.org/packages/AbpRadzen.Blazor.Server.UI/)
[![Abp.RadzenUI](https://img.shields.io/badge/License-MIT-blue)](https://github.com/shaohans/Abp.RadzenUI/blob/master/LICENSE)

</div>

[English](README.md) | 简体中文

## 🎨部分页面展示

### 1.登录页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/login.png)

### 2.列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list.png)

### 3.带有Filter的列表页面
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/list-with-filter.png)

### 4.其他主题
![image](https://raw.githubusercontent.com/ShaoHans/Abp.RadzenUI/refs/heads/main/samples/CRM.Blazor.Web/wwwroot/images/switch-theme.png)

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

### 6. 第一次运行示例程序的时候不要忘了执行迁移代码

