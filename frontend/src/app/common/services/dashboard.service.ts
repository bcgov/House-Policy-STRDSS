import { Injectable } from '@angular/core';
import { DashboardCard, DashboardCardSections } from '../models/dashboard-card';
import { ceu_action, licence_file_upload, listing_file_upload, listing_read, platform_write, role_read, takedown_action, upload_history_read, user_write } from '../consts/permissions.const';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private cards!: Array<DashboardCard>;

  constructor() {
    this.initCards();
  }

  getCards(): Array<DashboardCard> {
    return [...this.cards];
  }

  getCardsPerUserType(user: User): DashboardCardSections {
    const cardsPerUser = new Array<DashboardCard>();

    this.cards.forEach((card) => {
      if (!card.hidden) {
        if (card.isItOrgTypeBased) {
          if (user.organizationType === card.orgType) {
            cardsPerUser.push(card);
          }
        } else {
          if (user.permissions.includes(card.accessPermission)) {
            cardsPerUser.push(card);
          }
        }
      }
    });

    const sections: DashboardCardSections = {
      main: cardsPerUser.filter(x => x.section === 'main'),
      admin: cardsPerUser.filter(x => x.section === 'admin'),
      forms: cardsPerUser.filter(x => x.section === 'forms'),
      info: cardsPerUser.filter(x => x.section === 'info'),
    }

    return sections;
  }

  private initCards(): void {
    this.cards = [
      {
        title: 'Aggregated Listing Data',
        accessPermission: listing_read,
        buttonIcon: '',
        buttonText: 'View Aggregated Listing Data',
        description: 'View aggregated platform listing data for your community',
        route: 'aggregated-listings',
        boxId: 'aggregated_listings_box',
        buttonId: 'aggregated_listings_btn',
        section: 'main',
      },
      {
        title: 'Listing Data',
        accessPermission: listing_read,
        buttonIcon: '',
        buttonText: 'View Listing Data',
        description: 'View platform listing data for your community',
        route: 'listings',
        boxId: 'listings_box',
        buttonId: 'listings_btn',
        section: 'main',
      },
      {
        title: 'Download Listing Data',
        accessPermission: listing_read,
        buttonIcon: '',
        buttonText: 'Download Listing Data',
        description: 'Download listing data for your community',
        route: 'export-listings',
        boxId: 'export-listings_box',
        buttonId: 'export-listings_btn',
        section: 'main',
      },
      {
        title: 'Upload Platform Data',
        accessPermission: listing_file_upload,
        buttonIcon: '',
        buttonText: 'Upload data',
        description: 'Upload platform data to the system ',
        route: 'upload-listing-data',
        boxId: 'uploadPlatformData_box',
        buttonId: 'uploadPlatformData_btn',
        section: 'main',
      },
      {
        title: 'Platform Upload History',
        accessPermission: upload_history_read,
        description: 'View all platform upload history here',
        buttonIcon: '',
        buttonText: 'Platform Upload History',
        route: '/upload-listing-history',
        boxId: 'platformUploadHistory_box',
        buttonId: 'platformUploadHistory_btn',
        section: 'main',
      },
      {
        title: 'Upload Business Licence Data',
        accessPermission: licence_file_upload,
        buttonIcon: '',
        buttonText: 'Upload Business Licence Data',
        description: 'Upload your local government’s business licence data',
        route: '/upload-business-licence-data',
        boxId: 'uploadBlData_box',
        buttonId: 'uploadBlData_btn',
        section: 'main',
      },
      {
        title: 'User Management',
        accessPermission: user_write,
        buttonIcon: '',
        buttonText: 'User Management',
        description: 'Process new requests for system access',
        route: '/user-management',
        boxId: 'manageAccessRequests_box',
        buttonId: 'manageAccessRequests_btn',
        section: 'admin',
      },
      {
        title: 'Manage Roles and Permissions',
        accessPermission: role_read,
        buttonIcon: '',
        buttonText: 'Manage Roles And Permissions',
        description: 'Add or edit roles and permissions',
        route: '/roles',
        boxId: 'roleManagement_box',
        buttonId: 'roleManagement_btn',
        section: 'admin',
      },
      {
        title: 'Manage Platforms',
        accessPermission: platform_write,
        buttonIcon: '',
        buttonText: 'Manage Platforms',
        description: 'Manage platform information',
        route: '/platform-management',
        boxId: 'platformManagement_box',
        buttonId: 'platformManagement_btn',
        section: 'admin',
      },
      {
        title: 'Guidance for Local Government',
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Local Government Guide',
        description: 'View the policy guidance for sending Notices, Takedown Requests and Escalations',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-localgovernment',
        isItOrgTypeBased: true,
        orgType: 'LG',
        boxId: 'viewPolicyGuidance_box',
        buttonId: 'viewPolicyGuidance_btn',
        section: 'info',
      },
      {
        title: 'Guideline for Platforms',
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Platform Guide',
        description: 'Understand requirements for STR platforms under the STRAA',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-platforms',
        isItOrgTypeBased: true,
        orgType: 'Platform',
        boxId: 'viewPolicyGuidanceForPlatforms_box',
        buttonId: 'viewPolicyGuidanceForPlatforms_btn',
        section: 'info',
      },
      {
        title: 'User Guide',
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View User Guide',
        description: 'See detailed information about how the STR Data Portal works',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/strdataportal-support',
        isItOrgTypeBased: true,
        orgType: 'LG',
        boxId: 'viewUserGuide_box',
        buttonId: 'viewUserGuide_btn',
        section: 'info',
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Send Notice',
        description: 'Notify an STR host and platform that a listing is not compliant with a business licence requirement. A notice must be sent prior to sending a Takedown Request',
        route: '/compliance-notice',
        title: 'Send Notice of Non-Compliance',
        boxId: 'sendNotice_box',
        buttonId: 'sendNotice_btn',
        section: 'forms',
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Send Takedown Letter',
        description: 'Send a request to an STR platform to remove a non-compliant listing. A Takedown Request can be sent within a period of 5 to 90 days after a Notice of Non-Compliance is delivered.',
        route: '/delisting-request',
        title: 'Send Takedown Request',
        boxId: 'sendTakedownLetter_box',
        buttonId: 'sendTakedownLetter_btn',
        section: 'forms',
      },
    ];
  }
}
