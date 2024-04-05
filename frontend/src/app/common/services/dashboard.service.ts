import { Injectable } from '@angular/core';
import { DashboardCard } from '../models/dashboard-card';
import { ceu_action, listing_file_upload, listing_read, takedown_action, user_write } from '../consts/permissions.const';

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
      },
      {
        accessPermission: ceu_action,
        buttonIcon: '',
        buttonText: 'View Delisting Escalation Requests',
        description: 'View all delisting escalation requests sent by local governments',
        route: '',
        title: 'View All Escalation Requests',
        isButtonDisabled: true,
      },
      {
        accessPermission: ceu_action,
        buttonIcon: '',
        buttonText: 'Send Provincial Compliance Order',
        description: 'Send Provincial Compliance Orders to platforms that have not taken action on De-listing requests',
        route: '',
        title: 'Send Provincial Compliance Order',
        isButtonDisabled: true,
      },
      {
        accessPermission: user_write,
        buttonIcon: '',
        buttonText: 'Manage Access Requests',
        description: 'Process new requests for system access',
        route: '/user-management',
        title: 'Manage Access Requests',
      },
      {
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Policy Guidance',
        description: 'View the policy guidance for sending Notices, Takedown Requests and Escalations',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-localgovernment',
        title: 'Policy Guidance',
        isItOrgTypeBased: true,
        orgType: 'LG'
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Escalate to CEU',
        description: 'Notify the Compliance and Enforcement Unit that a platform has failed to remove a listing',
        route: '',
        title: 'Escalate to CEU',
        isButtonDisabled: true,
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Send Notice',
        description: 'Send a notice to a short-term rental host and platform that a listing may be removed',
        route: '/compliance-notice',
        title: 'Send Notice of Takedown',
      },
      {
        accessPermission: takedown_action,
        buttonIcon: '',
        buttonText: 'Send Takedown Letter',
        description: 'Send a request to remove a short-term rental listing to a platform',
        route: '/delisting-request',
        title: 'Send Takedown Letter',
      },
      {
        accessPermission: '',
        buttonIcon: '',
        buttonText: 'View Policy Guidance',
        description: 'View the guidelines for uploading data and actioning takedown requests',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-platforms',
        title: 'Guideline for Platforms',
        isItOrgTypeBased: true,
        orgType: 'Platform'
      },
      {
        accessPermission: listing_file_upload,
        buttonIcon: '',
        buttonText: 'Upload data',
        description: 'Upload listing data to the system ',
        route: '',
        title: 'Upload Listing Data',
        isButtonDisabled: true,
      },
    ]
  }
}
