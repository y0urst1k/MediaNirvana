namespace Infrastructure.DTO
{
    public class FilterState
    {
        public string Query { get; set; } = "";
        public string Type { get; set; } = "All";
        public string Year { get; set; } = "All";
        public string Tag { get; set; } = "All Tags";

        public bool IsActive => !string.IsNullOrEmpty(Query) || Type != "All" || Year != "All" || Tag != "All Tags";
    }
}
