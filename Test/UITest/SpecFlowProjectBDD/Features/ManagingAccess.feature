Feature: ManagingAccess
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-54

@Access
Scenario: ManagingAccess
#User Authentication

Given that I am an authenticated government user and the expected result is "<ExpectedResult>"

When I access the administrative interface of the system

Then There should be a dedicated section for managing user access requests

#User Access Request List:

When I navigate to the user access request section

Then I should see a list displaying all user access requests, including relevant details such as the user's name, role request, and date of submission

#Request Details:

When Reviewing a specific access request

Then I should be able to view detailed information provided by the user, including their role request and any justifications or additional comments

#Grant Access Button:

When Reviewing an access request

Then There should be a Grant Access button allowing me to approve the user's request

#Role Assignment:

When Clicking the Grant Access button

Then I should be prompted to assign the appropriate roles to the user based on their request and the system's role hierarchy

#Deny Access Option:

When Reviewing an access request for denial

Then There should be a Deny Access option allowing me to reject the user's request if it is deemed inappropriate or unnecessary

#Remove Access Option:

When Reviewing an access request that has been granted

Then There should be a Remove Access option allowing me to remove the user's access if it is deemed inappropriate or unnecessary

#Confirmation Message:

When Granting or denying access

Then I should receive a confirmation message indicating the success of the action taken, and the user should be notified accordingly

#User Access Status Updates:

When Managing user access requests

Then The access request list should dynamically update to reflect the current status approved or denied of each request

#
#
Examples:
	| ListingID           | Description                      | ExpectedResult | ListingURL                                                   | AdditionalCCsTextBox     | GovPhoneNumber | TakedownReason         |
	| 0                   | ListingID - Boundary             | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 9223372036854775807 | ListingID - Test for Max value   | pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 0                   | ListingURL - Valid URL           | pass           | HTTP://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 0                   | ListingURL - Valid URL           | pass           | HTTPS://listingURL.com                                       | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 0                   | ListingURL - Long URL SSL        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 0                   | Phone Number - Valid  '-'        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | 999-123-1234   | Get a business license |
#	| 0                   | Phone Number - Valid  '('        | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | (999)1231234   | Get a business license |
#	| 0                   | Phone Number - Valid  '- and ('  | pass           | http://ReallylongURLstring123123123123123123123123123123.com | richard.anderson@dxc.com | (999)-123-1234 | Get a business license |
#	| -1                  | ListingID - Negative number test | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| test                | ListingID - Test for string      | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| e                   | ListingID - Test for exponential | Pass           | http://listingURL.com                                        | richard.anderson@dxc.com | 9991231234     | Get a business license |
#	| 0                   | ListingURL - Invalid URL         | fail           | http://listingURL                                            | richard.anderson@dxc.com | 9991231234     | Get a business license |


