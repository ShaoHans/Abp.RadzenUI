var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis", 6379).WithImageTag("7.2");
var username = builder.AddParameter("pguser");
var password = builder.AddParameter("pgpwd");
var db = builder.AddPostgres("postgres", username, password, 5432).WithImageTag("16.2");

var migrator = builder
    .AddProject<Projects.CRM_DbMigrator>("crm-db-migrator")
    .WithReference(redis)
    .WithReference(db);

builder
    .AddProject<Projects.CRM_Blazor_Web>("crm-blazor-web")
    .WithReference(migrator)
    .WithReference(redis)
    .WithReference(db);

builder.Build().Run();
