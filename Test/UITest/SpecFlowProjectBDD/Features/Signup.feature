Feature: Signup
	This is an example working Specflow feature file. 

	Requirements:
	1) Visual Studio 2022 Community Edition with C#
	2) Visual Studio Specflow Extension
	3) Chrome
	4) The version of ChromeDriver.exe must match the correct version of Chrome. The version included here is for Chrome Version 122.0.6261.95 
	5) XUnit Test Framework Extension
	6) Running containers from Coding exercise 



@mytag
Scenario: Signup
	Given I am a user
	When I navigate to the STR Homepage ("http://localhost:5002")
	When I click the Signup Button
	Then I am presented with a signup dialog 