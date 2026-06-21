using Alpaca.Markets;
using AlpacaTrading.Api.Models;

namespace AlpacaTrading.Api.Services;

public class AlpacaAccountService : IAlpacaAccountService
{
    private readonly IConfiguration _configuration;

    public AlpacaAccountService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AccountSummaryDto> GetAccountAsync()
    {
        var apiKey = _configuration["Alpaca:ApiKey"];
        var apiSecret = _configuration["Alpaca:ApiSecret"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Alpaca API key or secret is missing.");
        }

        var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));

        var account = await client.GetAccountAsync();

        return new AccountSummaryDto
        {
            AccountNumber = account.AccountNumber,
            Status = account.Status.ToString(),
            BuyingPower = account.BuyingPower,
            //Cash = account.Cash,
            Equity = account.Equity,
            LastEquity = account.LastEquity,
            TodayPnL = account.Equity - account.LastEquity,
            IsTradingBlocked = account.IsTradingBlocked
        };
    }
    public async Task<IAccount> GetAccountWholeDetailsAsync()
    {
        var apiKey = _configuration["Alpaca:ApiKey"];
        var apiSecret = _configuration["Alpaca:ApiSecret"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Alpaca API key or secret is missing.");
        }

        var client = Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(apiKey, apiSecret));

        var account = await client.GetAccountAsync();

        return account;
    }
}