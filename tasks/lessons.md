# Lessons

- Linked Accounts 需求默认按独立模块设计，避免直接把服务与 UI 粘在 Abp.Blazor.Server.RadzenUI 内部。
- UI 扩展优先复用 Radzen 官方组件，只有组件无法承载时才补最小 CSS/JS。
- 新增用户约束后先更新 tasks/todo.md 与 tasks/lessons.md，再继续实现，避免后续返工。
- 多租户切换控制器不能固定重定向到普通登录页；必须保留当前 returnUrl 和 linkToken 等查询参数，否则关联登录流程会丢状态。
- Blazor Server 登录页不要在 OnInitializedAsync 里对 linked flow 直接做 NavigateTo；跨租户关联场景下认证态与查询参数存在短暂切换窗口，需延后到渲染后再导航，并且将 skipAutoLogin/linkToken/returnUrl 一并纳入保护条件。
- Linked Accounts 的 flow token 和 session stack 不能按当前租户作用域写入分布式缓存；跨租户完成关联时必须在 host 作用域共享同一份缓存状态，否则目标租户读取不到源租户创建的 token，会误报 expired or already consumed。