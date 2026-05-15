# 消息模块表结构设计（2026-05-14）

## 本轮执行计划

- [x] 核对 Abp-RadzenUI 当前独立模块组织方式、实体基类与 EF 配置风格。
- [x] 明确消息类型字段不采用枚举，给出适用于公共类库的可扩展建模方案。
- [x] 输出消息模块首版表结构、索引与约束设计，等待用户确认后再开始编码。

## 需求规格

- 目标：在 Abp-RadzenUI 中新增独立消息模块，支持租户内按用户投递站内消息，并为 Header 侧边栏、消息列表页、消息详情侧边栏提供统一数据源。
- 核心字段：TenantId、UserId、Title、Content、MessageType、IsRead、ReadTime、CreationTime、ExtraProperties。
- 模块定位：项目形态对齐 Abp.RadzenUI.DataDictionary.EntityFrameworkCore，避免把业务枚举和业务消息类型硬编码进公共库。
- 查询需求：支持按全部、已读、未读、消息类型、标题模糊、接收时间范围查询；默认按创建时间倒序。
- 写入约束：标题超出 200 字符时入库前截断；消息详情打开时要支持自动标记已读。
- UI 约束：消息类型筛选在 UI 中使用下拉框展示，下拉取值接口必须允许消费系统 override。
- UI 约束：消息详情需要支持富文本展示，`Content` 允许存放 HTML 片段并按 HTML 渲染。
- 非功能要求：不做权限控制，但要保留多租户隔离、用户隔离、后续扩展消息跳转参数和展示样式的能力。

## 当前设计结论

- 消息类型字段采用字符串代码而不是枚举；公共库只约束长度、查询与可选注册能力，不预设固定类型集合。
- 用户已确认不引入消息类型定义表；首版采用“单消息表”设计，消息类型字段名称直接使用 `MessageType`。
- UI 的消息类型下拉不从数据库类型表取值，改为由独立应用服务接口提供选项列表；该接口在默认实现下可返回空集合或基础值，并允许消费系统通过依赖替换或继承 override。
- 消息详情页和 Header 侧边栏详情需按 HTML 富文本渲染 `Content`，实现时应以受控渲染为前提，避免把公共库默认行为做成任意未审查脚本执行入口。

## 本轮结果复盘

- 已确认 Abp-RadzenUI 当前独立模块惯例是实体与 EF 映射放在独立 EntityFrameworkCore 项目中，AppService 与 UI 放在 Abp.Blazor.Server.RadzenUI 内承接。
- 已确认消息类型若直接做成枚举，会把公共库和消费方业务语义绑死；改为字符串代码后，由消费方自行定义即可，无需在公共库内维护类型表。
- 已确认消息类型下拉值来源不走持久化表，而走可 override 的查询接口，这样既满足公共库默认可运行，也给消费系统保留扩展口。
- 已确认 `Content` 需要面向 HTML 富文本展示，因此字段保持长文本，不做纯文本约束；后续实现时需要同时考虑默认渲染安全边界。
- 当前阶段只产出表结构设计与字段建模结论，不进入代码实现；待用户确认后再开始建模块、实体、EF 映射、AppService 与 UI。

## 实现阶段计划

- [x] 新增独立 EF 项目 `Abp.RadzenUI.Messages.EntityFrameworkCore`，落消息实体、常量、DbContext 与 EF 映射。
- [x] 在 `Abp.Blazor.Server.RadzenUI` 中接入项目引用、DbContext 注册和菜单常量；按需求保持消息模块无权限控制。
- [x] 新增消息 DTO、查询输入、消息类型下拉 DTO，以及 `IMessageAppService`、`IMessageTypeLookupAppService`。
- [x] 实现消息应用服务，覆盖 CRUD、批量已读、全部已读、未读数和可 override 的消息类型下拉默认实现。
- [x] 实现消息列表页、消息详情侧边栏、Header 消息图标与右侧消息面板，支持 HTML 富文本展示和多选批量已读。
- [x] 补充本地化文本，并完成面向 `Abp.Blazor.Server.RadzenUI` 的定向编译验证。

## 实现结果复盘

- 已新增独立项目 `src/Abp.RadzenUI.Messages.EntityFrameworkCore`，完成 `UserMessage` 实体、DbContext、表映射与索引配置，并接入主项目的默认仓储注册。
- 已在 `src/Abp.Blazor.Server.RadzenUI` 中补齐消息应用层契约、默认 `MessageAppService`、独立 `MessageTypeLookupAppService` 和 Mapperly 映射；默认查询只返回当前登录用户消息，详情打开时自动标记已读。
- 已完成 Header 未读消息角标、右侧消息面板、完整消息列表页和详情侧边栏，支持多选批量已读、全部已读、未读数刷新以及 HTML 内容渲染。
- 已补充中英文消息相关本地化文本，并将新项目加入 `Abp.RadzenUI.sln` 与 `Abp.RadzenUI.slnx`。
- 已使用 `dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj -nologo` 完成定向编译验证，当前主项目构建通过。

## 消息模块 UI 重构计划（2026-05-15）

- [x] 修复 Header 消息入口图标不可见问题，改为稳定可见的 Radzen 图标呈现。
- [x] 重做 Header 侧边消息面板，修正文案、筛选和列表层级，减少自定义样式。
- [x] 重构消息中心列表页布局，对齐客户列表示例：筛选区独立成卡片，操作区放进 Grid Header。
- [x] 保持消息详情与现有读状态联动逻辑不变，只调整消息列表和入口交互层。
- [x] 完成 `Abp.Blazor.Server.RadzenUI` 定向编译验证，并补充本轮结果复盘。

## 消息模块 UI 重构结果复盘（2026-05-15）

- 已将 Header 消息入口从自定义 `rzi-notifications` 图标切换为 `RadzenIcon`，并补充无障碍标签，避免图标在当前主题下不可见。
- 已为 `MessageInboxPanel` 和 `DetailContent` 显式指定 `AbpRadzenUIResource`，修复侧边栏中本地化 key 直接透出的现象。
- 已将消息侧边栏重排为 Radzen Card + Stack 的结构：头部汇总、筛选切换、批量操作和消息卡片列表均由官方组件承载。
- 已将消息中心列表页改为“查询卡片 + Grid Header 操作区”的结构，交互样式对齐参考客户列表页，行标题改为 `SideDialogLink`，减少自定义样式依赖。
- 已使用 `dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj -nologo` 完成定向编译验证，当前构建通过。

## 消息侧栏二次精简结果（2026-05-15）

- 已按反馈移除消息侧栏顶部卡片容器，改为更轻量的按钮区和筛选区。
- 已移除未读数量展示、批量已读功能和消息列表复选框，保留全部已读与查看更多入口。
- 已将顶部操作按钮和筛选按钮统一缩为 `ButtonSize.Small`，减轻视觉占用。
- 已再次执行 `dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj -nologo`，当前构建通过。