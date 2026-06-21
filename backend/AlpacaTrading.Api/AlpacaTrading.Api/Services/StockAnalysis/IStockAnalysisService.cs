using AlpacaTrading.Api.Models;

namespace AlpacaTrading.Api.Services;


public interface IStockAnalysisService
{
    Task<StockAnalysisDto> AnalyzeAsync(string symbol);
}
