namespace UITest.Models
{
    public class LoginPageModel
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

        public static string SignupButton
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
                return "submit_button";
            }
        }

    }
}
