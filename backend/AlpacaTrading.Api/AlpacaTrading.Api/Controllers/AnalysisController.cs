using AlpacaTrading.Api.Models;
using AlpacaTrading.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlpacaTrading.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IStockAnalysisService _stockAnalysisService;

    public AnalysisController(IStockAnalysisService stockAnalysisService)
    {
        _stockAnalysisService = stockAnalysisService;
    }

    [HttpGet("{symbol}")]
    [ProducesResponseType(typeof(StockAnalysisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AnalyzeStock(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Symbol is required.");
        }

        var result = await _stockAnalysisService.AnalyzeAsync(symbol);
        return Ok(result);
    }
}