# Todo

## Plan
- [x] 确认审计日志与登录日志页每页大小变更失败的控制路径
- [x] 用最小改动修复页大小被重置的问题，并保持默认值为 20
- [x] 运行针对性构建或错误检查验证修复

## Notes
- 初步假设：页面代码在每次 LoadData 前把 _defaultPageSize 强制写回 20，导致分页器下拉选择立即失效。
- 已确认：AuditLog 与 IdentitySecurityLog 两个页面都在加载流程中重置 _defaultPageSize；IdentitySecurityLog 还会在 Reset 时再次重置。

## Review
- 修复方式：将默认页大小 20 移到页面构造函数中，只初始化一次，不再在 LoadData/Reset 时覆盖用户选择。
- 验证结果：受影响文件无分析错误；dotnet build src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj 通过。
