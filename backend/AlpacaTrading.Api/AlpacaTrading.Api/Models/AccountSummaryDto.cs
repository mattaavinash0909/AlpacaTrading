namespace AlpacaTrading.Api.Models;

public class AccountSummaryDto
{
    public string? AccountNumber { get; set; }
    public string? Status { get; set; }
    public decimal? BuyingPower { get; set; }
    public decimal? Cash { get; set; }
    public decimal? Equity { get; set; }
    public decimal? LastEquity { get; set; }
    public decimal? TodayPnL { get; set; }
    public bool IsTradingBlocked { get; set; }
}