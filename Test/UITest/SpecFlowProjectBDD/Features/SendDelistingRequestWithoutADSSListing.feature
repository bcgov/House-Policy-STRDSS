Feature: SendDelistingRequestWithoutADSSListingFeature
Link to a feature: https://hous-hpb.atlassian.net/browse/DSS-71

@Delisting
Scenario: SendDelistingRequestWithoutADSSListing
#User Authentication
Given I am an authenticated LG staff member

When I navigate to the delisting request feature

#Input Form
Then I should be presented with an input form that includes fields for the listing URL

And I should be presented with a field to select which platform to send the request to 


And I should see an optional field for adding a LG staff user email address to be copied on the email

#ListingURLField

When Entering the listing URL "ListingURL"

Then The system should validate the URL format and ensure it is a valid link to the property listing

#PlatformField
When selecting the platform 

Then the system should present a list of available platform options to populate the field

#DelistingRequestMessage

When all required fields are entered

Then I see a template delisting request message that will be sent to both the platform

#SendDelistingRequest

When I submit the form with valid information

Then the system should send the delisting request message to the platform email addresses associated with the selected platform

#ConfirmationMessage

When successful submission

Then I should receive a confirmation message indicating that the delisting request has been sent

Then I should be copied on the email
#NotificationToPlatformAndHost

When the delisting request is submitted

Then the platform and host should receive email notifications containing the delisting request and instructions for compliance

#FrontEndErrorHandling

When there are issues with the submission, such as invalid email addresses or a missing URL

Then the system should provide clear error messages guiding me on how to correct the issues




