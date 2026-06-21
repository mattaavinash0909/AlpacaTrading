using System.Diagnostics;
using System.Text.Json;
using AlpacaTrading.Api.Models;

namespace AlpacaTrading.Api.Services;

public class StockAnalysisService : IStockAnalysisService
{
    private readonly IConfiguration _configuration;

    public StockAnalysisService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<StockAnalysisDto> AnalyzeAsync(string symbol)
    {
        var pythonExe = _configuration["Python:ExecutablePath"];
        var scriptPath = _configuration["Python:AnalysisScriptPath"];

        var alpacaApiKey = _configuration["Alpaca:ApiKey"];
        var alpacaApiSecret = _configuration["Alpaca:ApiSecret"];

        if (string.IsNullOrWhiteSpace(pythonExe))
        {
            throw new InvalidOperationException("Python executable path is missing.");
        }

        if (string.IsNullOrWhiteSpace(scriptPath))
        {
            throw new InvalidOperationException("Python analysis script path is missing.");
        }

        if (string.IsNullOrWhiteSpace(alpacaApiKey) || string.IsNullOrWhiteSpace(alpacaApiSecret))
        {
            throw new InvalidOperationException("Alpaca API key or secret is missing in C# configuration/User Secrets.");
        }

        if (!File.Exists(pythonExe))
        {
            throw new FileNotFoundException($"Python executable not found: {pythonExe}");
        }

        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Python script not found: {scriptPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = $"\"{scriptPath}\" {symbol}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.Environment["APCA_API_KEY_ID"] = alpacaApiKey;
        startInfo.Environment["APCA_API_SECRET_KEY"] = alpacaApiSecret;

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            return new StockAnalysisDto
            {
                Symbol = symbol.ToUpper(),
                Status = "Error",
                Message = error
            };
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<StockAnalysisDto>(output, options);

        return result ?? new StockAnalysisDto
        {
            Symbol = symbol.ToUpper(),
            Status = "Error",
            Message = "Unable to parse Python response."
        };
    }
}