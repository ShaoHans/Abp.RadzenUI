# Tab Page 功能分析

## 需求理解

- 目标是评估当前 Blazor Server 项目提供类似后台系统顶部页签的 tab page 能力的难易程度。
- 这里区分两类能力：
  - 页面内部标签切换：同一路由内的内容分组。
  - 布局级多页面标签栏：多个路由同时打开、切换、关闭，类似浏览器页签。

## 执行计划

- [x] 检查路由与主布局承载点
- [x] 检查现有复杂页面与 Tabs 复用情况
- [x] 评估布局级 tab page 的实现路径与技术阻力
- [x] 输出结论与建议

## 分析结论

### 结论摘要

- 如果需求是“页面内部 tab”，当前项目实现难度低。
- 如果需求是“布局级 tab page”，当前项目实现难度中等，不算高，但不是简单拼一个 RadzenTabs 就能完成。
- 真正的成本不在 UI，而在导航接管、状态管理、缓存策略和关闭行为设计。

### 现状依据

- 主路由由 Components/Routes.razor 中的 Router + AuthorizeRouteView 承载，页面切换时仍是标准 Blazor 路由行为。
- 主布局 Components/Layout/MainLayout.razor 当前只有 Header、侧边栏和 Body，没有现成的 tab 栏承载区。
- 菜单导航来自 Components/Layout/MenuSiderbar.razor，直接把 ApplicationMenuItem.Url 绑定到 Path，说明导航链路比较直接。
- 页头 Components/Layout/Header.razor 已有标题与面包屑逻辑，可为 tab 标题、激活页展示提供部分元数据来源。
- 项目已存在页面内部 tabs：
  - Components/Pages/User/Edit.razor
  - Components/Pages/Setting/Manage.razor

### 难点拆解

#### 1. 页面内部 Tabs

- 当前已直接使用 RadzenTabs。
- 对应模式已经在用户编辑和设置页面里验证过。
- 这类需求通常只涉及单页面拆分、路由 fragment 或选中状态同步，难度低。

#### 2. 布局级 Tab Page

- 需要在 MainLayout 里增加统一 tab bar 区域。
- 需要一个 TabState 服务维护：已打开页签、当前激活页、关闭后的回退页。
- 需要拦截或监听 NavigationManager.LocationChanged，把普通菜单跳转转成“打开或激活 tab”。
- 需要解决标题来源：菜单名、PageTitle、路由参数页名的统一策略。
- 需要决定是否缓存页面实例。

### 真正的技术阻力

- Blazor Server 标准路由切换后，旧页面组件会卸载；如果没有缓存机制，切 tab 只会重新进入页面，表单状态和局部筛选状态会丢失。
- 若要保留页面状态，就不能只做一个视觉上的 tab 栏，需要额外设计缓存或状态恢复机制。
- 项目当前没有现成的 keep-alive、动态组件缓存、打开页签持久化服务。
- 菜单和面包屑是按当前路由实时解析的；如果要支持多个已打开页面，需要新增“当前激活 tab”到 UI 元数据之间的映射。

## 难度分级

### 低难度

- 只做视觉上的 tab 栏。
- 点击菜单后把当前 URL 加进 tab 列表。
- 切 tab 本质仍然是 NavigateTo。
- 不保留未提交表单状态，不支持复杂缓存。

### 中难度

- tab 关闭、激活、重复打开去重。
- 支持刷新后恢复已打开 tab 列表。
- tab 标题、图标、默认首页、关闭当前后回退逻辑完整。
- 对列表页的筛选条件或页码做轻量恢复。

### 中高难度

- 切换 tab 后保留每个页面的实时组件状态。
- 对编辑页、树形页、弹窗上下文都做到无感恢复。
- 支持拖拽排序、右键菜单、关闭其他、关闭全部、固定首页等完整桌面式体验。

## 推荐实现路径

### 方案 A：先做布局级轻量 Tab Page

- 在 MainLayout 增加 tab bar。
- 新增 TabPageState 服务，维护打开页签集合和当前激活页。
- 监听 LocationChanged，同步打开/激活 tab。
- tab 点击时调用 NavigateTo。
- tab 关闭时按最近访问顺序或左邻页签回退。
- 可选用 sessionStorage 或 localStorage 持久化已打开页签。

适用场景：
- 主要目标是提升导航效率。
- 可以接受切回页面后重新加载。

### 方案 B：在方案 A 基础上加页面状态恢复

- 优先对列表页恢复查询参数、分页、排序。
- 对编辑页恢复关键表单模型，而不是缓存整个组件实例。
- 通过 query string、fragment、sessionStorage 或 scoped service 保存状态。

适用场景：
- 想提升体验，但不希望引入高复杂度组件缓存框架。

### 方案 C：做真正意义上的页面实例缓存

- 需要引入动态组件宿主、页面缓存容器、生命周期管理。
- 还要处理权限变化、资源释放、内存增长、长连接状态等问题。
- 在 Blazor Server 下可以做，但复杂度明显上升，不建议作为第一阶段目标。

## 建议结论

- 以当前项目结构看，“先做一个可用的 tab page”是可行的，难度属于中等偏低。
- 最优雅的第一阶段不是追求完整 keep-alive，而是先做“布局级 tab 管理 + 轻量状态恢复”。
- 如果你的预期是和 Vue Admin、Ant Design Pro 那类多标签工作台完全一致，第二阶段成本会明显上升。

## 结果复盘

- 当前项目并不缺 Tabs 组件能力，缺的是布局级页面管理机制。
- 因此这件事不是 UI 难，而是状态与导航设计难。
- 结论：建议分两阶段实施，第一阶段可控，第二阶段再根据实际页面类型决定是否做深缓存。