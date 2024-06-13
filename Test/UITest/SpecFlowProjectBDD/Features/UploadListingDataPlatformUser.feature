Feature: UploadListingDataPlatformUser
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-23

@UploadListingData
Scenario: UploadListingDataPlatformUser
#Platform Staff Authentication:
	Given I am an authenticated platform representative "<UserName>" with the necessary permissions and the expected result is "<ExpectedResult>" and I am a "<UserType>" user

#Access to Data Sharing System:
	When I access the Data Sharing System

	Then I should have the option to upload short-term listing data



#Initiate Upload Process:

	When I opt to upload short-term listing data

	Then the upload data interface should load

#CSV File Selection:

	Given I am on the upload data interface

	When I select a CSV file containing short-term listing data

#Month Designation:

	And I select which month the STR listing data is for

#Validation rules 

#Users cannot upload STR listing data for future month 
#
#Users can upload STR listing data for previous months 

#Initiate Upload:

	When I initiate the upload

	Then the Data Sharing System should import the STR listing data

#Successful Upload:

	When the data import is successful

	Then I should see a success message

	And a new entry on an upload log with a timestamp, username, and the number of records created.

#Unsuccessful Upload:

	When the data import is not successful

	Then I should see a confirmation message indicating the issue.

	And a new entry on an import log with a timestamp, username, and information about the unsuccessful import, such as error details.

#Validation -need to be defined 

#Email Confirmation:

	When the data import is complete

	Then i should receive an email confirming the status of my upload: Template: Platform Upload Error Notification

	And a report of any error codes that need to be addressed

#Testing:

#Conduct thorough testing to validate the functionality of the upload process and associated features under various scenarios.


Examples:
	| UserName      | UserType       | Environment | ExpectedResult |
	| STRDSSVrboDev | platform_staff | dev         | pass           |
	#| STRDSSAirbnbDev | platform_staff | dev       | pass           |






