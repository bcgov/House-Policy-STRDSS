using StrDss.Common.Converters;
using System.Text.Json.Serialization;

namespace StrDss.Model
{
    public class Crs
    {
        public string Type { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; } = "";
        public Crs Crs { get; set; }
        public double[] Coordinates { get; set; }
    }

    public class Properties
    {
        public string FullAddress { get; set; } = "";
        public int Score { get; set; }
        public string MatchPrecision { get; set; } = "";
        public int PrecisionPoints { get; set; }
        public List<object> Faults { get; set; }
        public string SiteName { get; set; } = "";
        public string UnitDesignator { get; set; } = "";

        [JsonConverter(typeof(Int32ToStringJsonConverter))]
        public string UnitNumber { get; set; } = "";
        public string UnitNumberSuffix { get; set; } = "";

        [JsonConverter(typeof(Int32ToStringJsonConverter))]
        public string CivicNumber { get; set; } = "";
        public string CivicNumberSuffix { get; set; } = "";
        public string StreetName { get; set; } = "";
        public string StreetType { get; set; } = "";
        public string IsStreetTypePrefix { get; set; } = "";
        public string StreetDirection { get; set; } = "";
        public string IsStreetDirectionPrefix { get; set; } = "";
        public string StreetQualifier { get; set; } = "";
        public string LocalityName { get; set; } = "";
        public string LocalityType { get; set; } = "";
        public string ElectoralArea { get; set; } = "";
        public string ProvinceCode { get; set; } = "";
        public string LocationPositionalAccuracy { get; set; } = "";
        public string LocationDescriptor { get; set; } = "";
        public string SiteId { get; set; } = "";
        [JsonConverter(typeof(Int32ToStringJsonConverter))]
        public string BlockId { get; set; } = "";
        public string FullSiteDescriptor { get; set; } = "";
        public string AccessNotes { get; set; } = "";
        public string SiteStatus { get; set; } = "";
        public string SiteRetireDate { get; set; } = "";
        public string ChangeDate { get; set; } = "";
        public string IsOfficial { get; set; } = "";
    }

    public class Feature
    {
        public string Type { get; set; } = "";
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class GeocoderResponse
    {
        public string Type { get; set; } = "";
        public string QueryAddress { get; set; } = "";
        public string SearchTimestamp { get; set; }
        public double ExecutionTime { get; set; }
        public string Version { get; set; } = "";
        public string BaseDataDate { get; set; }
        public string Interpolation { get; set; } = "";
        public string Echo { get; set; } = "";
        public string LocationDescriptor { get; set; } = "";
        public int SetBack { get; set; }
        public int MinScore { get; set; }
        public int MaxResults { get; set; }
        public string Disclaimer { get; set; } = "";
        public string PrivacyStatement { get; set; } = "";
        public string CopyrightNotice { get; set; } = "";
        public string CopyrightLicense { get; set; } = "";
        public Feature[] Features { get; set; }
    }
}
