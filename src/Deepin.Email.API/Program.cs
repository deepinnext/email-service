using Deepin.Email.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:5001");
builder.AddApplicationService();

var app = builder.Build();
app.UseApplicationService();
app.Run();
