# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Abp RadzenUI is a **Blazor Server UI theme + admin-shell library** built on the [ABP Framework](https://github.com/abpframework/abp) and the [Radzen Blazor](https://github.com/radzenhq/radzen-blazor) component library. It is distributed as NuGet packages (headline: `AbpRadzen.Blazor.Server.UI`), not as an application. The `samples/CRM.*` projects are the reference host application that consumes it.

Beyond styling, the core package ships a full Blazor Server runtime entry point, Radzen service registration, layout/menu/localization/auth-redirect wiring, and built-in modules: data dictionary, user messages, linked accounts, avatar upload, and the ABP management pages (identity, tenants, settings, audit/security logs).

- Target framework: `net10.0` everywhere. Central package versions live in `Directory.Packages.props` (ABP `10.5.0`, `Radzen.Blazor` `11.1.1`). Common build props in `Directory.Build.props`; package version is `AbpRadzenUIPackageVersion`.
- **`AGENTS.md` is the authoritative deep-dive** (in Chinese) covering module registration, UI shell, menus, and per-module directories. Read it before non-trivial changes to the core package.

## Commands

```powershell
# Build the whole solution (.slnx is the primary solution format; .sln also works)
dotnet build Abp.RadzenUI.slnx -v minimal

# Build a single project
dotnet build src/Abp.RadzenUI.Application/Abp.RadzenUI.Application.csproj -v minimal

# Run tests (xUnit + Moq)
dotnet test tests/Abp.RadzenUI.LinkAccounts.Tests/Abp.RadzenUI.LinkAccounts.Tests.csproj -v minimal

# Run a single test by name
dotnet test tests/Abp.RadzenUI.LinkAccounts.Tests/Abp.RadzenUI.LinkAccounts.Tests.csproj --filter "FullyQualifiedName~LinkedAccountSessionRegressionTests"
```

Running the `samples/CRM.*` app requires a configured PostgreSQL connection string, Redis, and OpenIddict setup, plus applied EF Core migrations.

## Architecture

Layered following ABP conventions. All packages share `RootNamespace` `Abp.RadzenUI`:

- `src/Abp.Blazor.Server.RadzenUI` — **the core UI package** (`AbpRadzen.Blazor.Server.UI`, `Microsoft.NET.Sdk.Razor`). Contains the module (`AbpRadzenUIModule`), layout/shell, menus, pages, and built-in-module UI.
- `src/Abp.RadzenUI.Application[.Contracts]` — app services, DTOs, permission definitions.
- `src/Abp.RadzenUI.Domain[.Shared]` — entities (data dictionary, user messages) and shared constants/localization.
- `src/Abp.RadzenUI.EntityFrameworkCore` — `AbpRadzenUIDbContext` and model mappings for built-in entities.
- `src/Abp.RadzenUI.LinkAccounts` — standalone linked-accounts module (`AbpRadzen.LinkAccounts`).

### Integration contract (how a host app wires this in)

This is the load-bearing part of the design — a host ABP Blazor Server project consumes the theme by:

1. Depending on `AbpRadzenUIModule` from its web module.
2. Configuring `AbpRadzenUIOptions` (see `src/Abp.Blazor.Server.RadzenUI/AbpRadzenUIOptions.cs`). **`RouterAdditionalAssemblies` is mandatory** — without it the router in `Components/Routes.razor` cannot discover the host's Razor pages.
3. Making the host localization resource inherit `AbpRadzenUIResource` to reuse built-in texts.
4. Registering a business `IMenuContributor` (never hard-code menus in layout).
5. Calling `app.UseRadzenUI()` at the **end** of `OnApplicationInitialization` — this maps `MapRazorComponents<App>()`, enables interactive server render mode, and appends `RouterAdditionalAssemblies`.

`samples/CRM.Blazor.Web/CRMBlazorWebModule.cs` is the canonical worked example. `AbpRadzenUIModule.ConfigureServices` is where the theme replaces ABP defaults (Cookie auth redirects to `/account/login` / `/forbidden`, swaps `IUiMessageService`/`IUiNotificationService` for Radzen implementations, clears default menu contributors, registers setting-page contributors).

### Key extension points

- **CRUD pages**: inherit `AbpCrudPageBase` (`src/Abp.Blazor.Server.RadzenUI/AbpCrudPageBase.cs`) for consistent Radzen DataGrid loading, paging/sorting, create/edit/delete dialogs, `Create/Update/DeletePolicyName` permission checks, and error handling.
- **Settings pages**: implement `ISettingComponentContributor` and register it in `SettingManagementComponentOptions.Contributors`.
- **EF Core**: host DbContext must call `builder.ConfigureAbpRadzenUI()` in `OnModelCreating` and declare the built-in DbSets if using those modules.

## Working conventions (from AGENTS.md / copilot-instructions.md)

- **Keep the library generic**: no hard-coded business project names, logos, layout classes, or business state in the shared packages. Branding goes through `LoginPageSettings`, `TitleBarSettings`, or `IUIPlaceHolderResolver`.
- Prefer reusing official Radzen components; add minimal CSS/JS only when a component can't carry the requirement.
- Menu badges / dynamic decorations use independent state (e.g. `MenuItemDecorationState`) — don't couple them to business state like the message center.
- If a Razor component needs both a `Template` and child content, pass `Template=@...` / `ChildContent=@...` explicitly to avoid Razor `RZ9996`.
- After UI changes, check the mobile layout — below `768px` the shell switches to mobile sidebar behavior.
- **`tasks/lessons.md`** records hard-won fixes (menu badges, message-state refresh, dark-theme HTML message rendering). Review it before touching UI or shared components, and append to it after a correction.
