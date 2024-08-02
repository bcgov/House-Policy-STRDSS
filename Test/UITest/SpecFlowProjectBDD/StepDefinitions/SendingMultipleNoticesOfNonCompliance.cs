using System;
using TechTalk.SpecFlow;

namespace SpecFlowProjectBDD.StepDefinitions
{
    [Binding]
    public class SendingMultipleNoticesOfNonCompliance
    {
        [When(@"a LG User Navigate to Login Page")]
        public void WhenALGUserNavigateToLoginPage()
        {
            throw new PendingStepException();
        }

        [When(@"user enters valid login credentials and clicks “Login"" button")]
        public void WhenUserEntersValidLoginCredentialsAndClicksLoginButton()
        {
            throw new PendingStepException();
        }

        [Then(@"LG user is redirected to dashboard-> Hompage")]
        public void ThenLGUserIsRedirectedToDashboard_Hompage()
        {
            throw new PendingStepException();
        }

        [When(@"LG user selects ""([^""]*)"" from menu to load listings data page\. Or User navigates to view listing data on homepage Screen")]
        public void WhenLGUserSelectsFromMenuToLoadListingsDataPage_OrUserNavigatesToViewListingDataOnHomepageScreen(string p0)
        {
            throw new PendingStepException();
        }

        [Then(@"the Send Notices of Non-Compliance  button is disabled at this stage")]
        public void ThenTheSendNoticesOfNon_ComplianceButtonIsDisabledAtThisStage()
        {
            throw new PendingStepException();
        }

        [When(@"LG USer Select Multiple Listings:")]
        public void WhenLGUSerSelectMultipleListings()
        {
            throw new PendingStepException();
        }

        [Then(@"the “Send Notices of Non-Compliance"" button is enabled")]
        public void ThenTheSendNoticesOfNon_ComplianceButtonIsEnabled()
        {
            throw new PendingStepException();
        }

        [When(@"LG user clicks “Send Notices of Non-Compliance"" button")]
        public void WhenLGUserClicksSendNoticesOfNon_ComplianceButton()
        {
            throw new PendingStepException();
        }

        [Then(@"system opens details to complete fields for sending notices")]
        public void ThenSystemOpensDetailsToCompleteFieldsForSendingNotices()
        {
            throw new PendingStepException();
        }

        [Then(@"the “Review"" button is disabled if any mandatory field is not completed")]
        public void ThenTheReviewButtonIsDisabledIfAnyMandatoryFieldIsNotCompleted()
        {
            throw new PendingStepException();
        }

        [Then(@"If LG user clicks ""([^""]*)"",  system prompts with a re-confirmation message")]
        public void ThenIfLGUserClicksSystemPromptsWithARe_ConfirmationMessage(string cancel)
        {
            throw new PendingStepException();
        }

        [Then(@"If user confirms cancellation, user is redirected back to listings data page")]
        public void ThenIfUserConfirmsCancellationUserIsRedirectedBackToListingsDataPage()
        {
            throw new PendingStepException();
        }

        [Then(@"the action history is not updated when the user cancels the action")]
        public void ThenTheActionHistoryIsNotUpdatedWhenTheUserCancelsTheAction()
        {
            throw new PendingStepException();
        }

        [When(@"user does not confirm, user remains on current page\.")]
        public void WhenUserDoesNotConfirmUserRemainsOnCurrentPage_()
        {
            throw new PendingStepException();
        }

        [When(@"Action History Not Updated")]
        public void WhenActionHistoryNotUpdated()
        {
            throw new PendingStepException();
        }

        [When(@"LG user completes mandatory fields\. \( Provide a LG email address to receive a copy of the Notice\)")]
        public void WhenLGUserCompletesMandatoryFields_ProvideALGEmailAddressToReceiveACopyOfTheNotice()
        {
            throw new PendingStepException();
        }

        [Then(@"that LG user can add BCCs")]
        public void ThenThatLGUserCanAddBCCs()
        {
            throw new PendingStepException();
        }

        [When(@"the LG User enters an Email Address")]
        public void WhenTheLGUserEntersAnEmailAddress()
        {
            throw new PendingStepException();
        }

        [Then(@"if user inputs an email that is not in the correct format the user is prompted to enter an email address in the correct format")]
        public void ThenIfUserInputsAnEmailThatIsNotInTheCorrectFormatTheUserIsPromptedToEnterAnEmailAddressInTheCorrectFormat()
        {
            throw new PendingStepException();
        }

        [Then(@"the user can add multiple email addresses")]
        public void ThenTheUserCanAddMultipleEmailAddresses()
        {
            throw new PendingStepException();
        }

        [Then(@"Verify that if remove the listing checkbox is unchecked, review is also disabled")]
        public void ThenVerifyThatIfRemoveTheListingCheckboxIsUncheckedReviewIsAlsoDisabled()
        {
            throw new PendingStepException();
        }

        [When(@"the LG user clicks “Review"" button to confirm details to be sent")]
        public void WhenTheLGUserClicksReviewButtonToConfirmDetailsToBeSent()
        {
            throw new PendingStepException();
        }

        [When(@"the LG user selects ""([^""]*)""")]
        public void WhenTheLGUserSelects(string submit)
        {
            throw new PendingStepException();
        }

        [Then(@"Successful confirmation is displayed for user on top Right of the page")]
        public void ThenSuccessfulConfirmationIsDisplayedForUserOnTopRightOfThePage()
        {
            throw new PendingStepException();
        }

        [Then(@"System immediately sends notices to platform/host for selected listings")]
        public void ThenSystemImmediatelySendsNoticesToPlatformHostForSelectedListings()
        {
            throw new PendingStepException();
        }

        [Then(@"A copy email is also sent to LG email address added to receive a copy of the notice same, a copy of email to bcc")]
        public void ThenACopyEmailIsAlsoSentToLGEmailAddressAddedToReceiveACopyOfTheNoticeSameACopyOfEmailToBcc()
        {
            throw new PendingStepException();
        }

        [Then(@"Action history is updated immediately with action taken")]
        public void ThenActionHistoryIsUpdatedImmediatelyWithActionTaken()
        {
            throw new PendingStepException();
        }

        [Then(@"On the listings page, last action and last action date should be updated")]
        public void ThenOnTheListingsPageLastActionAndLastActionDateShouldBeUpdated()
        {
            throw new PendingStepException();
        }
    }
}
