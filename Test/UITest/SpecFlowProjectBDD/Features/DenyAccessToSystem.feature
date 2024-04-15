Feature: DenyAccessToSystem
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-75

@Access
Scenario: DenyAccessToSystem
#User Authentication

	Given that I am an authenticated LG, CEU, Provincial Gov or Platform user and the expected result is "<ExpectedResult>"

	When I attempt to access the Data Sharing System as "<UserName>"

	Then I dont have the required access permissions

	Then I should see a specific message indicating that access is restricted
#
#
Examples:
	| UserName | Description | ExpectedResult |
	| CEUATST  | HappyPath   | pass           |