using System;
using MongoDB.Driver;
using users_webapi.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Bogus;
using Xunit;

namespace users_webapi_tests;

public abstract class TestUsersBase
{
    protected  readonly IMongoCollection<User> _users;
    public TestUsersBase()
    {
        var (connString, dbName) = getConnectionString();
       
        var client = new MongoClient(connString);
        var dataBase = client.GetDatabase(dbName);
        _users = dataBase.GetCollection<User>("Users");
        SeedData();
    }
    
    

    private void SeedData()
    {
        var usersCount = _users.CountDocuments(new BsonDocument());
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

    private (string,string) getConnectionString()
    {
        string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
        var configuration = new ConfigurationBuilder().SetBasePath(projectPath).
            AddJsonFile("appsettings.json").Build();
        var connectionString = configuration.GetConnectionString("mongodb");
        var dbName = configuration.GetConnectionString("dbName");
        return (connectionString, dbName);
    }
}