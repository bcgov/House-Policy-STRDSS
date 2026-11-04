using server.AppHost.Resources;

var builder = DistributedApplication.CreateBuilder(args);

var database = new DatabaseResources(builder);

var api = new ApiResources(builder, database).Configure();
var hangfire = new HangfireResources(builder, database).Configure();

var frontend = new FrontendResources(builder).Configure(api);

builder.Build().Run();
