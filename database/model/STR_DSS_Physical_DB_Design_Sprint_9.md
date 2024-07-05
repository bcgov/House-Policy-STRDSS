#Physical Database Design (Sprint 9)
Generated using [DbSchema](https://dbschema.com)




### Physical Data Diagram (Sprint 9)
![img](./PhysicalDataDiagram(Sprint9).svg)



### Table dss.dss_access_request_status 
A potential status for a user access request (e.g. Requested, Approved, or Denied)

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | access\_request\_status\_cd| varchar(25)  | System-consistent code for the request status |
| * | access\_request\_status\_nm| varchar(250)  | Business term for the request status |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_access\_request\_status\_pk | ON access\_request\_status\_cd|



### Table dss.dss_email_message 
A message that is sent to one or more recipients via email

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | email\_message\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * &#11016; | email\_message\_type| varchar(50)  | Foreign key |
| * &#128270; | message\_delivery\_dtm| timestamptz  | A timestamp indicating when the message delivery was initiated |
| * | message\_template\_dsc| varchar(4000)  | The full text or template for the message that is sent |
| * | is\_submitter\_cc\_required| boolean  | Indicates whether the user initiating the message should receive a copy of the email |
|  | is\_host\_contacted\_externally| boolean  | Indicates whether the the property host has already been contacted by external means |
|  | is\_with\_standard\_detail| boolean  | Indicates whether message body should include text a block of detail text that is standard for the message type |
|  | lg\_phone\_no| varchar(30)  | A phone number associated with a Local Government contact |
|  | unreported\_listing\_no| varchar(50)  | The platform issued identification number for the listing (if not included in a rental listing report) |
|  | host\_email\_address\_dsc| varchar(320)  | E-mail address of a short term rental host (directly entered by the user as a message recipient) |
|  | lg\_email\_address\_dsc| varchar(320)  | E-mail address of a local government contact (directly entered by the user as a message recipient) |
|  | cc\_email\_address\_dsc| varchar(4000)  | E-mail address of a secondary message recipient (directly entered by the user) |
|  | unreported\_listing\_url| varchar(4000)  | User-provided URL for a short-term rental platform listing that is the subject of the message |
|  | lg\_str\_bylaw\_url| varchar(4000)  | User-provided URL for a local government bylaw that is the subject of the message |
|  | custom\_detail\_txt| varchar(4000)  | Free form text that should be included in the message body |
| &#128270; &#11016; | concerned\_with\_rental\_listing\_id| bigint  | Foreign key |
| &#11016; | initiating\_user\_identity\_id| bigint  | Foreign key |
| &#11016; | affected\_by\_user\_identity\_id| bigint  | Foreign key |
| &#11016; | involved\_in\_organization\_id| bigint  | Foreign key |
| &#11016; | batching\_email\_message\_id| bigint  | Foreign key |
| &#11016; | requesting\_organization\_id| bigint  | Foreign key |
|  | external\_message\_no| varchar(50)  | External identifier for tracking the message delivery progress |
|  | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_email\_message\_pk | ON email\_message\_id|
| &#128270;  | dss\_email\_message\_i1 | ON concerned\_with\_rental\_listing\_id, message\_delivery\_dtm|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_email_message_fk_initiated_by | ( initiating\_user\_identity\_id ) ref [dss.dss\_user\_identity](#dss\_user\_identity) (user\_identity\_id) |
|  | dss_email_message_fk_affecting | ( affected\_by\_user\_identity\_id ) ref [dss.dss\_user\_identity](#dss\_user\_identity) (user\_identity\_id) |
|  | dss_email_message_fk_involving | ( involved\_in\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_email_message_fk_communicating | ( email\_message\_type ) ref [dss.dss\_email\_message\_type](#dss\_email\_message\_type) (email\_message\_type) |
|  | dss_email_message_fk_batched_in | ( batching\_email\_message\_id ) ref [dss.dss\_email\_message](#dss\_email\_message) (email\_message\_id) |
|  | dss_email_message_fk_requested_by | ( requesting\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_email_message_fk_included_in | ( concerned\_with\_rental\_listing\_id ) ref [dss.dss\_rental\_listing](#dss\_rental\_listing) (rental\_listing\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_email_message_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_email\_message\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_email\_message FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_email_message_type 
The type or purpose of a system generated message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification)

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | email\_message\_type| varchar(50)  | System-consistent code for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification) |
| * | email\_message\_type\_nm| varchar(250)  | Business term for the type or purpose of the message (e.g. Notice of Takedown, Takedown Request, Delisting Warning, Delisting Request, Access Granted Notification, Access Denied Notification) |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_email\_message\_type\_pk | ON email\_message\_type|



### Table dss.dss_listing_status_type 
A potential status for a CURRENT RENTAL LISTING (e.g. New, Active, Inactive, Reassigned, Taken Down)

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | listing\_status\_type| varchar(2)  | System-consistent code for the listing status (e.g. N, A, I, R, T) |
| * | listing\_status\_type\_nm| varchar(50)  | Business term for the listing status (e.g. New, Active, Inactive, Reassigned, Taken Down) |
| * | listing\_status\_sort\_no| smallint  |  |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_listing\_status\_type\_pk | ON listing\_status\_type|



### Table dss.dss_organization 
A private company or governing body component that plays a role in short term rental reporting or enforcement

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | organization\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * &#128270; &#11016; | organization\_type| varchar(25)  | Foreign key |
| * &#128269; | organization\_cd| varchar(25)  | An immutable system code that identifies the organization (e.g. CEU, AIRBNB) |
| * | organization\_nm| varchar(250)  | A human-readable name that identifies the organization (e.g. Corporate Enforecement Unit, City of Victoria) |
|  | economic\_region\_dsc| varchar(100)  | A free form description of the economic region to which a Local Government Subdivision belongs |
|  | is\_lg\_participating| boolean  | Indicates whether a LOCAL GOVERNMENT ORGANIZATION participates in Short Term Rental Data Sharing |
|  | is\_principal\_residence\_required| boolean  | Indicates whether a LOCAL GOVERNMENT SUBDIVISION is subject to Provincial Principal Residence Short Term Rental restrictions |
|  | is\_business\_licence\_required| boolean  | Indicates whether a LOCAL GOVERNMENT SUBDIVISION requires a business licence for Short Term Rental operation |
|  | area\_geometry| geometry  | the multipolygon shape identifying the boundaries of a local government subdivision |
| &#128270; &#11016; | managing\_organization\_id| bigint  | Self-referential hierarchical foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_organization\_pk | ON organization\_id|
| &#128270;  | dss\_organization\_i1 | ON organization\_type|
| &#128269;  | dss\_organization\_uk | ON organization\_cd|
| &#128270;  | dss\_organization\_i2 | ON managing\_organization\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_organization_fk_managed_by | ( managing\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_organization_fk_treated_as | ( organization\_type ) ref [dss.dss\_organization\_type](#dss\_organization\_type) (organization\_type) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_organization_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_organization\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_organization FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_organization_contact_person 
A person who has been identified as a notable contact for a particular organization

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  | organization\_contact\_person\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | email\_address\_dsc| varchar(320)  | E-mail address given for the contact by the organization |
|  | is\_primary| boolean  | Indicates whether the contact should receive all communications directed at the organization |
|  | given\_nm| varchar(25)  | A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name) |
|  | family\_nm| varchar(25)  | A name that is often shared amongst members of the same family (commonly known as a surname within some cultures) |
|  | phone\_no| varchar(30)  | Phone number given for the contact by the organization (contains only digits) |
| * &#11016; | contacted\_through\_organization\_id| bigint  | Foreign key |
| &#11016; | email\_message\_type| varchar(50)  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_organization\_contact\_person\_pk | ON organization\_contact\_person\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_organization_contact_person_fk_contacted_for | ( contacted\_through\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_organization_contact_person_fk_subscribed_to | ( email\_message\_type ) ref [dss.dss\_email\_message\_type](#dss\_email\_message\_type) (email\_message\_type) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_organization_contact_person_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_organization\_contact\_person\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_organization\_contact\_person FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_organization_type 
A level of government or business category

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | organization\_type| varchar(25)  | System-consistent code for a level of government or business category |
| * | organization\_type\_nm| varchar(250)  | Business term for a level of government or business category |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_organization\_type\_pk | ON organization\_type|



### Table dss.dss_physical_address 
A property address that includes any verifiable BC attributes

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | physical\_address\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * &#128270; | original\_address\_txt| varchar(250)  | The source-provided address of a short-term rental offering |
|  | match\_result\_json| json  | Full JSON result of the source address matching attempt |
| &#128270; | match\_address\_txt| varchar(250)  | The sanitized physical address (returned as fullAddress) that has been derived from the original |
|  | match\_score\_amt| smallint  | The relative score returned from the address matching attempt |
|  | unit\_no| varchar(50)  | The unitNumber (suite) returned by the address match (e.g. 100) |
|  | civic\_no| varchar(50)  | The civicNumber (building number) returned by the address match (e.g. 1285) |
|  | street\_nm| varchar(100)  | The streetName returned by the address match (e.g. Pender) |
|  | street\_type\_dsc| varchar(50)  | The streetType returned by the address match (e.g. St or Street) |
|  | street\_direction\_dsc| varchar(50)  | The streetDirection returned by the address match (e.g. W or West) |
|  | locality\_nm| varchar(100)  | The localityName (community) returned by the address match (e.g. Vancouver) |
|  | locality\_type\_dsc| varchar(50)  | The localityType returned by the address match (e.g. City) |
|  | province\_cd| varchar(5)  | The provinceCode returned by the address match |
|  | site\_no| varchar(50)  | The siteID returned by the address match |
|  | block\_no| varchar(50)  | The blockID returned by the address match |
|  | location\_geometry| geometry  | The computed location point of the matched address |
|  | is\_exempt| boolean  | Indicates whether the address has been identified as exempt from Short Term Rental regulations |
|  | is\_match\_verified| boolean  | Indicates whether the matched address has been verified as correct for the listing by the responsible authorities |
|  | is\_changed\_original\_address| boolean  | Indicates whether the original address has received a different property address from the platform in the last reporting period |
|  | is\_match\_corrected| boolean  | Indicates whether the matched address has been manually changed to one that is verified as correct for the listing |
|  | is\_system\_processing| boolean  | Indicates whether the physical address is being processed by the system and may not yet be in its final form |
| &#128270; &#11016; | containing\_organization\_id| bigint  | Foreign key |
| &#11016; | replacing\_physical\_address\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_physical\_address\_pk | ON physical\_address\_id|
| &#128270;  | dss\_physical\_address\_i1 | ON original\_address\_txt|
| &#128270;  | dss\_physical\_address\_i2 | ON match\_address\_txt|
| &#128270;  | dss\_physical\_address\_i3 | ON containing\_organization\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_physical_address_fk_contained_in | ( containing\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_physical_address_fk_replaced_by | ( replacing\_physical\_address\_id ) ref [dss.dss\_physical\_address](#dss\_physical\_address) (physical\_address\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_physical_address_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_physical\_address\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_physical\_address FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_rental_listing 
A rental listing snapshot that is either relevant to a specific monthly report, or is the current, master version

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | rental\_listing\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * &#128270; | platform\_listing\_no| varchar(50)  | The platform issued identification number for the listing |
|  | platform\_listing\_url| varchar(4000)  | URL for the short-term rental platform listing |
|  | business\_licence\_no| varchar(100)  | The local government issued licence number that applies to the rental offering |
|  | bc\_registry\_no| varchar(50)  | The Short Term Registry issued permit number |
| * | is\_current| boolean  | Indicates whether the RENTAL LISTING VERSION is a CURRENT RENTAL LISTING (if it is a copy of the most current REPORTED RENTAL LISTING (having the same listing number for the same offering platform) |
|  | is\_active| boolean  | Indicates whether a CURRENT RENTAL LISTING was included in the most recent RENTAL LISTING REPORT |
|  | is\_new| boolean  | Indicates whether a CURRENT RENTAL LISTING appeared for the first time in the last reporting period |
|  | is\_taken\_down| boolean  | Indicates whether a CURRENT RENTAL LISTING has been reported as taken down by the offering platform |
|  | is\_changed\_original\_address| boolean  | Indicates whether a CURRENT RENTAL LISTING has received a different property address in the last reporting period |
|  | is\_changed\_address| boolean  | Indicates whether a CURRENT RENTAL LISTING has been subjected to address match changes by a user |
|  | is\_lg\_transferred| boolean  | Indicates whether a CURRENT RENTAL LISTING has been transferred to a different Local Goverment Organization as a result of address changes |
|  | is\_entire\_unit| boolean  | Indicates whether the entire dwelling unit is offered for rental (as opposed to a single bedroom) |
|  | available\_bedrooms\_qty| smallint  | The number of bedrooms in the dwelling unit that are available for short term rental |
|  | nights\_booked\_qty| smallint  | The number of nights that short term rental accommodation services were provided during the reporting period |
|  | separate\_reservations\_qty| smallint  | The number of separate reservations that were taken during the reporting period |
| * &#128270; &#11016; | offering\_organization\_id| bigint  | Foreign key |
| &#128270; &#11016; | including\_rental\_listing\_report\_id| bigint  | Foreign key |
| &#128270; &#11016; | derived\_from\_rental\_listing\_id| bigint  | Foreign key |
| &#128270; &#11016; | locating\_physical\_address\_id| bigint  | Foreign key |
| &#128270; &#11016; | listing\_status\_type| varchar(2)  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_rental\_listing\_pk | ON rental\_listing\_id|
| &#128270;  | dss\_rental\_listing\_i1 | ON offering\_organization\_id, platform\_listing\_no|
| &#128270;  | dss\_rental\_listing\_i2 | ON including\_rental\_listing\_report\_id|
| &#128270;  | dss\_rental\_listing\_i3 | ON derived\_from\_rental\_listing\_id|
| &#128270;  | dss\_rental\_listing\_i4 | ON locating\_physical\_address\_id|
| &#128270;  | dss\_rental\_listing\_i5 | ON listing\_status\_type, offering\_organization\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_rental_listing_fk_offered_by | ( offering\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_rental_listing_fk_included_in | ( including\_rental\_listing\_report\_id ) ref [dss.dss\_rental\_listing\_report](#dss\_rental\_listing\_report) (rental\_listing\_report\_id) |
|  | dss_rental_listing_fk_located_at | ( locating\_physical\_address\_id ) ref [dss.dss\_physical\_address](#dss\_physical\_address) (physical\_address\_id) |
|  | dss_rental_listing_fk_generating | ( derived\_from\_rental\_listing\_id ) ref [dss.dss\_rental\_listing](#dss\_rental\_listing) (rental\_listing\_id) |
|  | dss_rental_listing_fk_classified_as | ( listing\_status\_type ) ref [dss.dss\_listing\_status\_type](#dss\_listing\_status\_type) (listing\_status\_type) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_rental_listing_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_rental\_listing\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_rental\_listing FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_rental_listing_contact 
A person who has been identified as a notable contact for a particular rental listing

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  | rental\_listing\_contact\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | is\_property\_owner| boolean  | Indicates a person with the legal right to the unit being short-term rental |
|  | listing\_contact\_nbr| smallint  | Indicates which of the five possible supplier hosts is represented by this contact |
|  | supplier\_host\_no| varchar(50)  | The platform identifier for the supplier host |
|  | full\_nm| varchar(100)  | The full name of the contact person as inluded in the listing |
|  | phone\_no| varchar(30)  | Phone number given for the contact |
|  | fax\_no| varchar(30)  | Facsimile numbrer given for the contact |
|  | full\_address\_txt| varchar(250)  | Mailing address given for the contact |
|  | email\_address\_dsc| varchar(320)  | E-mail address given for the contact |
| * &#128270; &#11016; | contacted\_through\_rental\_listing\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_rental\_listing\_contact\_pk | ON rental\_listing\_contact\_id|
| &#128270;  | dss\_rental\_listing\_contact\_i1 | ON contacted\_through\_rental\_listing\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_rental_listing_contact_fk_contacted_for | ( contacted\_through\_rental\_listing\_id ) ref [dss.dss\_rental\_listing](#dss\_rental\_listing) (rental\_listing\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_rental_listing_contact_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_rental\_listing\_contact\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_rental\_listing\_contact FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_rental_listing_extract 
A prebuilt report that is specific to a subset of rental listings

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  | rental\_listing\_extract\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | rental\_listing\_extract\_nm| varchar(250)  | A description of the information contained in the extract |
| * | is\_pr\_requirement\_filtered| boolean  | Indicates whether the report is filtered by jurisdictional principal residence requirement |
|  | source\_bin| bytea  | The binary image of the information in the report |
| &#11016; | filtering\_organization\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_rental\_listing\_extract\_pk | ON rental\_listing\_extract\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_rental_listing_extract_fk_filtered_by | ( filtering\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_rental_listing_extract_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_rental\_listing\_extract\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_rental\_listing\_extract FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_rental_listing_report 
A platform-specific collection of rental listing information that is relevant to a specific month

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | rental\_listing\_report\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * &#128269; | report\_period\_ym| date  | The month to which the listing information is relevant (always set to the first day of the month) |
| * | is\_current| boolean  | Indicates whether the rental listing version is the most recent one reported by the platform |
| * &#128269; &#11016; | providing\_organization\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_rental\_listing\_report\_pk | ON rental\_listing\_report\_id|
| &#128269;  | dss\_rental\_listing\_report\_uk | ON providing\_organization\_id, report\_period\_ym|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_rental_listing_report_fk_provided_by | ( providing\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_rental_listing_report_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_rental\_listing\_report\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_rental\_listing\_report FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_upload_delivery 
A delivery of uploaded information that is relevant to a specific month

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | upload\_delivery\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | upload\_delivery\_type| varchar(25)  | Identifies the treatment applied to ingesting the uploaded information |
|  | report\_period\_ym| date  | The month to which the delivery batch is relevant (always set to the first day of the month) |
| * | source\_hash\_dsc| varchar(256)  | The hash value of the information that was uploaded |
|  | source\_bin| bytea  | The binary image of the information that was uploaded |
| * &#11016; | providing\_organization\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_upload\_delivery\_pk | ON upload\_delivery\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_upload_delivery_fk_provided_by | ( providing\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_upload_delivery_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_upload\_delivery\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_upload\_delivery FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_upload_line 
An upload delivery line that has been extracted from the source

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  | upload\_line\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | is\_validation\_failure| boolean  | Indicates that there has been a validation problem that prevents successful ingestion of the upload line |
| * | is\_system\_failure| boolean  | Indicates that a system fault has prevented complete ingestion of the upload line |
| * | is\_processed| boolean  | Indicates that no further ingestion attempt is required for the upload line |
| * | source\_organization\_cd| varchar(25)  | An immutable system code identifying the organization who created the information in the upload line (e.g. AIRBNB) |
| * &#128269; | source\_record\_no| varchar(50)  | The immutable identification number for the source record, such as a rental listing number |
| * | source\_line\_txt| varchar(32000)  | Full text of the uploaod line |
|  | error\_txt| varchar(32000)  | Freeform description of the problem found while attempting to interpret the report line |
| * &#128269; &#11016; | including\_upload\_delivery\_id| bigint  | Foreign key |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_upload\_line\_pk | ON upload\_line\_id|
| &#128269;  | dss\_upload\_line\_uk | ON including\_upload\_delivery\_id, source\_record\_no|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_upload_line_fk_included_in | ( including\_upload\_delivery\_id ) ref [dss.dss\_upload\_delivery](#dss\_upload\_delivery) (upload\_delivery\_id) |




### Table dss.dss_user_identity 
An externally defined domain directory object representing a potential application user or group

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | user\_identity\_id| bigint GENERATED ALWAYS AS IDENTITY  | Unique generated key |
| * | user\_guid| uuid  | An immutable unique identifier assigned by the identity provider |
| * | display\_nm| varchar(250)  | A human-readable full name that is assigned by the identity provider (this may include a preferred name and/or business unit name) |
| * | identity\_provider\_nm| varchar(25)  | A directory or domain that authenticates system users to allow access to the application or its API  (e.g. idir, bceidbusiness) |
| * | is\_enabled| boolean  | Indicates whether access is currently permitted using this identity |
| * &#11016; | access\_request\_status\_cd| varchar(25)  | The current status of the most recent access request made by the user (restricted to Requested, Approved, or Denied) |
|  | access\_request\_dtm| timestamptz  | A timestamp indicating when the most recent access request was made by the user |
|  | access\_request\_justification\_txt| varchar(250)  | The most recent user-provided reason for requesting application access |
|  | given\_nm| varchar(25)  | A name given to a person so that they can easily be identified among their family members (in some cultures, this is often the first name) |
|  | family\_nm| varchar(25)  | A name that is often shared amongst members of the same family (commonly known as a surname within some cultures) |
|  | email\_address\_dsc| varchar(320)  | E-mail address associated with the user by the identity provider |
|  | business\_nm| varchar(250)  | A human-readable organization name that is associated with the user by the identity provider |
|  | terms\_acceptance\_dtm| timestamptz  | A timestamp indicating when the user most recently accepted the published Terms and Conditions of application access |
| &#11016; | represented\_by\_organization\_id| bigint  | Foreign key |
| * | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_user\_identity\_pk | ON user\_identity\_id|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_user_identity_fk_representing | ( represented\_by\_organization\_id ) ref [dss.dss\_organization](#dss\_organization) (organization\_id) |
|  | dss_user_identity_fk_given | ( access\_request\_status\_cd ) ref [dss.dss\_access\_request\_status](#dss\_access\_request\_status) (access\_request\_status\_cd) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_user_identity_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_user\_identity\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_user\_identity FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_user_privilege 
A granular access right or privilege within the application that may be granted to a role

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | user\_privilege\_cd| varchar(25)  | The immutable system code that identifies the privilege |
| * | user\_privilege\_nm| varchar(250)  | The human-readable name that is given for the role |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_user\_privilege\_pk | ON user\_privilege\_cd|



### Table dss.dss_user_role 
A set of access rights and privileges within the application that may be granted to users

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11019; | user\_role\_cd| varchar(25)  | The immutable system code that identifies the role |
| * | user\_role\_nm| varchar(250)  | The human-readable name that is given for the role |
|  | user\_role\_dsc| varchar(200)  | The human-readable description that is given for the role |
|  | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_user\_role\_pk | ON user\_role\_cd|

##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_user_role_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_user\_role\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_user\_role FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_user_role_assignment 
The association of a grantee credential to a role for the purpose of conveying application privileges

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11016; | user\_identity\_id| bigint  | Foreign key |
| * &#128273;  &#11016; | user\_role\_cd| varchar(25)  | Foreign key |
|  | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_user\_role\_assignment\_pk | ON user\_identity\_id, user\_role\_cd|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_user_role_assignment_fk_granted | ( user\_role\_cd ) ref [dss.dss\_user\_role](#dss\_user\_role) (user\_role\_cd) |
|  | dss_user_role_assignment_fk_granted_to | ( user\_identity\_id ) ref [dss.dss\_user\_identity](#dss\_user\_identity) (user\_identity\_id) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_user_role_assignment_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_user\_role\_assignment\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_user\_role\_assignment FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 


### Table dss.dss_user_role_privilege 
The association of a granular application privilege to a role

|Idx |Name |Data Type |Description |
|---|---|---|---|
| * &#128273;  &#11016; | user\_privilege\_cd| varchar(25)  | Foreign key |
| * &#128273;  &#11016; | user\_role\_cd| varchar(25)  | Foreign key |
|  | upd\_dtm| timestamptz  | Trigger-updated timestamp of last change |
|  | upd\_user\_guid| uuid  | The globally unique identifier (assigned by the identity provider) for the most recent user to record a change |


##### Indexes 
|Type |Name |On |
|---|---|---|
| &#128273;  | dss\_user\_role\_privilege\_pk | ON user\_privilege\_cd, user\_role\_cd|

##### Foreign Keys
|Type |Name |On |
|---|---|---|
|  | dss_user_role_privilege_fk_conferred_by | ( user\_role\_cd ) ref [dss.dss\_user\_role](#dss\_user\_role) (user\_role\_cd) |
|  | dss_user_role_privilege_fk_conferring | ( user\_privilege\_cd ) ref [dss.dss\_user\_privilege](#dss\_user\_privilege) (user\_privilege\_cd) |


##### Triggers
|Name |Definition |
|---|---|
### Trigger dss_user_role_privilege_br_iu_tr 
  
 ```
CREATE TRIGGER dss\_user\_role\_privilege\_br\_iu\_tr BEFORE INSERT OR UPDATE ON dss.dss\_user\_role\_privilege FOR EACH ROW EXECUTE FUNCTION dss.dss\_update\_audit\_columns()
``` 

## Schema dss
### Functions 
dss\_containing\_organization\_id 
```

```
dss\_update\_audit\_columns 
```

```


