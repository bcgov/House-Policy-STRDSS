Feature: SendingMultipleNoticesOfNonCompliance

Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-57
#, https://hous-hpb.atlassian.net/browse/DSS-104

@Scenario
@TermsAndConditions
Scenario: SendingMultipleNoticesOfNonCompliance

#Preconditions:
#•	LG user has valid login credentials.
#•	Listings are available for selection.

	When a LG User Navigate to Login Page 
#	LG user navigates to login page.
#	Login: 
	And user enters valid login credentials and clicks “Login" button
#	System verifies credentials and logs user in.
	Then LG user is redirected to dashboard-> Hompage
#	Navigate to Listings: 
	When LG user selects "View Listings" from menu to load listings data page. Or User navigates to view listing data on homepage Screen
#	Initial State: 
	Then the Send Notices of Non-Compliance  button is disabled at this stage
	When LG USer Select Multiple Listings: 
	Then the “Send Notices of Non-Compliance" button is enabled
#	Click Send Notices of Non-Compliance: 
	When LG user clicks “Send Notices of Non-Compliance" button
	Then system opens details to complete fields for sending notices 
#	Mandatory Fields Check: 
	Then the “Review" button is disabled if any mandatory field is not completed
	Then If LG user clicks "Cancel",  system prompts with a re-confirmation message 
	Then If user confirms cancellation, user is redirected back to listings data page
	And the action history is not updated when the user cancels the action
	When user does not confirm, user remains on current page.
	And  Action History Not Updated 
#	Verify that the action history is not updated when the user cancels the action.
#	Complete Mandatory Fields: 
	When LG user completes mandatory fields. ( Provide a LG email address to receive a copy of the Notice)
#	Complete Optional Fields: 
	Then that LG user can add BCCs
#	Check State of "Review" Button 
#	LG user also verify the "Review" button is disabled if user inputs an email that is not in the correct format, with a note (ensure the email format you have entered is correct) to prompt user
	When the LG User enters an Email Address
	Then if user inputs an email that is not in the correct format the user is prompted to enter an email address in the correct format
#	Then (ensure the email format you have entered is correct) to prompt user
	And  the user can add multiple email addresses
	And Verify that if remove the listing checkbox is unchecked, review is also disabled 
#	Review and Submit Notices: 
	When the LG user clicks “Review" button to confirm details to be sent
	And the LG user selects "Submit"
	Then Successful confirmation is displayed for user on top Right of the page 
	Then  System immediately sends notices to platform/host for selected listings
	And A copy email is also sent to LG email address added to receive a copy of the notice same, a copy of email to bcc 
	And Action history is updated immediately with action taken
	And On the listings page, last action and last action date should be updated

#	Scenarios 1:LG user selects multiple listings from available listing data .
#	Scenario 2: LG user selects listings from different platforms
Examples:
	| UserName     | RoleName  | Email             | Environment | ExpectedResult |
	| CEUATST      | ceu_admin | ceuatst@gov.bc.ca | all         | pass           |
	#| STRDSSLg1Dev | lg_staff  | ceuatst@gov.bc.ca | all         | pass           |




