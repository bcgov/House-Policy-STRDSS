import { Injectable } from '@angular/core';
import { DashboardCard } from '../models/dashboard-card';
import { ceu_action, listing_file_upload, listing_read, takedown_action, user_write } from '../consts/permissions.const';
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

  getCardsPerUserType(user: User): Array<DashboardCard> {
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

    return cardsPerUser;
  }

  private initCards() {
    this.cards = [
      {
        title: 'Platform Upload History',
        accessPermission: listing_read,
        description: 'View all platform upload history here',
        buttonIcon: '',
        buttonText: 'Platform Upload History',
        route: '',
        isButtonDisabled: true,
        hidden: true,
        boxId: 'platformUploadHistory_box',
        buttonId: 'platformUploadHistory_btn',
      },
      {
        accessPermission: ceu_action,
        buttonIcon: '',
        buttonText: 'View Delisting Escalation Requests',
        description: 'View all delisting escalation requests sent by local governments',
        route: '',
        title: 'View All Escalation Requests',
        isButtonDisabled: true,
        boxId: 'viewAllEscalationRequests_box',
        buttonId: 'viewAllEscalationRequests_btn',
      },
      {
        accessPermission: ceu_action,
        buttonIcon: '',
        buttonText: 'Send Provincial Compliance Order',
        description: 'Send Provincial Compliance Orders to platforms that have not taken action on De-listing requests',
        route: '',
        title: 'Send Provincial Compliance Order',
        isButtonDisabled: true,
        boxId: 'sendProvincialComplianceOrder_box',
        buttonId: 'sendProvincialComplianceOrder_btn',
      },
      {
        accessPermission: user_write,
        buttonIcon: '',
        buttonText: 'Manage Access Requests',
        description: 'Process new requests for system access',
        route: '/user-management',
        title: 'Manage Access Requests',
        boxId: 'manageAccessRequests_box',
        buttonId: 'manageAccessRequests_btn',
      },
      {
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Policy Guidance',
        description: 'View the policy guidance for sending Notices, Takedown Requests and Escalations',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-localgovernment',
        title: 'Policy Guidance',
        isItOrgTypeBased: true,
        orgType: 'LG',
        boxId: 'viewPolicyGuidance_box',
        buttonId: 'viewPolicyGuidance_btn',
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Escalate to CEU',
        description: 'Notify the Compliance and Enforcement Unit that a platform has failed to remove a listing',
        route: '',
        title: 'Escalate to CEU',
        isButtonDisabled: true,
        hidden: true,
        boxId: 'escalateToCeu_box',
        buttonId: 'escalateToCeu_btn',
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
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Send Takedown Letter',
        description: 'Send a request to an STR platform to remove a non-compliant listing. A Request can be be initiated after a period of 8- 90 days after sending a Notice of Non-Compliance.',
        route: '/delisting-request',
        title: 'Send Takedown Request',
        boxId: 'sendTakedownLetter_box',
        buttonId: 'sendTakedownLetter_btn',
      },
      {
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Policy Guidance',
        description: 'View the guidelines for uploading data and actioning takedown requests',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-platforms',
        title: 'Guideline for Platforms',
        isItOrgTypeBased: true,
        orgType: 'Platform',
        boxId: 'viewPolicyGuidanceForPlatforms_box',
        buttonId: 'viewPolicyGuidanceForPlatforms_btn',
      },
      {
        accessPermission: listing_file_upload,
        buttonIcon: '',
        buttonText: 'Upload data',
        description: 'Upload listing data to the system ',
        route: '',
        title: 'Upload Listing Data',
        isButtonDisabled: true,
        boxId: 'uploadListingData_box',
        buttonId: 'uploadListingData_btn',
      },
    ]
  }
}
