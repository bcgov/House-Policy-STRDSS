namespace UITest.Models
{
    public class BCIDModel
    {
        public static string UserName
        {
            get
            {
                return "user";
            }
        }

        public static string Password
        {
            get
            {
                return "password";
            }
        }

        public static string ContinueButton
        {
            get
            {
                return "#login-form > section > div > div.col-sm-7.col-md-8 > div:nth-child(1) > div.panel-body > div.login-form-action > input";
            }
        }

    }
}
