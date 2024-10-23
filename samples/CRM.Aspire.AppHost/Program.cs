var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CRM_Blazor_Web>("crm-blazor-web");

builder.Build().Run();
