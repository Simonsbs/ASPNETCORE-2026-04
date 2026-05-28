using Demo1.DbContexts;
using Demo1.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Services.Repositories {
    public class CityRepository : ICityRepository {
        const int maxPageSize = 10;
        
        private readonly MyMainContext _context;

        public CityRepository(MyMainContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<bool> ExistsAsync(int id) {
            return _context.Cities.AnyAsync(c => c.Id == id);
        }

        public async Task<(ICollection<City> Cities, PagingMetadata PagingMetadata)> GetCitiesAsync(
            string? name, 
            string? search,
            int? pageNumber = 1,
            int? pageSize = maxPageSize
            ) {

            
            if (pageSize > maxPageSize) {
                pageSize = maxPageSize; 
            }

            var cities = _context.Cities.AsQueryable();

            if (!string.IsNullOrEmpty(name)) {
                cities = cities.Where(c => c.Name.Equals(name.Trim()));
            }

            if (!string.IsNullOrEmpty(search)) {
                search = search.Trim();
                cities = cities.Where(c => c.Name.Contains(search) || 
                                        (c.Description != null && c.Description.Contains(search)));
            }

            var totalItemCount = await cities.CountAsync();

            var metadata = new PagingMetadata(totalItemCount, 
                                                pageSize ?? maxPageSize, 
                                                pageNumber ?? 1);

            cities = cities.
                Skip(((pageNumber ?? 1) - 1) * (pageSize ?? maxPageSize)).
                Take(pageSize ?? maxPageSize);

            return (await cities.OrderByDescending(c => c.Name).ToListAsync(), metadata);
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
        Task<(ICollection<City> Cities, PagingMetadata PagingMetadata)> GetCitiesAsync(
            string? name, 
            string? search, 
            int? pageNumber, 
            int? pageSize);

        Task<bool> ExistsAsync(int id);

        Task<City?> GetCityAsync(int id, bool includeLandMarks = false);
    }
}
