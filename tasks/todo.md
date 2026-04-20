# Todo

## Plan

- [x] 对照 Radzen 官方布局方案，修正主布局与左右侧栏的移动端行为
- [x] 在共享层启用 DataGrid 响应式支持，避免逐页重复修改
- [x] 补充必要的移动端样式，确保内容区可收缩、可滚动、不互相挤压
- [x] 运行针对性构建验证改动未引入编译错误

## Notes

- 假设：当前手机端错乱的主因是主布局固定三列网格，加上侧栏未启用 overlay responsive 模式，导致正文宽度被持续压缩。
- 判别检查：修正后在窄屏下，菜单/主题面板应覆盖正文而不是挤压正文；DataGrid 应保留横向滚动。

## Result Review

- Radzen 官方 overlay 示例依赖侧栏脱离网格流；本次保留桌面三列布局，同时在小屏断点下把左右侧栏切成 fixed overlay，并加了遮罩关闭行为。
- 共享 LocalizedDataGrid 默认启用 Responsive，并通过样式让表格保留横向滚动，符合 Radzen DataGrid 的移动端用法。
- 为避免 `RadzenMediaQuery` 首屏不回传当前匹配状态，主布局首次渲染时会主动读取当前 viewport，并据此决定手机端首屏默认收起菜单、桌面端默认展开菜单。
- viewport helper 已内联到 App 页面，避免外部脚本缓存或加载时机导致首屏判定失效；同时布局增加了 JS 调用容错，防止 circuit 因 helper 缺失直接中断。
- 已多次通过命令 dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj -c Debug 验证编译成功，并重启 samples/CRM.Blazor.Web 运行最新代码。
- 当前会话内的最终手机态验收仍受限于 VS Code 集成浏览器窗口尺寸，Playwright 回读到的 `window.innerWidth` 仍为 1170，无法在这里严格证明 390 宽度下的首屏表现；需要在真实窄屏浏览器或设备上做最终 UI 确认。