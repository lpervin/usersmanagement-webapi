using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using users_webapi.Models;
using Xunit;

namespace users_webapi_tests;

public class QueryTestUsers : TestUsersBase
{
    public QueryTestUsers() : base()
    {

    }
    [Fact]
    public async  Task CanReadUsers()
    {
        //var users = _users.Find(u => u.Id != null);
        var usersCount = _users.CountDocuments(new BsonDocument());
        Assert.NotEqual(0, usersCount);
    }


    [Fact]
    public async Task CanFindUsers()
    {
        var lenny = (await _users.FindAsync(u => u.Name == "Lenny")).FirstOrDefault();
        Assert.NotNull(lenny);
        var id = lenny.Id;
        Assert.NotNull(id);
    }

    [Fact]
    public async Task CanListUser()
    {
        var sort = Builders<User>.Sort.Descending(u => u.Name);

        var query = _users.Find(x => x.Name != null);
        var totalTask =  query.CountDocumentsAsync();
        var itemsTask = query.Skip(0).Limit(1002).Sort(sort).ToListAsync();
        await Task.WhenAll(new List<Task>() { totalTask, itemsTask});
        Assert.Equal(totalTask.Result, 1002);
        foreach (var user in itemsTask.Result)
        {
            Assert.NotNull(user.Name);
           // System.Diagnostics.Debug.WriteLine(user.Name);
           Console.WriteLine(user.Name);
        }
    }

    [Theory]
    [InlineData(1, 5)]
    public async Task CanPageUsers(int page, int pageSize)
    {

        var countFacet = AggregateFacet.Create("count",
                        PipelineDefinition<User, AggregateCountResult>.Create(
            new[] {PipelineStageDefinitionBuilder.Count<User>()}
        ));

        var dataFacet = AggregateFacet.Create("data",
                        PipelineDefinition<User,User>.Create(new []
                        {
                            PipelineStageDefinitionBuilder.Sort(Builders<User>.Sort.Ascending(x => x.Id)),
                            PipelineStageDefinitionBuilder.Skip<User>((page - 1) * pageSize),
                            PipelineStageDefinitionBuilder.Limit<User>(pageSize)}
                        ));
        var filter = Builders<User>.Filter.Empty;
        var aggregation = await _users.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()?.FirstOrDefault()?.Count ?? 0;
        var totalPages = (int) count / pageSize;
        Assert.NotEqual(0, count);

        var pageData = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<User>();

        Assert.Equal(pageData.Count, pageSize);
    }


}
