using Projects;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<WorkerApp>("Worker");
builder.Build().Run();