
using Backend_dotnet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        // GET: api/search/name/{query}
        [HttpGet("name/{query}")]
        public async Task<IActionResult> SearchByName(string query)
        {
            var result = await _searchService.GetCategoryIdsByNameAsync(query);
            return Ok(result);
        }

        // GET: api/search/date?from=yyyy-mm-dd&to=yyyy-mm-dd
        [HttpGet("date")]
        public async Task<IActionResult> SearchByDateRange([FromQuery] string from, [FromQuery] string to)
        {
            if (!DateOnly.TryParse(from, out DateOnly fromDate) || !DateOnly.TryParse(to, out DateOnly toDate))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd");
            }

            var result = await _searchService.GetCategoryIdsByDateRangeAsync(fromDate, toDate);
            return Ok(result);
        }

        // GET: api/search/cost/{maxCost}
        [HttpGet("cost/{maxCost}")]
        public async Task<IActionResult> SearchByMaxCost(decimal maxCost)
        {
            var result = await _searchService.GetCategoryIdsByMaxCostAsync(maxCost);
            return Ok(result);
        }
    }
}
