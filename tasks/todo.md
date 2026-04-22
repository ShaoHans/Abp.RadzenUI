# Todo

## Plan
- [x] 确认 MainLayout 首屏 loading 的结构与样式控制点
- [x] 重设计 loading 视觉层次，保留现有渲染切换逻辑不变
- [x] 运行针对性错误检查或构建验证改动可用
- [x] 将 loading 品牌文案改为复用 TitleBarSettings.Title
- [x] 将 loading 标题与副标题改为 AbpRadzenUIResource 多语言资源
- [x] 运行针对性错误检查或构建验证文案来源改造
- [x] 将 loading 中央图标改为复用 LoginPageSettings.LogoPath
- [x] 运行针对性错误检查或构建验证 loading 图标来源改造

## Notes
- 初步假设：当前 loading 观感单薄主要源于结构只有一个 logo 容器，而全局样式只提供了简单的渐变与阴影动画。
- 控制点已确认：结构在 MainLayout.razor，样式在 wwwroot/css/site.css 的 rz-app-loading 与 logo-loading 段落。
- 当前文案问题已确认：MainLayout 直接写死品牌名和英文提示语，没有复用 TitleBarSettings 与 AbpRadzenUIResource。
- 当前图标问题已确认：MainLayout 的 loading 标识仍由 CSS 伪元素绘制，没有复用 LoginPageSettings.LogoPath。

## Review
- 改造结果：将原先仅包含 logo 的加载态，调整为带品牌标识、标题、副标题、进度条和节奏点动画的居中面板，并重写背景氛围与标识动效。
- 验证结果：MainLayout.razor 与 site.css 无分析错误；dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj 通过。
- 文案来源改造：loading 品牌文案已切换为 TitleBarSettings.Title，标题与副标题已切换为 AbpRadzenUIResource 资源键。
- 二次验证结果：MainLayout.razor、en.json、zh-Hans.json 无分析错误；dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj 再次通过。
- 图标来源改造：loading 中央图标已切换为读取 LoginPageSettings.LogoPath，外层 halo 与容器动画保留。
- 三次验证结果：MainLayout.razor 与 site.css 无分析错误；dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj 通过。
