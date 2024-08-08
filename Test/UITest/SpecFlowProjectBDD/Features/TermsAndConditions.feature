Feature: TermsAndConditions

Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-57
#, https://hous-hpb.atlassian.net/browse/DSS-104

@TermsAndConditions
Scenario: TermsAndConditions
#User Authentication:

	Given User "<Email>" is enabled, approved, has the correct roles "<RoleName>", but has not accepted TOC

	Given that I am an authenticated User "<UserName>" and the expected result is "<ExpectedResult>" and I am a "<RoleName>" user

	When I log in or access the system

	Then I have not previously accepted the terms and conditions after access approval

	Then I should be prompted to accept the terms and conditions

#Terms and Conditions Page:

	When prompted to accept the terms and conditions

	Then I should be directed to a dedicated page on the HOUS website in a new tab displaying the complete and updated terms and conditions of system usage

#Acceptance Confirmation:

	When I return to the system after reviewing the terms and conditions

	Then there should be a clear option, such as a checkbox or button, allowing me to confirm my acceptance

#Date of Acceptance:

	When accepting the terms and conditions

	Then the system should record and display the date and time of my acceptance

#Access Restriction for Non-Acceptance:

	When I attempt to access system features

	And I haven't accepted the terms and conditions

	Then I should be restricted from performing certain actions until I complete the acceptance process

#Accessible Language:

	When presenting the terms and conditions

	Then the language should be clear, concise, and accessible to ensure user understanding

#Confirmation Message:

	When I successfully accept the terms and conditions

	Then I should receive a confirmation message indicating that my acceptance has been recorded

	And I should be directed to the landing page for my role

#Bypass Terms and Conditions for CEU users

	Given I am an IDIR authenticated user

	Then TOC flag will be set to true

	Then I will not have to accept the TOC in order to access the system

#RoleName is the value for user_role_cd in the public.dss_user_role DB table
Examples:
	| UserName | RoleName  | Email             | Environment | ExpectedResult |
	| CEUATST  | ceu_admin | ceuatst@gov.bc.ca | all         | pass           |
	#| STRDSSLg1Dev | lg_staff  | ceuatst@gov.bc.ca | all         | pass           |




