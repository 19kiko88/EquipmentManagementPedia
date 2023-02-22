namespace EMP.Website.Dtos
{
    public class ReqGetDiffList
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> PNs { get; set; }
    }
}
