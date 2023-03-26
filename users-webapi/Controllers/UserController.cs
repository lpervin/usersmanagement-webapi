using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using users_webapi.Models;
using users_webapi.Models.Request;
using users_webapi.Repo;

namespace users_webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;

    public UserController(IUserRepo repo, IConfiguration configuration)
    {
        _userRepo = repo;
        _configuration = configuration;
    }


    [HttpGet]
    public async Task<IActionResult> ListUser([FromQuery] PageInfo paging)
    {
        var results = await _userRepo.ListUsersAsync(paging);
        return Ok(results);
    }

    [HttpPut]
    [Route("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId,UserInfo userToUpdate)
    {
      await _userRepo.UpdateUserAsync(userId, userToUpdate);
      return Ok(userToUpdate);
    }
}
