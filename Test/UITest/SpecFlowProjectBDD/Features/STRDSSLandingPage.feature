Feature: STRDSSLandingPage
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-4

@LandingPage
Scenario: STRDSSLandingPage
#User Authentication
	Given that I am an authenticated User "<UserName>" and the expected result is "<ExpectedResult>"
	When I navigate to the Landing Page

#Landing Page for Government Users:


	When I am an authenticated government user and I access the Data Sharing System landing page

	Then I should find where I can submit delisting warnings and requests to short-term rental platforms

#Landing Page for Platform Users:

	When I am an authenticated platform user "<UserType>" and I access the Data Sharing System landing page

	#And I access the Data Sharing System landing page

	Then I should find where I can upload a CSV file

	And I should see some information about my obligations as a platform

#Clear Navigation:

	When I explore the landing page

	Then there should be a clear and intuitive navigation menu that guides me to other relevant sections of the application

#Brand Guidelines:

	#When viewing the landing page

	#Then it should have visual elements consistent with branding guidelines


Examples:
	| UserName         | UserType             | Environment | ExpectedResult |
	| CEUSTST          | CodeEnforementStaff  | all         |     pass           |
	#| CEUATST          | CodeEnforcementAdmin | all         |                |
	#| STRDSSAitbnbtest | PlatformUser         | test        |                |
	#| STRDSSVrboTest   | PlatformUser         | test        |                |
	#| STRDSSLg1Test    | LocalGovernmentUser  | test        |                |
	#| STRDSSLg2Test    | LocalGovernmentUser  | test        |                |
	#| STRDSSVrboDev    |                      | dev         |                |
	#| STRDSSLg1Dev     |                      | dev         |                |
	#| STRDSSLg2T       |                      |             |                |




