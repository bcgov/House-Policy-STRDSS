Feature: STRDSSLandingPage
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-4

@LandingPage
Scenario: STRDSSLandingPage
#User Authentication
	Given that I am an authenticated User "<UserName>" and the expected result is "<ExpectedResult>" and I am a "<UserType>" user

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
	| UserName         | UserType       | Environment | ExpectedResult |
	| CEUATST          | ceu_admin      | all         | pass           |
	| CEUSTST          | ceu_staff      | all         | pass           |
	| STRDSSLg1Dev     | lg_staff       | dev         | pass           |
	#| STRDSSLg2Dev     | lg_staff       | dev         | pass           |
	| STRDSSVrboDev    | platform_staff | dev         | pass           |
	| STRDSSAirbnbDev | platform_staff | test        | pass           |
	#| STRDSSVrboTest   | platform_staff | test        | pass           |
	#| STRDSSLg1Test    | lg_staff       | test        | pass           |
	#| STRDSSLg2Test    | lg_staff       | test        | Pass           |





