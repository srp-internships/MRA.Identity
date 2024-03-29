﻿using MRA.Configurations.Common.Interfaces.Services;

namespace MRA.Jobs.Application.IntegrationTests.Services;

public static class TestSmsSandbox
{
    public static string PhoneNumber { get; set; }
    public static string Text { get; set; }
}
public class TestSmsSenderService : ISmsService
{
    public Task<bool> SendSmsAsync(string phoneNumber, string text)
    {
        TestSmsSandbox.PhoneNumber = phoneNumber;
        TestSmsSandbox.Text = text;

        return Task.FromResult(true);
    }
}
