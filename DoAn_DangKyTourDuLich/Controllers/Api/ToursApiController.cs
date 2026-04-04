using DoAn_DangKyTourDuLich.Data;
using DoAn_DangKyTourDuLich.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn_DangKyTourDuLich.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToursApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ToursApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTours()
        {
            var tours = await _context.Tours
                .Include(t => t.Category)
                .Where(t => t.IsActive)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.TourCode,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    t.DisplayPrice,
                    t.AvailableSlots,
                    t.Duration,
                    t.Destination
                })
                .ToListAsync();

            return Ok(tours);
        }

        // GET: api/ToursApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTour(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .Include(t => t.TourSchedules)
                .Where(t => t.IsActive && t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.TourCode,
                    t.DisplayPrice,
                    categoryName = t.Category != null ? t.Category.Name : null,
                    t.ImageUrl,
                    Schedules = t.TourSchedules.Select(ts => new
                    {
                        ts.DepartureDate,
                        ts.Price,
                        ts.AvailableSlots
                    })
                })
                .FirstOrDefaultAsync();

            if (tour == null)
            {
                return NotFound();
            }

            return Ok(tour);
        }
    }
}
