using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using users_webapi.Models;
using Xunit;

namespace users_webapi_tests;

public class UpdateUsersTest : TestUsersBase
{
  public UpdateUsersTest()
  {

  }

  [Fact]
  public async Task UpdateOneUser()
  {
    var userid = ObjectId.Parse("63b49696369e4bd1a8d470fe");
    var userQuery = Builders<User>.Filter.Eq("_id", userid);
    var userEntity = (await _users.FindAsync(userQuery)).FirstOrDefault();
     userEntity.Email = "update@email.com";
    var result = await _users.ReplaceOneAsync(userQuery, userEntity);

    Assert.NotNull(result);
  }

}
