Feature: SendNoticeOfTakedownWithoutADSSlisting
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-71

@Delisting
Scenario: SendNoticeOfTakedownWithoutADSSlisting
#User Authentication
	Given that I am an authenticated LG staff member "<UserName>" and the expected result is "<ExpectedResult>"
	When I navigate to the delisting warning feature

#Input Form
	Then I should be presented with an input form that includes fields for the listing URL, and an optional field for the host email address

	And I should be presented with a field to select which platform to send the warning to

	#And I should be presented with a dropdown menu to select reason for delisting

	And I should see an optional field for Listing ID "ListingID"

	And I should see an optional field for adding a LG staff user email address to be copied on the email

#ListingURLField

	When Entering the listing URL "<ListingURL>"

	Then The system should validate the URL format and ensure it is a valid link to the property listing

#PlatformField
	When selecting the platform

	Then the system should present a list of available platform options to populate the field

#HostEmailAddressField
	When entering the optional host email address

	Then the system should validate the email format

#ReasonForDelisting

#	When I select a reason for delisting
#
#	Then the system should present a list of reasons for requesting delisting: No business licence provided, invalid business licence number, expired business licence, or suspended business license

#CC and Send Copy Options:

	When submitting a notice

	Then there should be checkboxes or fields to enable sending a copy of the request to myself or adding additional CCs "<AdditionalCCsTextBox>"

#Local Government Contact Information:

	Then there should be fields to input email and phone number for the local government contact, with the latter being optional

#DelistingWarningMessage

	When all required fields are entered

	Then I see a template delisting warning message that will be sent to both the platform and host

#SendDelistingRequest

	When I submit the form with valid information

	Then the system should send the delisting warning message to the provided platform and host email addresses

#ConfirmationMessage

	When successful submission

	Then I should receive a confirmation message indicating that the delisting warning has been sent

	Then I should be copied on the email
#NotificationToPlatformAndHost

	When the delisting warning is submitted

	Then the platform and host should receive email notifications containing the delisting warning and instructions for compliance

#FrontEndErrorHandling

	When there are issues with the submission, such as invalid email addresses or a missing URL

	Then the system should provide clear error messages guiding me on how to correct the issues
#
Examples:
	| UserName | ListingID           | Description                      | ExpectedResult | ListingURL                                                   | AdditionalCCsTextBox     | GovPhoneNumber | TakedownReason         |
	| STRDSSLg1Dev  | 0                   | ListingID - Boundary             | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 9223372036854775807 | ListingID - Test for Max value   | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 0                   | ListingURL - Valid URL           | pass           | HTTP://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 0                   | ListingURL - Valid URL           | pass           | HTTPS://listingURL.com                                       | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 0                   | ListingURL - Long URL SSL        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 0                   | Phone Number - Valid  '-'        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | 999-123-1234   | Get a business license |
	| STRDSSLg1Dev  | 0                   | Phone Number - Valid  '('        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | (999)1231234   | Get a business license |
	| STRDSSLg1Dev  | 0                   | Phone Number - Valid  '- and ('  | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | (999)-123-1234 | Get a business license |
	| STRDSSLg1Dev  | -1                  | ListingID - Negative number test | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | test                | ListingID - Test for string      | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | e                   | ListingID - Test for exponential | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
	| STRDSSLg1Dev  | 0                   | ListingURL - Invalid URL         | fail           | http://listingURL                                            | richard.anderson@dxc.com | 9991231234     | Get a business license |


