# 使用 dotnet new 模板接入 Abp RadzenUI

本文档说明如何使用仓库内置的 `dotnet new` 模板，把 Abp RadzenUI 接入到已有 ABP Blazor Server 项目中。

## 适用场景

该模板适合已经通过 ABP CLI 创建好的 Blazor Server 项目，例如：

```powershell
abp new MyCompany.MyProject -u blazor-server -dbms PostgreSQL -m none --theme leptonx-lite -csf
```

模板不会直接修改已有 Module、csproj 或 Program 文件，而是生成一组接入辅助文件和清单。这样可以避免不同 ABP 项目结构差异导致脚手架误改代码。

## 安装模板

在本仓库根目录执行：

```powershell
dotnet new install .\templates\AbpRadzenUI.Integration
```

安装后可以查看模板：

```powershell
dotnet new list abp-radzenui-integration
```

## 在 Web 项目中生成接入文件

进入宿主 Blazor Server Web 项目目录，例如：

```powershell
cd .\src\MyCompany.MyProject.Blazor
```

执行模板：

```powershell
dotnet new abp-radzenui-integration -n MyProject --rootNamespace MyCompany.MyProject.Blazor --title "My Project"
```

如果要启用 Radzen 高级主题：

```powershell
dotnet new abp-radzenui-integration -n MyProject --rootNamespace MyCompany.MyProject.Blazor --title "My Project" --premiumTheme true
```

模板会生成：

- `RadzenUI/<ProjectName>RadzenUIIntegrationExtensions.cs`
- `Menus/<ProjectName>Menus.cs`
- `Menus/<ProjectName>MenuContributor.cs`
- `Components/Pages/Home.razor`
- `RadzenUI/RADZENUI-INTEGRATION.md`

## 修改 Web 项目

### 1. 添加 NuGet 包

```powershell
dotnet add package AbpRadzen.Blazor.Server.UI
```

如果是在本仓库内开发示例，也可以临时使用项目引用：

```xml
<ProjectReference Include="..\..\src\Abp.Blazor.Server.RadzenUI\Abp.Blazor.Server.RadzenUI.csproj" />
```

### 2. 添加模块依赖

在 Web Module 的 `[DependsOn]` 中加入：

```csharp
typeof(AbpRadzenUIModule)
```

并添加：

```csharp
using Abp.RadzenUI;
```

### 3. 调用接入辅助方法

在 Web Module 的 `ConfigureServices` 中调用模板生成的方法。示例：

```csharp
context.Services.AddRadzenUIIntegration<Home, MyProjectResource, MyProjectMenuContributor>();
```

需要添加的 using 通常包括：

```csharp
using MyCompany.MyProject.Blazor.Components.Pages;
using MyCompany.MyProject.Blazor.Menus;
using MyCompany.MyProject.Blazor.RadzenUI;
```

该方法会完成：

- 设置 `AbpRadzenUIOptions.RouterAdditionalAssemblies`。
- 设置标题栏和登录页标题。
- 为宿主本地化资源添加 `AbpRadzenUIResource` 基类。
- 注册业务菜单贡献器。

### 4. 启用 RadzenUI 端点

在 `OnApplicationInitialization` 的管线末尾调用：

```csharp
app.UseRadzenUI();
```

建议放在认证、授权、审计、Swagger 等中间件之后。

### 5. 清理旧主题

如果项目是从 LeptonX 或其他主题生成的，通常需要移除旧主题相关 NuGet 包、布局和默认页面，避免路由、样式和布局冲突。

常见清理项：

- 默认 `Pages` 或 `Components/Pages` 首页。
- 旧主题模块依赖。
- 旧主题静态资源。
- 与 `/account/login`、`/` 等路由冲突的页面。

## 生成后的最小代码形态

模板生成的扩展方法形如：

```csharp
context.Services.AddRadzenUIIntegration<Home, MyProjectResource, MyProjectMenuContributor>();
```

它比把所有配置写进 Web Module 更容易复用，也能让接入点保持清晰。后续如果需要自定义标题栏、登录页 Logo、主题或第三方登录图标，可以直接修改生成的 `RadzenUIIntegrationExtensions`。

## 验证

完成接入后，建议先构建：

```powershell
dotnet build -v minimal
```

再运行 Web 项目并检查：

- `/` 是否显示模板首页。
- `/account/login` 是否进入 RadzenUI 登录页。
- 左侧菜单是否包含 Home。
- 标题栏是否显示配置的标题。
- 切换主题、语言菜单、用户菜单是否正常。

## 卸载模板

如需卸载：

```powershell
dotnet new uninstall .\templates\AbpRadzenUI.Integration
```
其中 `-n` 应使用不含点号的短项目名，因为它会参与生成 C# 类型名；`--rootNamespace` 使用目标 Web 项目的真实根命名空间。

