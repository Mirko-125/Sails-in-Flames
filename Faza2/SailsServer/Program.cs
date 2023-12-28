using SailsServer.Database;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton(new DatabaseConfig { Name = "Data Source=Sails.sqlite" });

builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
builder.Services.AddSingleton<IIgraciProvider, IgraciProvider>();
builder.Services.AddSingleton<IIgraciRepository, IgraciRepository>();
builder.Services.AddSingleton<IIgreProvider, IgreProvider>();
builder.Services.AddSingleton<IIgreRepository, IgreRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.GetService<IDatabaseBootstrap>()!.Setup();

app.Run();
