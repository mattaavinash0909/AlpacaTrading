namespace AlpacaTrading.Api.Models
{
    public class StockAnalysisDto
    {
        public string? Symbol { get; set; }
        public string? Status { get; set; }
        public string? DataSource { get; set; }
        public decimal? LatestClose { get; set; }
        public decimal? Sma20 { get; set; }
        public decimal? Sma50 { get; set; }
        public long? Volume { get; set; }
        public string? Signal { get; set; }
        public string? Reason { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public string? Message { get; set; }
    }
}
