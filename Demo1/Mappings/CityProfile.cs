using AutoMapper;
using Demo1.DTO;
using Demo1.Entities;

namespace Demo1.Mappings {
    public class CityProfile : Profile {
        public CityProfile() {
            CreateMap<City, CityDTO>();
            CreateMap<City, CityWithoutLandmarksDTO>();
            CreateMap<LandMark, LandMarkDTO>();
            CreateMap<LandMarkForCreateDTO, LandMark>();
            CreateMap<LandMarkForUpdateDTO, LandMark>();
            CreateMap<LandMark, LandMarkForUpdateDTO>();
        }
    }
}
