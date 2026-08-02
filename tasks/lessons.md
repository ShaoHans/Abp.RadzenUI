# Lessons

- UI 扩展优先复用 Radzen 官方组件，只有组件无法承载时才补最小 CSS/JS。
- 用户明确要求 UI 调整不要新增 `.css` 文件时，优先用 Radzen 组件参数、布局参数和最小内联样式完成；先满足约束，再考虑局部样式文件。
- loading、登录页这类品牌入口优先复用 LoginPageSettings/TitleBarSettings，避免在 Razor 或 CSS 里写死图标和品牌素材。
- 不要因为当前编辑器停留在某个业务仓库就默认任务实现也在该仓库；如果用户明确点名 Abp-RadzenUI 这类目标项目，必须先锁定目标仓库，其他仓库只作为交互或实现参考。
- 公共库里的“类型”字段如果用户明确拒绝类型表，就不要继续沿着字典表/定义表设计推进；改用字符串字段 + 可 override 的选项提供接口，避免过度设计。
- 当消息正文可能是 HTML 时，设计阶段就要把它视为富文本展示需求，而不是默认按纯文本处理；后续实现必须同时考虑可渲染和安全边界。
- 富文本详情在暗黑模式下不能只设置容器背景；还要处理正文内部常见的白字/黑字/浅底内联样式，否则消息内容会在主题切换后局部不可见。
- 公共 Razor 组件不要硬编码某个布局专用的 class；一旦要在登录页、Header 等多个容器复用，应把样式类参数化或按宿主分别包裹，否则很容易退回到组件库默认外观。
- Radzen 组件一旦同时使用 `Template` 和子节点，优先改成显式 `Template=@...`、`ChildContent=@...` 参数传递；不要在组件体内混用条件块和匿名子内容，否则 Razor 很容易触发 `RZ9996`。
- 菜单 badge 这类动态装饰如果来源于 `IMenuItemDecorationContributor`，就不能只在侧边栏初始化时取一次；至少要绑定到路由变化或某个 scoped state 事件上，否则数字只会在整页刷新后更新。
- 公共库里的菜单 badge 刷新不要偷懒复用消息中心这类业务状态；应提供独立的 `MenuItemDecorationState`，并用刷新键让侧边栏只重算受影响的菜单项，避免一次消息已读把待办等无关查询也全部重跑。
- 数据库本地化覆盖走 ABP 的 `ILocalizationResourceContributor`（`IsDynamic=true`）而不是 `IExternalLocalizationStore`：后者要求返回抽象的 `LocalizationResourceBase`，开源框架里没有可直接 new 的非类型化具体资源。`LocalizationResourceContributorList.GetOrNull` 是**倒序**遍历（最后加入的先命中），`Fill` 是正序覆盖，所以把 DB 贡献器在 `IPostConfigureOptions<AbpLocalizationOptions>` 里 `Add` 到每个资源的**最后**，DB 值就会盖过静态 JSON。要读取“纯静态基准值”用 `resource.Contributors.Fill(culture, dict, includeDynamicContributors:false)`。
- ABP 的 `IStringLocalizer`/`IAuthorizationService` 扩展方法（如 `IsGrantedAsync`）定义在 `Microsoft.AspNetCore.Authorization` 命名空间下，不是 `Volo.Abp.Authorization`；`.razor.cs` 代码后置文件不吃 `_Imports.razor` 的 `@using`，缺了这个 using 会报“未包含 IsGrantedAsync 的定义”。
- Blazor CSS 隔离(`.razor.css` + `::deep`)只给 `.razor` 里的**原生 HTML 元素**打 `b-{hash}` scope 属性,不会打给 `<RadzenStack>` 这类子组件的根节点。所以 `::deep` 前缀若挂在 `<RadzenStack class="...">` 上,整份 scoped 规则会匹配不到任何元素、静默失效(表现为“改了没效果”)。要控制 Radzen 内部结构,更稳的做法是把规则写进 `wwwroot/css/site.css`(由 `BlazorGlobalStyleContributor` 全局加载),用一个包裹用的原生 `<div class="xxx">` 作用域类前缀 + Radzen 自带结构类(`.rz-tabview`/`.rz-tabview-left`/`.rz-tabview-panels`/`.rz-tree`),既不依赖 scope 属性,也不依赖组件是否透传 `class`。注意:无论 scoped 还是 site.css,Razor 标记改动都必须**重新 build + 重启** Blazor Server 宿主才生效,热重载不认。
- 嵌套 RadzenTabs + RadzenTree 想只保留“树内单滚动条”时:Radzen 默认 `.rz-tabview-panels{flex:1;overflow:auto}`、`.rz-tree{overflow:auto}` 都会各自出滚动条。正解是搭一条 flex 撑满链(每层 `min-height:0`,tab 面板 `overflow:hidden`,面板/子 tabview `height:100%`),把唯一滚动条交给树;直接删滚动条而不撑高会让整个 Dialog 变成滚动体,表头跟着滚走。链条根部必须是**确定高度**(`height:calc(...)` 而非只有 `max-height`),否则子层 `height:100%` 无法解析、树拿不到边界。
- 不要给 `.rz-tabview-panel` 直接加 `display:flex`:Radzen 用**内联 style** 切换未选中面板的显隐,内联会盖过你的类规则(选中面板可能被强行 block、未选中面板可能被你的 flex 顶开)。当一个 tab 面板里除了树还有别的元素(如“全选”复选框、分隔线),要让树独占滚动,应在面板内自己包一个 `display:flex;flex-direction:column;height:100%` 的原生 `<div>`,树设 `flex:1;min-height:0`;面板只给 `height:100%`。DialogFormLayout 的滚动体默认 `BodyStyle="flex:1;overflow:auto"`,复用它做单滚动布局时把 `BodyStyle` 覆写成 `display:flex;flex-direction:column;flex:1;min-height:0;overflow:hidden`,让内部 tabs 撑满、滚动条落到树上。
- ABP 开源版 `AbpSignInManager.PreSignInCheck` 有三条会直接返回 `SignInResult.NotAllowed`(登录页报 "IsNotAllowed"):`!user.IsActive`、`user.ShouldChangePasswordOnNextLogin == true`、`ShouldPeriodicallyChangePasswordAsync`(定期改密超期,用 `LastPasswordChangeTime ?? CreationTime` 判断)。其中 **`ShouldChangePasswordOnNextLogin`(“下次登录须改密”)是 Account **Pro** 商业版才有配套强制改密页的功能;开源版设成 true 会把用户永久挡在登录外、且没有任何改密入口**——所以管理端不要提供“下次登录须改密”开关。管理员设密应照 ABP 官方 `IdentityUserAppService.UpdateAsync` 的做法(`RemovePasswordAsync`+`AddPasswordAsync`),并额外 `user.SetShouldChangePasswordOnNextLogin(false)` 显式清除该标志,既符合“管理员刚设好密码”的语义,也能把之前误被置 true 而锁死的用户救回。
- 本地化贡献器的同步 `GetOrNull`/`Fill` 会被 `L["key"]` 高频调用，不能在里面查库：用单例 store 做 L1 内存快照（同步读）+ `IDistributedCache` 做 L2（跨节点），写操作即时失效本节点 L1、移除 L2，其他节点靠 stale-while-revalidate 在 `RefreshInterval` 内收敛。store 的后台/fire-and-forget 查库必须自建 scope + `IUnitOfWorkManager.Begin`，因为脱离了请求作用域。
