using Demo1.DbContexts;
using Demo1.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Services.Repositories {
    public class LandMarkRepository : ILandMarkRepository {
        private readonly MyMainContext _context;

        public LandMarkRepository(MyMainContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LandMark>> GetForCityAsync(int cityId) {
            return await _context.LandMarks.Where(l => l.CityId == cityId).ToListAsync();
        }

        public async Task<LandMark?> GetLandMarkAsync(int cityId, int landMarkId) {
            return await _context.LandMarks.FirstOrDefaultAsync(l => l.CityId == cityId && l.Id == landMarkId);
        }

        public async Task AddLandMarkAsync(int cityId, LandMark landMark, bool autoSave = true) {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city != null) {
                city.LandMarks.Add(landMark);
            }

            if (autoSave) {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int cityId, LandMark landMark, bool autoSave = true) {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city != null) {
                city.LandMarks.Remove(landMark);
            }

            if (autoSave) {
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync() {
            await _context.SaveChangesAsync();
        }
    }
}
