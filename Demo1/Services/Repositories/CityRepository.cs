using Demo1.DbContexts;
using Demo1.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Services.Repositories {
    public class CityRepository : ICityRepository {
        private readonly MyMainContext _context;

        public CityRepository(MyMainContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> ExistsAsync(int id) {
            return _context.Cities.AnyAsync(c => c.Id == id);
        }

        public async Task<ICollection<City>> GetCitiesAsync(string? name) {
            var cities = _context.Cities.AsQueryable();
            
            if (!string.IsNullOrEmpty(name)) {
                cities = cities.Where(c => c.Name.Equals(name.Trim()));
            }

            return await cities.OrderByDescending(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int id, bool includeLandMarks) {
            if (includeLandMarks) {
                return await _context.
                    Cities.
                    Include(c => c.LandMarks).
                    FirstOrDefaultAsync(c => c.Id == id);
            }

            return await _context.
                    Cities.
                    FirstOrDefaultAsync(c => c.Id == id);
        }
    }

    public interface ICityRepository {
        Task<ICollection<City>> GetCitiesAsync(string? name);

        Task<bool> ExistsAsync(int id);

        Task<City?> GetCityAsync(int id, bool includeLandMarks = false);
    }
}
