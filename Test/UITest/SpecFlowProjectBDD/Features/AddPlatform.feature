Feature: ManagePlatformsAddPlatform
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226

@AddPlatform
Scenario: AddPlatform
#User Authentication
	Given that I am an authenticated User "<UserName>" and the expected result is "<ExpectedResult>" and I am a "<UserType>" user
	Then I am directed to the Landing Page

#Select Manage Platforms
	When I login and navigate to the Manage Platforms feature

	Then I should be presented with a list of platforms and sub-platforms

#Select Platform
	When I click the add new platform button for a platform

#Add platform

	Then I should be presented with the Add Platform page

#Input fields
	Then I should see a form with the required input fields for creating a sub-platform

	#Platform Name
	#Platform Code
	#Primary Email for Non-Compliance Notices
	#Primary Email for Takedown Request Letters
	#Secondary Email for Non-Compliance Notices
	#Secondary Email for Takedown Request Letters
	#Platform Type
	#Platform Status


#Enter values for Input Fields
	When I fill in valid values for the input fields
	Then the Save button should be enabled

#Click Save button to create sub platform
	When I click the Save button
	Then the sub platform should be created 
	Then the sub platform should be a child of the parent platform



Examples:
	| UserName         | UserType       | Environment | ExpectedResult |
	| CEUATST          | ceu_admin      | all         | pass           |






