# Lessons

- UI 扩展优先复用 Radzen 官方组件，只有组件无法承载时才补最小 CSS/JS。
- 用户明确要求 UI 调整不要新增 `.css` 文件时，优先用 Radzen 组件参数、布局参数和最小内联样式完成；先满足约束，再考虑局部样式文件。
- loading、登录页这类品牌入口优先复用 LoginPageSettings/TitleBarSettings，避免在 Razor 或 CSS 里写死图标和品牌素材。
