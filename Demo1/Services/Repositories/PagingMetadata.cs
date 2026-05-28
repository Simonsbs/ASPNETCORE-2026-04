namespace Demo1.Services.Repositories {
    public class PagingMetadata {
        public int TotalItemCount { get; set; }

        public int TotalPageCount { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }


        public PagingMetadata(int totalItemCount, int pageSize, int currentPage) {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPageCount = (int)Math.Ceiling((double)TotalItemCount / PageSize);
        }
    }
}