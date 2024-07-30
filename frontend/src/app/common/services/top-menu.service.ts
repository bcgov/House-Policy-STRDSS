import { Injectable } from '@angular/core';
import { TopMenuItem } from '../models/top-menu-item';
import { ceu_action, licence_file_upload, listing_file_upload, listing_read, role_read, takedown_action, upload_history_read, user_write } from '../consts/permissions.const';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class TopMenuService {
  private menuItems = new Array<TopMenuItem>();

  constructor() {
    this.initMenuItems();
  }

  getMenuItems(): Array<TopMenuItem> {
    return [...this.menuItems]
  }

  getMenuItemsPerUserType(user: User): Array<TopMenuItem> {
    const itemsPerUser = new Array<TopMenuItem>();

    this.menuItems.forEach((item) => {
      if (!item.hidden) {
        if (item.isItOrgTypeBased) {
          if (user.organizationType === item.orgType) {
            itemsPerUser.push(item);
          }
        } else {
          if (user.permissions.includes(item.accessPermission)) {
            itemsPerUser.push(item);
          }
        }
      }
    })

    return itemsPerUser;
  }

  private initMenuItems(): void {
    this.menuItems = [
      {
        accessPermission: listing_read,
        buttonId: 'listings_mi_btn',
        route: '/listings',
        title: 'View Listings',
        folderName: 'Listings',
      },
      {
        accessPermission: listing_read,
        buttonId: 'export_listings_mi_btn',
        route: '/export-listings',
        title: 'Download Listing Data',
        folderName: 'Listings',
      },
      {
        accessPermission: takedown_action,
        buttonId: 'sendNotice_mi_btn',
        route: '/compliance-notice',
        title: 'Notice of Non-Compliance',
        description: 'Send a request to an STR platform to remove a non-compliant listing. A Takedown Request can be sent within a period of 5 to 90 days after a Notice of Non-Compliance is delivered.',
        folderName: 'Forms',
      },
      {
        accessPermission: takedown_action,
        buttonId: 'sendTakedownLetter_mi_btn',
        route: '/delisting-request',
        description: 'Send a request to an STR platform to remove a non-compliant listing. A Takedown Request can be sent within a period of 5 to 90 days after a Notice of Non-Compliance is delivered.',
        title: 'Takedown Request',
        folderName: 'Forms',
      },
      {
        accessPermission: takedown_action,
        buttonId: 'escalateToCeu_mi_btn',
        route: '',
        disabled: true,
        hidden: true,
        description: 'Notify the Compliance and Enforcement Unit that a platform has failed to remove a listing',
        title: 'Escalation',
        folderName: 'Forms',
      },
      {
        accessPermission: ceu_action,
        buttonId: 'sendProvincialComplianceOrder_mi_btn',
        route: '',
        disabled: true,
        hidden: true,
        description: 'Send Provincial Compliance Orders to platforms that have not taken action on De-listing requests',
        title: 'Provincial Compliance Order',
        folderName: 'Forms',
      },

      {
        accessPermission: listing_file_upload,
        buttonId: 'uploadListingData_mi_btn',
        route: '/upload-listing-data',
        description: 'Upload listing data to the system',
        title: 'Upload Listings',
        folderName: 'Upload',
      },
      {
        accessPermission: upload_history_read,
        buttonId: 'platformUploadHistory_mi_btn',
        route: '/upload-listing-history',
        description: 'View all platform upload history here',
        title: 'Upload History',
        folderName: 'Upload',
      },
      {
        accessPermission: licence_file_upload,
        buttonId: 'businessLicenses_mi_btn',
        route: '',
        disabled: true,
        description: '',
        title: 'Business Licenses',
        folderName: 'Upload',
      },

      {
        accessPermission: user_write,
        buttonId: 'userManagementRequests_mi_btn',
        route: '/user-management',
        description: 'Process new requests for system access',
        title: 'User Management',
        folderName: 'Admin Tools',
      },
      {
        accessPermission: role_read,
        buttonId: 'roleManagement_mi_btn',
        route: '/roles',
        description: 'Add or edit roles and permissions',
        title: 'Manage Roles & Permissions',
        folderName: 'Admin Tools',
      },

      {
        accessPermission: '',
        buttonId: 'viewPolicyGuidance_mi_btn',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-localgovernment',
        orgType: 'LG',
        isItOrgTypeBased: true,
        description: 'View the policy guidance for sending Notices, Takedown Requests and Escalations',
        title: 'Guidance for local governments',
        folderName: 'Support',
      },
      {
        accessPermission: '',
        buttonId: 'userGuide_mi_btn',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/strdataportal-support',
        orgType: 'LG',
        isItOrgTypeBased: true,
        description: 'Additional information for Local Government users',
        title: 'User Guide',
        folderName: 'Support',
      },
      {
        accessPermission: '',
        buttonId: 'viewPolicyGuidanceForPlatforms_mi_btn',
        route: 'https://www2.gov.bc.ca/gov/content/housing-tenancy/short-term-rentals/data-guidelines-platforms',
        orgType: 'Platform',
        isItOrgTypeBased: true,
        description: 'View the guidelines for uploading data and actioning takedown requests',
        title: 'Guideline for Platforms',
        folderName: 'Support',
      },
    ]
  }
}
