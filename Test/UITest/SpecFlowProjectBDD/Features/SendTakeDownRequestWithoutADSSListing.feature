Feature: SendTakedownRequestWithoutADSSListing
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-74

@Delisting
Scenario: SendTakedownRequestWithoutADSSListing
#User Authentication
	Given I am an authenticated LG staff member and the expected result is "<ExpectedResult>"

	When I navigate to the delisting request feature

#Input Form
	Then I should be presented with an input form that includes fields for the listing URL

	And I Should be Presented with an Input form that Lists requests Initiated By

	And I should be presented with a field to select which platform to send the request to

	And I should see an optional field for adding a LG staff user email address to be copied on the email

#Initiated By

	When Selecting the LG for Initiated By

	Then The system should present a list of available LG options to populate the field

#ListingIDField

	When Entering the listing ID "<ListingID>"

	Then The system should validate the ID is a number

#ListingURLField

	When Entering the listing URL "<ListingURL>"

	Then The system should validate the URL format and ensure it is a valid link to the property listing

#PlatformField
	When selecting the platform

	Then the system should present a list of available platform options to populate the field

#DelistingRequestMessage

	When all required fields are entered

	Then I click the review button

	Then I see a template delisting request message that will be sent to both the platform

#SendDelistingRequest

	When I submit the form with valid information

	Then the system should send the delisting request message to the platform email addresses associated with the selected platform

#ConfirmationMessage

	When successful submission

	Then I should receive a confirmation message indicating that the delisting request has been sent

	Then I should be copied on the email
#NotificationToPlatformAndHost

	When the delisting request is submitted

	Then the platform and host should receive email notifications containing the delisting request and instructions for compliance

#FrontEndErrorHandling

	When there are issues with the submission, such as invalid email addresses or a missing URL

	Then the system should provide clear error messages guiding me on how to correct the issues

Examples:
	| ListingID           | Description                      | ExpectedResult | ListingURL                                                   | AdditionalCCsTextBox     | Comment                                                                        |
	| 0                   | ListingID - Boundary             | pass           | http://listingURL.com                                        | richard.anderson@dxc.com |                                                                                |
	| 9223372036854775807 | ListingID - Test for Max value   | pass           | http://listingURL.com                                        | richard.anderson@dxc.com |                                                                                |
	| 0                   | ListingURL - Valid URL           | pass           | HTTP://listingURL.com                                        | richard.anderson@dxc.com |                                                                                |
	| 0                   | ListingURL - Valid URL SSL       | pass           | HTTPS://listingURL.com                                       | richard.anderson@dxc.com |                                                                                |
	| 0                   | ListingURL - Long URL            | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com |                                                                                |
	| -1                  | ListingID - Negative number test | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | Should pass because non-numberic values are ignored and Listing ID is optional |
	| test                | ListingID - Test for string      | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | should pass because non-numberic values are ignored and Listing ID is optional |
	| e                   | ListingID - Test for exponential | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | should pass because non-numberic values are ignored and Listing ID is optional |
	| 0                   | ListingURL - Invalid URL         | fail           | http://listingURL                                            | richard.anderson@dxc.com |                                                                                |
