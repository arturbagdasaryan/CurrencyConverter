using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers
{
    [ApiController]
    [Route("api/v1/rates")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ICurrencyConversionService _conversionService;
        private readonly ILogger<ExchangeRatesController> _logger;

        public ExchangeRatesController(
            ICurrencyConversionService conversionService,
            ILogger<ExchangeRatesController> logger)
        {
            _conversionService = conversionService;
            _logger = logger;
        }

        // GET: /api/v1/rates/latest?baseCurrency=EUR
        [HttpGet("latest")]
        [Authorize]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            var rates = await _conversionService.GetLatestRatesAsync(baseCurrency);
            return Ok(rates);
        }

        // POST: /api/v1/rates/convert
        [HttpPost("convert")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionRequest request)
        {
            // Exclude currencies: TRY, PLN, THB, MXN
            var excluded = new List<string> { "TRY", "PLN", "THB", "MXN" };
            if (excluded.Contains(request.SourceCurrency.ToUpper()) || excluded.Contains(request.TargetCurrency.ToUpper()))
            {
                return BadRequest("Conversion involving one or more excluded currencies is not allowed.");
            }

            var convertedAmount = await _conversionService.ConvertCurrencyAsync(request);
            return Ok(new { convertedAmount });
        }

        // GET: /api/v1/rates/historical?baseCurrency=EUR&startDate=2020-01-01&endDate=2020-01-31&pageNumber=1&pageSize=10
        [HttpGet("historical")]
        [Authorize]
        public async Task<IActionResult> GetHistoricalRates(
            [FromQuery] string baseCurrency,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var historicalData = await _conversionService.GetHistoricalRatesAsync(
                baseCurrency, startDate, endDate, pageNumber, pageSize);
            return Ok(historicalData);
        }
    }
}
