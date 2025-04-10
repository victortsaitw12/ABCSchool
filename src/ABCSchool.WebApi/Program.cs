using ABCSchool.Infrastructure;
using ABCSchool.Application;
using ABCSchool.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));
builder.Services.AddApplicationServices();

var app = builder.Build();

// Database Seeder
await app.Services.AddDatabaseInitializerAsync();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseInfrastructure();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();