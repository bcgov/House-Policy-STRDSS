import { Injectable } from '@angular/core';
import { AuthenticatedUser } from '../models/user';
import { CommonCodesApiService } from './api/common-codes-api.service';
import { CommonCode } from '../models/common-code.model';
import { CommonCodes } from '../enums/common-codes.enum';

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  currentUser!: AuthenticatedUser | null;
  roles = new Array<CommonCode>();
  zoneTypes = new Array<CommonCode>();
  strAffiliates = new Array<CommonCode>();
  complianceStatuses = new Array<CommonCode>();

  constructor(public commonCodesApiService: CommonCodesApiService) {
    this.getCommonCodes();
  }

  getCommonCodes(): void {
    this.commonCodesApiService.getCommonCodes().subscribe((data) => {
      this.manageCommonCodes(data);
    })
  }

  manageCommonCodes(array: Array<CommonCode>): void {
    array.forEach(x => {
      switch (x.codeSet) {
        case CommonCodes.Role:
          this.roles.push(x);
          break;
        case CommonCodes.ZoneType:
          this.zoneTypes.push(x);
          break;
        case CommonCodes.ComplianceStatus:
          this.complianceStatuses.push(x);
          break;
        case CommonCodes.StrAffiliate:
          this.strAffiliates.push(x);
          break;

        default:
          break;
      }
    });

  }
}
