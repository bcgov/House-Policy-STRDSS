Feature: ManagePlatformsEditSubPlatform
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-226

@EditSubsidiaryPlatform
Scenario: EditSubsidiaryPlatform
#User Authentication
	Given that I am an authenticated User "<UserName>" and the expected result is "<ExpectedResult>" and I am a "<UserType>" user
	Then I am directed to the Landing Page

#Select Manage Platforms
	When I click on the Manage Platforms button
	Then I should be presented with a list of platforms and sub-platforms

#Select Platform
	When I select an existing platform
	Then I should be be able to edit platform information,
	And I should see a call to action to disable the platform

#Edit sub-platform Name
	When I edit platform name information
	Then platform information should update across the platform (e.g., listing view, detailed view, and drop down platform select menus, etc.)

#Edit sub-platform Contacts
	When I edit platform email addresses
	Then emails should go to the updated platform contacts for each type of email (Notice, takedown,)

#Edit Platform Parent, Subsidiary or Code
	When I update parent or subsidiary information or platform code
	Then the platform should be able to upload monthly data reports or takedown reports for all platforms associated with it (ie. parent or subsidiary platforms)
	And the platform uploads should validate against the updated platform code

#Input fields
#	Then I should see a form with the required input fields for creating a sub-platform
#
#	#Platform Name
#	#Platform Code
#	#Primary Email for Non-Compliance Notices
#	#Primary Email for Takedown Request Letters
#	#Secondary Email for Non-Compliance Notices
#	#Secondary Email for Takedown Request Letters


#Error Handling:
When submitting platform details
Then the system should perform validation checks and provide clear error messages for any input errors

#Click Save button to create sub platform
	When I click the Save button
	Then the sub platform should be created
	Then the sub platform should be a child of the parent platform

Examples:
	| UserName | UserType  | Environment | ExpectedResult |
	| CEUATST  | ceu_admin | all         | pass           |






