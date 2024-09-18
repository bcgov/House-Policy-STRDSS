Feature: ViewAggregatedListings
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-505

@LandingPage
Scenario: ViewAggregatedListings 
#User Authentication
	#Given that I am an authenticated user "<UserName>" with the necessary permissions to view listings View listings (listing_read)  and the expected result is
	Given that I am an authenticated user "<UserName>" with the necessary permissions to view listings and the expected result is "<ExpectedResult>" and I am a "<UserType>" user



#Access to Data Portal:

	When I access the Data Portal

#View Aggregated Listings

	Given that I am on the aggregated listing page

	Then I should see a parent row for each STR “property grouping “with information that is common to all listings associated with the group

	And the ability to view drop down child rows under each parent row  so that I can review all listings that are associated with a property grouping

#Parent Row 

	Given I am viewing a listing on the aggregated listing page

	Then I should see information that is “common” to all listings associated with a property, including:

	#Primary Host name
	#
	#Address (Best Match) 
	#
	#Nights stayed YTD (from listing data)
	#
	#Business licence number
	#
	#Last Action
	#
	#Last Action Date

	And I should have the option to expand a dropdown to view all child rows with listings associated with the parent row

#Child rows

	When I expand the dropdown to view all child rows,

	Then I should see key information for each listing, including:

	#status/ flags
	#
	#platform name
	#
	#listing ID
	#
	#Link to Listing Details
	#
	#Address (Best match)
	#
	#Nights stayed (YTD)
	#
	#Business Licence 
	#
	#last action
	#
	#last action date


Examples:
	| UserName     | UserType | Environment | ExpectedResult |
	| STRDSSLg1Dev | lg_staff | dev         | pass           |






