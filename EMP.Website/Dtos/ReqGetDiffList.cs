﻿namespace EMP.Website.Dtos
{
    public class ReqGetDiffList
    {
        public string FilePath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? PNs { get; set; }
    }
}
