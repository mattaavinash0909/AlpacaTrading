using AlpacaTrading.Api.Models;
using AlpacaTrading.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlpacaTrading.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAlpacaAccountService _alpacaAccountService;

    public AccountController(IAlpacaAccountService alpacaAccountService)
    {
        _alpacaAccountService = alpacaAccountService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(AccountSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccount()
    {
        var account = await _alpacaAccountService.GetAccountWholeDetailsAsync();
        return Ok(account);
    }
}