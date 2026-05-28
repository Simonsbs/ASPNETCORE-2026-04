using Demo1.Entities;

namespace Demo1.Services.Repositories {
    public interface ILandMarkRepository {
        Task AddLandMarkAsync(int cityId, LandMark landMark, bool autoSave = true);
        Task DeleteAsync(int cityId, LandMark landMark, bool autoSave = true);
        Task<IEnumerable<LandMark>> GetForCityAsync(int cityId);
        Task<LandMark?> GetLandMarkAsync(int cityId, int landMarkId);

        Task SaveAsync();
    }
}