namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GetCoverageResultQuery : CpGateWayQueryBase, IQuery<List<string>>
    {
        public string IDCardNo { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string SubDistrict { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required]
        public string BuildingType { get; set; }

        public string Tower { get; set; }

        public string Floor { get; set; }

        [Required]
        public string HouseNo { get; set; }

        public string Moo { get; set; }

        public string Soi { get; set; }

        public string Road { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }

        public string OnlineNumberFlag { get; set; }
    }
}