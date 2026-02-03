using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_dotnet.Data;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Repositories.Implementations
{
    public class TourRepository : ITourRepository
    {
        private readonly AppDbContext _context;

        public TourRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<category_master?> GetCategoryWithDetailsAsync(int categoryId)
        {
            return await _context.category_master
                .Include(c => c.itinerary_master)
                .Include(c => c.cost_master)
                .Include(c => c.departure_master)
                .Include(c => c.tour_master)
                    .ThenInclude(t => t.departure)
                .Include(c => c.tour_master)
                    .ThenInclude(t => t.tour_guide)
                .FirstOrDefaultAsync(c => c.category_id == categoryId);
        }

        public async Task<IEnumerable<category_master>> GetCategoriesWithDetailsAsync(IEnumerable<int> categoryIds)
        {
             return await _context.category_master
                .Include(c => c.itinerary_master)
                .Include(c => c.cost_master)
                .Include(c => c.departure_master)
                .Include(c => c.tour_master)
                    .ThenInclude(t => t.departure)
                .Include(c => c.tour_master)
                    .ThenInclude(t => t.tour_guide)
                .Where(c => categoryIds.Contains(c.category_id))
                .ToListAsync();
        }

        public async Task<IEnumerable<tour_master>> GetAllToursAsync()
        {
            return await _context.tour_master
                .Include(t => t.category)
                    .ThenInclude(c => c.departure_master)
                .Include(t => t.departure)
                .Include(t => t.tour_guide)
                .ToListAsync();
        }

        public async Task<tour_master?> GetTourByIdAsync(int id)
        {
            return await _context.tour_master
                .Include(t => t.category)
                    .ThenInclude(c => c.departure_master)
                .Include(t => t.departure)
                .Include(t => t.tour_guide)
                .FirstOrDefaultAsync(t => t.tour_id == id);
        }

        public async Task<tour_master> AddTourAsync(tour_master tour)
        {
            _context.tour_master.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task<tour_master> UpdateTourAsync(tour_master tour)
        {
            _context.tour_master.Update(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task<bool> DeleteTourAsync(int id)
        {
            var tour = await _context.tour_master.FindAsync(id);
            if (tour == null) return false;

            _context.tour_master.Remove(tour);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<tour_master>> SearchToursAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllToursAsync();

            query = query.ToLower();
            return await _context.tour_master
                .Include(t => t.category)
                .Include(t => t.departure)
                .Where(t => t.category.category_name.ToLower().Contains(query) ||
                            t.category.cat_code.ToLower().Contains(query))
                .ToListAsync();
        }

        public async Task<IEnumerable<tour_master>> GetToursByCategoryAsync(int categoryId)
        {
             return await _context.tour_master
                .Include(t => t.category)
                .Include(t => t.departure)
                .Include(t => t.tour_guide)
                .Where(t => t.category_id == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<tour_guide>> GetGuidesByTourIdAsync(int tourId)
        {
            return await _context.tour_guide
                .Where(tg => tg.tour_id == tourId)
                .ToListAsync();
        }
        
        public async Task<tour_guide?> GetTourGuideByIdAsync(int guideId)
        {
            return await _context.tour_guide.FindAsync(guideId);
        }

        public async Task<tour_guide> AddTourGuideAsync(tour_guide guide)
        {
            _context.tour_guide.Add(guide);
            await _context.SaveChangesAsync();
            return guide;
        }

        public async Task<tour_guide> UpdateTourGuideAsync(tour_guide guide)
        {
            _context.tour_guide.Update(guide);
            await _context.SaveChangesAsync();
            return guide;
        }

        public async Task<bool> DeleteTourGuideAsync(int guideId)
        {
             var guide = await _context.tour_guide.FindAsync(guideId);
            if (guide == null) return false;

            _context.tour_guide.Remove(guide);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
