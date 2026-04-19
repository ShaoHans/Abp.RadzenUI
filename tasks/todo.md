# Linked Accounts 设计与实施计划

## 需求规格
- 目标：在不依赖 ABP Commercial 授权的前提下，为当前 CRM Demo 实现一套接近 ABP Commercial Linked Accounts 的账号关联与无退出切换能力。
- 架构约束：
  - Linked Accounts 需抽成独立类库服务，避免和 Abp.Blazor.Server.RadzenUI 的 UI 逻辑强耦合。
  - Abp.Blazor.Server.RadzenUI 负责集成该服务并提供 Radzen 组件界面。
  - UI 尽量复用 Radzen 官方组件，减少自定义 CSS/JS，仅在缺少承载能力时补最小量样式与脚本。
- 核心能力：
  - 当前已登录账号可以发起“关联新账号”。
  - 允许关联不同租户下的账号。
  - 关联过程中需要二次登录目标账号，并在成功后建立双向关联关系。
  - 已关联账号列表可见，并支持一键切换登录。
  - 支持删除直接关联。
  - 支持返回原账号。
  - 支持间接关联可见与可切换。
  - 保留审计链路，能识别原账号、当前账号、来源租户、目标租户、切换时间。
- 安全要求：
  - 不能仅通过切换 CurrentTenant 完成身份切换，必须重新签发认证票据。
  - 所有关联/切换请求都必须绑定一次性令牌和有效期。
  - 禁止自关联、重复关联、禁用账号关联、软删除账号关联。
  - 切换后 Claims、Tenant、Permission、Feature 都必须按目标账号重新生效。

## 本地锚点与设计假设
- 入口锚点：src/Abp.Blazor.Server.RadzenUI/Components/Layout/Header.razor
- 认证锚点：src/Abp.Blazor.Server.RadzenUI/Controllers/AccountController.cs
- 登录页锚点：src/Abp.Blazor.Server.RadzenUI/Components/Pages/Account/Login.razor
- 服务锚点：新增 src/Abp.RadzenUI.LinkAccounts 独立模块，承载链接关系、流程状态、切换签发抽象与实现。
- 假设：通过在独立 LinkAccounts 模块中承载关系图、流程状态与切换服务，再由现有 AccountController 登录链路消费“链接流程意图”和“切换流程意图”统一重签 cookie，可以在当前架构下实现与商业版相近的 linked accounts 行为。
- 快速证伪检查：若当前认证链路无法在登录成功后安全读取/消费一次性流程令牌并重定向到确认页，则需补专用控制器或认证服务层。

## 实施步骤
- [ ] 创建独立类库项目 src/Abp.RadzenUI.LinkAccounts，并接入解决方案。
- [ ] 在 Abp.Blazor.Server.RadzenUI 中集成 LinkAccounts 模块依赖。
- [ ] 设计领域模型、数据库结构与审计字段。
- [ ] 设计应用服务 API：列表、发起关联、完成关联、切换登录、删除关联、返回原账号。
- [ ] 设计认证流程：关联登录、登录成功确认页、切换签发、返回原账号。
- [ ] 设计 UI：用户菜单入口、Linked Accounts 弹窗、跨租户登录表单、成功确认页、返回原账号提示，优先使用 Radzen 官方组件。
- [x] 统一 Current Session 页签、顶部摘要与成功回调页的会话展示文案与账号标签拼装逻辑。
- [ ] 设计权限与安全边界：令牌、防重放、有效期、禁用场景、间接关联图遍历。
- [ ] 设计兼容策略：本地登录、外部登录、多租户、动态 Claims、OpenIddict。
- [ ] 输出验证方案：单元测试、集成测试、手工验证脚本。
- [x] 为关联会话回归场景补自动化测试，覆盖重复切回账户时的 session 栈裁剪与摘要来源正确性。

## 验证清单
- [ ] 同租户两个账号可互相关联并互切。
- [ ] 跨租户两个账号可互相关联并互切。
- [ ] A-B、B-C 时，A 可看到并切换到 C 的间接关联。
- [ ] 删除 A-B 直接关联后，间接关联视图与切换权限符合预期。
- [ ] 切换后 CurrentUser、CurrentTenant、权限、菜单、数据隔离均更新。
- [ ] 返回原账号后，审计链与 UI 状态恢复正确。
- [x] 顶部摘要、Current Session 页签、回调页对同一会话展示一致，且不会再出现 self-to-self 或错误来源租户名。

## 复盘
- 待实现后补充。