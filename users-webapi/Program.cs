using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using users_webapi.Repo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddCors();


var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var connectionString = config.GetConnectionString("mongodb");
var dbName = config.GetConnectionString("dbName");

//Register data Services and Repos
// Register the MongoDB client
//builder.Services.AddSingleton(new MongoClient(connectionString));
// Register the MongoDB database
//builder.Services.AddSingleton<IMongoDatabase>(sp =>    sp.GetService<MongoClient>().GetDatabase(dbName));
//register repo
builder.Services.AddTransient<IUserRepo, UserRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// global cors policy
app.UseCors(x => x
  .AllowAnyMethod()
  .AllowAnyHeader()
  .SetIsOriginAllowed(origin => true) // allow any origin
  .AllowCredentials()); // allow credentials

app.UseHttpsRedirection();

// app.UseFileServer(new FileServerOptions()
// {
//   FileProvider = new PhysicalFileProvider(
//       Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
//         RequestPath = "",
//         EnableDefaultFiles = true
// });

//app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
