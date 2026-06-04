namespace Demo1.DTO {
    public class CityDTO {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public int LandMarkCount => LandMarks.Count();

        public IEnumerable<LandMarkDTO> LandMarks { get; set; } = new List<LandMarkDTO>();
    }

    /// <summary>
    /// This is a city with no landmark
    /// </summary>
    public class CityWithoutLandmarksDTO {
        /// <summary>
        /// the ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// the description
        /// </summary>
        public string? Description { get; set; }
    }
}
