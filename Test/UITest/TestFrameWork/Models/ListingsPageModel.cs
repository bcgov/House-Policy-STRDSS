namespace UITest.Models
{
    public class ListingsPageModel
    {
        public static string ListingsTypeDropDown { get => ""; }

        public static string ErrorMessageBox { get => "body > app-root > app-layout > p-toast > div > p-toastitem > div"; }

        public static string RowsPerPageTextBox { get => "#pn_id_17_content > div > p-paginator > div > span"; }

        public static string FirstPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-first.p-paginator-element.p-link.p-disabled.ng-star-inserted"; }

        public static string LastPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-last.p-paginator-element.p-link.ng-star-inserted"; }

        public static string PrevPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-prev.p-paginator-element.p-link.p-disabled"; }

        public static string NextPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-next.p-paginator-element.p-link"; }


    }
}
