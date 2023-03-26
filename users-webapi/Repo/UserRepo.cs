using System.Linq.Expressions;
using Bogus;
using MongoDB.Bson;
using MongoDB.Driver;
using users_webapi.Models;
using users_webapi.Models.Request;

namespace users_webapi.Repo;

public class UserRepo : IUserRepo
{
    private readonly IMongoDatabase _mongoDatabase;
    private IMongoCollection<User> _users;

    public  UserRepo(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongodb");
        var mongoClient = new MongoClient(connectionString);
        _mongoDatabase = mongoClient.GetDatabase("AppManagement");
         SeedTestData();
    }

    public async Task<PagedResponse<UserInfo>> ListUsersAsync(PageInfo paging)
    {
        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<User, AggregateCountResult>.Create(
                new[] {PipelineStageDefinitionBuilder.Count<User>()}
            ));

        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<User,User>.Create(new []
                {
                    PipelineStageDefinitionBuilder.Sort(BuildSortingExp(paging.SortByName, paging.Order)),
                    PipelineStageDefinitionBuilder.Skip<User>((paging.PageNumber - 1) * paging.PageSize),
                    PipelineStageDefinitionBuilder.Limit<User>(paging.PageSize)}
            ));
        var filter = Builders<User>.Filter.Empty;
        var aggregation = await _users.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()?.FirstOrDefault()?.Count ?? 0;
        var totalPages = (double) count /  (double) paging.PageSize;


        var pageData = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<User>();

        return new PagedResponse<UserInfo>()
        {
            TotalRecordsCount = count,
            PageRecordsCount = pageData.Count,
            PageCount = (int) Math.Ceiling(totalPages),
            PageSize = paging.PageSize,
            OrderBy = new SortInfo(paging.SortByName, (paging.Order ?? SortOrder.Asc)),
            PageNumber = paging.PageNumber,
            PageData = pageData.Select(p => p.ToUserInfo()).ToList()
        };
    }

    public async Task UpdateUserAsync(string userId, UserInfo userToUpdate)
    {
      var objUserId =  ObjectId.Parse(userId);
      var userQuery = Builders<User>.Filter.Eq("_id", objUserId);
      var userEntity = (await _users.FindAsync(userQuery)).FirstOrDefault();
      if (userEntity == null)
        throw new KeyNotFoundException(userId);

      userEntity.Name = userToUpdate.Name;
      userEntity.Email = userToUpdate.Email;
      userEntity.Age = userToUpdate.Age;

      var result = await _users.ReplaceOneAsync(userQuery, userEntity);
      if (result.ModifiedCount < 1)
        throw new ApplicationException("Updated Failed");
    }

    public void  SeedTestData()
    {
      _users = _mongoDatabase.GetCollection<User>("Users");
      var usersCount =  _users.CountDocuments(FilterDefinition<User>.Empty);
      if (usersCount>2)
        return;

      var userFaker = new Faker<User>()
        .RuleFor(u => u.Name, f => f.Name.FirstName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Age, f => f.Random.Number(18, 100));

      // Generate 1000 random users
      var users = userFaker.Generate(1000);
       _users.InsertMany(users);


    }

    private SortDefinition<User> BuildSortingExp(string? pagingSortBy, SortOrder? pagingOrder)
    {
        Expression<Func<User, object>>? orderExpr = null;// (x => x.Id);

        switch (pagingSortBy)
        {
            case "Id":
                orderExpr = (x => x.Id);
                break;
            case "Name":
                orderExpr = (x => x.Name);
                break;
            case "Age":
                orderExpr = (x => x.Age);
                break;
            case "Email":
                orderExpr = (x => x.Email);
                break;
            default:
                throw new ArgumentException($"Never heard of {pagingSortBy}");
        }
        switch (pagingOrder)
        {
            case SortOrder.Asc:
                return Builders<User>.Sort.Ascending(orderExpr);
            case SortOrder.Desc:
                return Builders<User>.Sort.Descending(orderExpr);
            default:
                return Builders<User>.Sort.Ascending(x => x.Id);
        }
    }
}
