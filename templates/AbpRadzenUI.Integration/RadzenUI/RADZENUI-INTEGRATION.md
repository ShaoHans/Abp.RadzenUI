# RadzenUI Integration Checklist

This template added the common integration files for Abp RadzenUI.

## Files Added

- `RadzenUI/RadzenUiTemplateRadzenUIIntegrationExtensions.cs`
- `Menus/RadzenUiTemplateMenus.cs`
- `Menus/RadzenUiTemplateMenuContributor.cs`
- `Components/Pages/Home.razor`

## Required Manual Changes

1. Add the UI package to the Blazor Server web project:

   ```powershell
   dotnet add package AbpRadzen.Blazor.Server.UI
   ```

2. Add the module dependency to your web module:

   ```csharp
   [DependsOn(
       typeof(AbpRadzenUIModule)
   )]
   ```

3. Add the generated integration helper in `ConfigureServices`:

   ```csharp
   context.Services.AddRadzenUIIntegration<Home, YourResource, RadzenUiTemplateMenuContributor>();
   ```

4. Add these using statements to the web module as needed:

   ```csharp
   using Abp.RadzenUI;
   using RadzenUiTemplateNamespace.Components.Pages;
   using RadzenUiTemplateNamespace.Menus;
   using RadzenUiTemplateNamespace.RadzenUI;
   ```

5. Call RadzenUI at the end of `OnApplicationInitialization`:

   ```csharp
   app.UseRadzenUI();
   ```

6. Remove or disable the previous Blazor theme package and default pages if they conflict with the generated home page.
