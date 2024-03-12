namespace Models
{
    public class SignupPageModel
    {

        public static string UserName
        {
            get
            {
                return "#username_input";
            }
        }

        public static string Password
        {
            get
            {
                return "#password_input";
            }
        }

        public static string LastName
        {
            get
            {
                return "#lastName_input";
            }
        }

        public static string PhoneNumber
        {
            get
            {
                return "#phone_mask_input";
            }
        }

        public static string StreetAddress
        {
            get
            {
                return "#addressStreet_input";
            }
        }

        public static string City
        {
            get
            {
                return "#addressCity_input";
            }
        }

        public static string Province
        {
            get
            {
                return "#addressProvince_input";
            }
        }

        public static string PostalCode
        {
            get
            {
                return "#addressPostalCode_input";
            }
        }

        public static string SigninButton
        {
            get
            {
                return "#to_register_link > button";
            }
        }

        public static string SubmitButton
        {
            get
            {
                return "document.querySelector(\"#submit_button\").click()";
            }
        }
    }
}
