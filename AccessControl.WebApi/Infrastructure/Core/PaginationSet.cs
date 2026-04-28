namespace AccessControl.WebApi.Infrastructure.Core
{
    public class PaginationSet<T>
    {
        public int Page { set; get; }

        public int Count
        {
            get
            {
                return Items != null ? Items.Count() : 0;
            }
        }

        public int TotalPages { set; get; }
        public int TotalCount { set; get; }
        public int MaxPage { set; get; }
        public IEnumerable<T>? Items { set; get; }
    }
    public class PaginationDashboardSet<T>
    {
        public int Page { set; get; }

        public int Count
        {
            get
            {
                return Items != null ? Items.Count() : 0;
            }
        }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int User { get; set; }
        public int Finger { get; set; }
        public int Face { get; set; }
        public int Card { get; set; }
        public int Device { get; set; }
        public int Late { get; set; }
        public int Early { get; set; }
        public int Absent { get; set; }
        public int OverTime { get; set; }
        public int TotalPages { set; get; }
        public int TotalCount { set; get; }
        public int MaxPage { set; get; }
        public IEnumerable<T>? Items { set; get; }
    }
}
