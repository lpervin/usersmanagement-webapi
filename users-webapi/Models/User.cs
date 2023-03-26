using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace users_webapi.Models;

public class User
{
   public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }

    public UserInfo ToUserInfo()
    {
      return new UserInfo()
      {
          Id = this.Id.ToString(),
          Email = this.Email,
          Name = this.Name,
          Age = this.Age
      };
    }
}

public class UserInfo
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

