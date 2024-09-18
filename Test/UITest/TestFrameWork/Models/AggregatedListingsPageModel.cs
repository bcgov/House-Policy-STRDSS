namespace UITest.Models
{
    public class AggregatedListingsPageModel
    {

        public static string ListingsTypeDropDown { get => ""; }

        public static string ErrorMessageBox { get => "body > app-root > app-layout > p-toast > div > p-toastitem > div"; }

        public static string RowsPerPageTextBox { get => "#pn_id_17_content > div > p-paginator > div > span"; }

        public static string FirstPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-first.p-paginator-element.p-link.p-disabled.ng-star-inserted"; }

        public static string LastPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-last.p-paginator-element.p-link.ng-star-inserted"; }

        public static string PrevPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-prev.p-paginator-element.p-link.p-disabled"; }

        public static string NextPageButton { get => "#pn_id_15_content > div > p-paginator > div > button.p-ripple.p-element.p-paginator-next.p-paginator-element.p-link"; }

        public static string SendNoticeOfNonComplianceButton { get => "send_delisting_notice_btn"; }
        public static string SendTakedownRequestButton { get => "send_takedown_request_btn"; }

        public static string AggregatedListingsTable { get => "listings-table"; }

        public static string SelectAllCheckbox {get => "[id^='pn_id_'][id$='-table'] > thead > tr > th:nth-child(1) > p-tableheadercheckbox > div > div.p-checkbox-box"; }

        public static string ExpandButton { get => @"#expand-listing-row-{Row} > i"; }
}
}
