using Microsoft.AspNetCore.Mvc;
using Backend_dotnet.DTOs.Tour;
using Backend_dotnet.Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/tours")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // GET: api/tours
        [HttpGet]
        public async Task<IActionResult> GetAllTours()
        {
            var tours = await _tourService.GetAllToursAsync();
            return Ok(tours);
        }

        // GET: api/tours/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourById(int id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound($"Tour with ID {id} not found.");
            return Ok(tour);
        }

        // POST: api/tours
        [HttpPost]
        public async Task<IActionResult> CreateTour([FromBody] CreateTourDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdTour = await _tourService.CreateTourAsync(dto);
            return CreatedAtAction(nameof(GetTourById), new { id = createdTour.TourId }, createdTour);
        }

        // PUT: api/tours/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour(int id, [FromBody] UpdateTourDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedTour = await _tourService.UpdateTourAsync(id, dto);
                return Ok(updatedTour);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/tours/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour(int id)
        {
             var result = await _tourService.DeleteTourAsync(id);
             if (!result) return NotFound($"Tour with ID {id} not found.");
             return NoContent();
        }

        // GET: api/tours/search?query=...
        [HttpGet("search")]
        public async Task<IActionResult> SearchTours([FromQuery] string query)
        {
            var results = await _tourService.SearchToursAsync(query);
            return Ok(results);
        }

        // ==========================================
        // Category / Package Endpoints
        // ==========================================

        // GET: api/tours/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetTourDetailsByCategory(int categoryId)
        {
            var details = await _tourService.GetTourPackageDetailsAsync(categoryId);
            if (details == null) return NotFound($"Tour Category with ID {categoryId} not found.");
            return Ok(details);
        }

        // POST: api/tours/category/list
        // Bulk fetch endpoint for multiple category IDs
        [HttpPost("category/list")]
        public async Task<IActionResult> GetTourDetailsByCategories([FromBody] List<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any()) return BadRequest("Category IDs list cannot be empty.");
            
            var detailsList = await _tourService.GetTourPackagesDetailsAsync(categoryIds);
            return Ok(detailsList);
        }

        // ==========================================
        // Tour Guide Endpoints
        // ==========================================

        // GET: api/tours/{id}/guides
        [HttpGet("{id}/guides")]
        public async Task<IActionResult> GetGuidesForTour(int id)
        {
            // Verify tour exists first
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null) return NotFound($"Tour with ID {id} not found.");

            var guides = await _tourService.GetGuidesForTourAsync(id);
            return Ok(guides);
        }

        // POST: api/tours/{id}/guides
        [HttpPost("{id}/guides")]
         public async Task<IActionResult> AddGuideToTour(int id, [FromBody] CreateTourGuideDto dto)
        {
             if (!ModelState.IsValid) return BadRequest(ModelState);
             var tour = await _tourService.GetTourByIdAsync(id);
             if (tour == null) return NotFound($"Tour with ID {id} not found.");

             var createdGuide = await _tourService.AddGuideToTourAsync(id, dto);
             return Ok(createdGuide);
        }

        // PUT: api/tours/{id}/guides/{guideId}
        [HttpPut("{id}/guides/{guideId}")]
        public async Task<IActionResult> UpdateTourGuide(int id, int guideId, [FromBody] CreateTourGuideDto dto)
        {
             if (!ModelState.IsValid) return BadRequest(ModelState);
             try
             {
                 var updatedGuide = await _tourService.UpdateGuideAsync(id, guideId, dto);
                 return Ok(updatedGuide);
             }
             catch(KeyNotFoundException ex)
             {
                 return NotFound(ex.Message);
             }
        }

        // DELETE: api/tours/{id}/guides/{guideId}
        [HttpDelete("{id}/guides/{guideId}")]
        public async Task<IActionResult> RemoveGuideFromTour(int id, int guideId)
        {
            var result = await _tourService.RemoveGuideFromTourAsync(id, guideId);
             if (!result) return NotFound($"Guide with ID {guideId} not found for Tour {id}.");
             return NoContent();
        }
    }
}
