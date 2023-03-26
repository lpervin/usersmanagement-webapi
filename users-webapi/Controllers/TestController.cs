using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using users_webapi.Models;

namespace users_webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{

  private readonly IConfiguration _configuration;

  public TestController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpGet]
  public async Task<IActionResult> Test()
  {
    try
    {
        var connectionString = _configuration.GetConnectionString("mongodb");
        var mC = new MongoClient(connectionString);
        var _mongoDatabase = mC.GetDatabase("AppManagement");
        var _users = _mongoDatabase.GetCollection<User>("Users");
        var usersCount = await _users.CountDocumentsAsync(FilterDefinition<User>.Empty);
        return Ok(usersCount);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }

  }

}
