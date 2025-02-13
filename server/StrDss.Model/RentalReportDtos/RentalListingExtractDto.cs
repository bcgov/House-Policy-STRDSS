﻿using System.Text.Json.Serialization;

namespace StrDss.Model.RentalReportDtos
{
    public class RentalListingExtractDto
    {
        public long RentalListingExtractId { get; set; }
        public string RentalListingExtractNm { get; set; } = null!;
        public bool IsPrRequirementFiltered { get; set; }
        public long? FilteringOrganizationId { get; set; }
        [JsonIgnore]
        public byte[]? SourceBin { get; set; }
        public DateTime UpdDtm { get; set; }
    }
}
