Feature: SendDelistingWarningWithoutADSSListingFeature
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-71

@Delisting
Scenario: SendDelistingWarningWithoutADSSListing
#User Authentication
Given I am an authenticated LG staff member

When I navigate to the delisting warning feature

#Input Form
Then I should be presented with an input form that includes fields for the listing URL, and an optional field for the host email address

And I should be presented with a field to select which platform to send the warning to 

And I should be presented with a dropdown menu to select reason for delisting

And I should see an optional field for Listing ID

And I should see an optional field for adding a LG staff user email address to be copied on the email

#ListingURLField

When Entering the listing URL "ListingURL"

Then The system should validate the URL format and ensure it is a valid link to the property listing

#PlatformField
When selecting the platform 

Then the system should present a list of available platform options to populate the field

#HostEmailAddressField
When entering the optional host email address

Then the system should validate the email format 

#ReasonForDelisting

When I select a reason for delisting

Then the system should present a list of reasons for requesting delisting: No business licence provided, invalid business licence number, expired business licence, or suspended business license

#DelistingWarningMessage

When all required fields are entered

Then I see a template delisting warning message that will be sent to both the platform and host

#SendDelistingRequest

When I submit the form with valid information

Then the system should send the delisting warning message to the provided platform and host email addresses

#ConfirmationMessage

When successful submission

Then I should receive a confirmation message indicating that the delisting warning has been sent

Then I should be copied on the email
#NotificationToPlatformAndHost

When the delisting warning is submitted

Then the platform and host should receive email notifications containing the delisting warning and instructions for compliance

#FrontEndErrorHandling

When there are issues with the submission, such as invalid email addresses or a missing URL

Then the system should provide clear error messages guiding me on how to correct the issues




