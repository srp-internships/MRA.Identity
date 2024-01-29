﻿using System.Net;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using MRA.Identity.Application.Contract.User.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Jobs.Application.IntegrationTests.Users.Query;

public class GetUserByKeyQueryTests : BaseTest
{
    private static readonly Guid Id = Guid.NewGuid();
    private static readonly string UserName = "user";

    [SetUp]
    public async Task SetUp()
    {
        await AddUser();
    }

    private static IEnumerable<string> GetTestCases()
    {
        yield return Id.ToString();
        yield return UserName;
    }

    [Test]
    [TestCaseSource(nameof(GetTestCases))]
    public async Task GetUserByKey_ReturnUserResponse(string key)
    {
        await AddAuthorizationAsync();
        var response = await _client.GetAsync($"api/User/{key}");
        response.EnsureSuccessStatusCode();

        Assert.That((await response.Content.ReadFromJsonAsync<UserResponse>()) is not null);
    }

    [Test]
    public async Task GetUserByKey_Return_NotFoundException()
    {
        await AddAuthorizationAsync();
        var response = await _client.GetAsync($"api/User/{Guid.NewGuid()}");
        Assert.That(response.StatusCode == HttpStatusCode.NotFound);
    }

    [Test]
    [TestCase("")]
    [TestCase("Reviewer")]
    public async Task GetUserByKey_Return_Forbidden(string role)
    {
        if (role.IsNullOrEmpty())
            await AddApplicantAuthorizationAsync();
        else
            await AddReviewerAuthorizationAsync();


        var response = await _client.GetAsync($"api/User/{UserName}");
        Assert.That(response.StatusCode == HttpStatusCode.Forbidden);
    }

    private async Task AddUser()
    {
        var user = await GetEntity<ApplicationUser>(a => a.UserName == UserName);
        if (user != null) return;
        user = new ApplicationUser()
        {
            Id = Id,
            UserName = UserName
        };
        await AddEntity(user);
    }
}