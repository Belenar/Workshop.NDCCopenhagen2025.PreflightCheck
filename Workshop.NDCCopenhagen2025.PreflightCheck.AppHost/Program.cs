using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("marten-db")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.Workshop_NDCCopenhagen2025_PreflightCheck_ApiService>("apiservice")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
