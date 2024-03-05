namespace StrDss.Common
{
    public class RegexInfo
    {
        public string Regex { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class RegexDefs
    {
        public const string Email = "Email";
        public const string GpsCoords = "D14_9"; //latitude longitude
        public const string Offset = "D7_3"; //offset
        public const string QtyAccmpAmount = "D11_3";
        public const string DollarValue = "D10"; //value of work, accomplishment
        public const string Volume = "D6_2"; //rockfall volume
        public const string Quantity = "D4"; //wildlife quantity
        public const string RatioValue = "D9_5";    //ratio value numeric(9,5)
        public const string SiteNumber = "SiteNumber";
        public const string Time = "Time";
        public const string Direction = "Direction";
        public const string YN = "YN";
        public const string Phone = "Phone";
        public const string Alphanumeric = "Alphanumeric";
        public const string StructureNumber = "StructureNumber";
        public const string Password = "Password";
        public const string PhoneNumber = "PhoneNumber";

        private readonly Dictionary<string, RegexInfo> _regexInfos;

        public RegexDefs()
        {
            _regexInfos = new Dictionary<string, RegexInfo>();

            _regexInfos.Add(Email, new RegexInfo { Regex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Wrong email address" });

            _regexInfos.Add(GpsCoords, new RegexInfo { Regex = @"^\-?\d{1,5}(\.\d{1,9})?$", ErrorMessage = "Value must be a number of less than 6 digits optionally with maximum 9 decimal digits" });
            _regexInfos.Add(Offset, new RegexInfo { Regex = @"^\-?\d{1,4}(\.\d{1,3})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 3 decimal digits" });
            _regexInfos.Add(DollarValue, new RegexInfo { Regex = @"^\-?\d{1,10}$", ErrorMessage = "Value [{0}] must be a number of less than 11 digits" });
            _regexInfos.Add(Volume, new RegexInfo { Regex = @"^\-?\d{1,4}(\.\d{1,2})?$", ErrorMessage = "Value must be a number of less than 5 digits optionally with maximum 2 decimal digits" });
            _regexInfos.Add(Quantity, new RegexInfo { Regex = @"^\-?\d{1,4}$", ErrorMessage = "Value must be a number of less than 5 digits" });
            _regexInfos.Add(QtyAccmpAmount, new RegexInfo { Regex = @"^\-?\d{1,10}(\.\d{1,3})?$", ErrorMessage = "Value must be a number of less than 11 digits optionally with maximum 3 decimal digits" });
            _regexInfos.Add(RatioValue, new RegexInfo { Regex = @"^\-?\d{1,8}(\.\d{1,5})?$", ErrorMessage = "Value must be a number of less than 9 digits optionally with maximum 5 decimal digits" });

            _regexInfos.Add(SiteNumber, new RegexInfo { Regex = @"^[ABDLRSTWX]\d{4}\d{0,2}$", ErrorMessage = "Value must start with one of these 9 [ABDLRSTWX] letters followed by 4 to 6 digits" });

            _regexInfos.Add(Time, new RegexInfo { Regex = @"^(0[0-9]|1[0-9]|2[0-3]|[0-9]):[0-5][0-9]$", ErrorMessage = "Value must be in time format such as 21:35" });
            _regexInfos.Add(Direction, new RegexInfo { Regex = @"^[NSEW]$", ErrorMessage = "Value must be one of these 4 [NSEW] letters" });
            _regexInfos.Add(YN, new RegexInfo { Regex = @"^[YN]$", ErrorMessage = "Value must be Y or N" });
            _regexInfos.Add(Phone, new RegexInfo { Regex = @"^(\+0?1\s)?\(?\d{3}\)?[\s.-]\d{3}[\s.-]\d{4}$", ErrorMessage = "Value must follow phone number format" });

            _regexInfos.Add(Alphanumeric, new RegexInfo { Regex = @"^[a-zA-Z0-9]*$", ErrorMessage = "Value must be alphanumeric" });
            _regexInfos.Add(StructureNumber, new RegexInfo { Regex = @"^[a-zA-Z0-9]{2,6}$", ErrorMessage = "Structure number must be alphanumeric with max length 6 and minimum length 2" });

            _regexInfos.Add(Password, new RegexInfo { Regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit." });
            _regexInfos.Add(PhoneNumber, new RegexInfo { Regex = @"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Phone number must follow the phone number format (xxx) xxx-xxxx" });

        }

        public RegexInfo GetRegexInfo(string name)
        {
            if (!_regexInfos.TryGetValue(name, out RegexInfo regexInfo))
            {
                throw new Exception($"RegexInfo for {name} does not exist.");
            }

            return regexInfo;
        }
    }
}
