using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_dotnet.DTOs.Tour;
using Backend_dotnet.Models.Entities;
using Backend_dotnet.Repositories.Interfaces;
using Backend_dotnet.Services.Interfaces;

namespace Backend_dotnet.Services.Implementations
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        public TourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public async Task<TourDetailsDto?> GetTourPackageDetailsAsync(int categoryId)
        {
            var category = await _tourRepository.GetCategoryWithDetailsAsync(categoryId);
            if (category == null) return null;

            return MapToTourDetailsDto(category);
        }

        public async Task<IEnumerable<TourDetailsDto>> GetTourPackagesDetailsAsync(IEnumerable<int> categoryIds)
        {
            var categories = await _tourRepository.GetCategoriesWithDetailsAsync(categoryIds);
            return categories.Select(MapToTourDetailsDto);
        }

        public async Task<IEnumerable<TourDto>> GetAllToursAsync()
        {
            var tours = await _tourRepository.GetAllToursAsync();
            return tours.Select(MapToTourDto);
        }

        public async Task<TourDto?> GetTourByIdAsync(int id)
        {
            var tour = await _tourRepository.GetTourByIdAsync(id);
            return tour == null ? null : MapToTourDto(tour);
        }

        public async Task<TourDto> CreateTourAsync(CreateTourDto dto)
        {
            var tour = new tour_master
            {
                category_id = dto.CategoryId,
                departure_id = dto.DepartureId
            };
            
            var createdTour = await _tourRepository.AddTourAsync(tour);
            var fullTour = await _tourRepository.GetTourByIdAsync(createdTour.tour_id);
            return MapToTourDto(fullTour!);
        }

        public async Task<TourDto> UpdateTourAsync(int id, UpdateTourDto dto)
        {
             var tour = await _tourRepository.GetTourByIdAsync(id);
             if (tour == null) throw new KeyNotFoundException("Tour not found");

             tour.category_id = dto.CategoryId;
             tour.departure_id = dto.DepartureId;

             await _tourRepository.UpdateTourAsync(tour);
             
             var fullTour = await _tourRepository.GetTourByIdAsync(id);
             return MapToTourDto(fullTour!);
        }

        public async Task<bool> DeleteTourAsync(int id)
        {
            return await _tourRepository.DeleteTourAsync(id);
        }

        public async Task<IEnumerable<TourDto>> SearchToursAsync(string query)
        {
            var tours = await _tourRepository.SearchToursAsync(query);
            return tours.Select(MapToTourDto);
        }

        public async Task<IEnumerable<TourGuideDto>> GetGuidesForTourAsync(int tourId)
        {
            var guides = await _tourRepository.GetGuidesByTourIdAsync(tourId);
            return guides.Select(g => new TourGuideDto
            {
                TourGuideId = g.tour_guide_id,
                Name = g.name,
                Email = g.email,
                Phone = g.phone
            });
        }

        public async Task<TourGuideDto> AddGuideToTourAsync(int tourId, CreateTourGuideDto dto)
        {
            var guide = new tour_guide
            {
                tour_id = tourId,
                name = dto.Name,
                email = dto.Email,
                phone = dto.Phone
            };

            var createdGuide = await _tourRepository.AddTourGuideAsync(guide);
            return new TourGuideDto
            {
                TourGuideId = createdGuide.tour_guide_id,
                Name = createdGuide.name,
                Email = createdGuide.email,
                Phone = createdGuide.phone
            };
        }

        public async Task<bool> RemoveGuideFromTourAsync(int tourId, int guideId)
        {
            var guide = await _tourRepository.GetTourGuideByIdAsync(guideId);
            if (guide == null || guide.tour_id != tourId) return false;
            
            return await _tourRepository.DeleteTourGuideAsync(guideId);
        }

        public async Task<TourGuideDto> UpdateGuideAsync(int tourId, int guideId, CreateTourGuideDto dto)
        {
             var guide = await _tourRepository.GetTourGuideByIdAsync(guideId);
             if (guide == null || guide.tour_id != tourId) throw new KeyNotFoundException("Guide not found or does not belong to this tour");

             guide.name = dto.Name;
             guide.email = dto.Email;
             guide.phone = dto.Phone;

             await _tourRepository.UpdateTourGuideAsync(guide);

             return new TourGuideDto
             {
                 TourGuideId = guide.tour_guide_id,
                 Name = guide.name,
                 Email = guide.email,
                 Phone = guide.phone
             };
        }

        private TourDetailsDto MapToTourDetailsDto(category_master category)
        {
            var dto = new TourDetailsDto
            {
                CategoryId = category.category_id,
                CategoryName = category.category_name,
                CatCode = category.cat_code,
                SubCatCode = category.subcat_code,
                ImagePath = category.image_path
            };

            // Map Itineraries (Using updated property names)
            dto.Itineraries = category.itinerary_master.Select(i => new ItineraryDto
            {
                ItineraryId = i.itinerary_id,
                DayNo = i.day_no,
                ItineraryDetail = i.itinerary_detail,
                DayWiseImage = i.day_wise_image
            }).OrderBy(i => i.DayNo).ToList();

            // Map Costs
            dto.Costs = category.cost_master.Select(c => new CostDto
            {
                CostId = c.cost_id,
                CategoryId = c.category_id,
                SinglePersonCost = c.single_person_cost,
                ExtraPersonCost = c.extra_person_cost,
                ChildWithBedCost = c.child_with_bed_cost,
                ChildWithoutBedCost = c.child_without_bed_cost,
                ValidFrom = c.valid_from,
                ValidTo = c.valid_to
            }).ToList();

            // Map Scheduled Tours
            dto.ScheduledTours = category.tour_master.Select(t => new ScheduledTourDto
            {
                TourId = t.tour_id,
                DepartureId = t.departure_id,
                DepartDate = t.departure.depart_date,
                EndDate = t.departure.end_date,
                NoOfDays = t.departure.no_of_days,
                Guides = t.tour_guide.Select(g => new TourGuideDto
                {
                    TourGuideId = g.tour_guide_id,
                    Name = g.name,
                    Email = g.email,
                    Phone = g.phone
                }).ToList()
            }).ToList();

            // Map All Available Departures
            dto.Departures = category.departure_master.Select(d => new DepartureDto
            {
                DepartureId = d.departure_id,
                DepartDate = d.depart_date,
                EndDate = d.end_date,
                NoOfDays = d.no_of_days
            }).OrderBy(d => d.DepartDate).ToList();

            return dto;
        }

        private TourDto MapToTourDto(tour_master tour)
        {
            return new TourDto
            {
                TourId = tour.tour_id,
                CategoryId = tour.category_id,
                CategoryName = tour.category != null ? tour.category.category_name : "Unknown",
                DepartureId = tour.departure_id,
                DepartDate = tour.departure != null ? tour.departure.depart_date : default,
                EndDate = tour.departure != null ? tour.departure.end_date : default,
                NoOfDays = tour.departure != null ? tour.departure.no_of_days : 0,
                AvailableDepartures = tour.category?.departure_master.Select(d => new DepartureDto
                {
                    DepartureId = d.departure_id,
                    DepartDate = d.depart_date,
                    EndDate = d.end_date,
                    NoOfDays = d.no_of_days
                }).OrderBy(d => d.DepartDate).ToList() ?? new List<DepartureDto>()
            };
        }
    }
}
