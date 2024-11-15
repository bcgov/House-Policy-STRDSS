Feature: ManageJurisdiction
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-860

@ManageJurisdiction
Scenario: ManageJurisdiction
#User Authentication
	Given I am an authenticated CEU staff member "<UserName>"  with the appropriate permissions (ADD) and the expected result is "<ExpectedResult>" and I am a "<UserType>" user

#Accessing the feature
	When I log in and navigate to the Manage Jurisdictions feature
	Then I should be presented with a list of platforms with a list of local government jurisdictions

#Jurisdiction Information
	When I view the list of jurisdictions 
	Then I should see key information about each one

		#Principle Residence Requirement Applies
		#Business Licence Requirement 
		#STRs prohibited
		#Business Licence format
		#Local Government Code 

#Edit Jurisdiction Information
	And I should have the ability to edit key information about each one (as above)

Examples:
	| UserName | UserType  | Environment | ExpectedResult |
	| CEUaTST  | ceu_admin | all         | pass           |






