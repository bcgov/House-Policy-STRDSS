namespace UITest.Models
{
    public class CreateApplicationPageModel
    {
        public static string PropertyOwnerLastName { get => "#lastName_input"; }
        public static string PropertyStreetAddress { get => "#streetAddress"; }
        public static string City { get => "#city"; }

        public static string Province { get => "#province"; }
        public static string PostalCode { get => "#postalCode"; }
        public static string ZoningTypeDropDown { get => "zoningTypeId"; }
        public static string SquareFootage { get => "#squareFootage > span > input"; }
        public static string STRAffiliateDropDown { get => "#strAffiliateId > span"; }
        public static string IsPropertyOwnerPrimaryResidenceCheckBox { get => "#isOwnerPrimaryResidence > div > div.p-checkbox-box"; }
        public static string CancelButton { get => "#cancel_application_button > button"; }
        public static string SubmitButton { get => "#submit_application_button > button"; }


    }
}
