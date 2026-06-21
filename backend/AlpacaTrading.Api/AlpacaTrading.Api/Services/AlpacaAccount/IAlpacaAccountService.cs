using Alpaca.Markets;
using AlpacaTrading.Api.Models;

namespace AlpacaTrading.Api.Services;

public interface IAlpacaAccountService
{
    Task<AccountSummaryDto> GetAccountAsync();
    Task<IAccount> GetAccountWholeDetailsAsync();
}