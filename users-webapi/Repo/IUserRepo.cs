using users_webapi.Models;
using users_webapi.Models.Request;

namespace users_webapi.Repo;

public interface IUserRepo
{
    Task<PagedResponse<UserInfo>> ListUsersAsync(PageInfo paging);

    Task UpdateUserAsync(string userId, UserInfo userToUpdate);

    Task<UserInfo> AddUserAsync(UserInfo userToAdd);

    void SeedTestData();
}
