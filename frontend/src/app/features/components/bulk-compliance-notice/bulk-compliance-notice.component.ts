import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { environment } from '../../../../environments/environment';
import { TableModule } from 'primeng/table';
import { PanelModule } from 'primeng/panel';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ListingDetails, ListingDetailsWithHostCheckboxExtension } from '../../../common/models/listing-details';
import { DialogModule } from 'primeng/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectedListingsStateService } from '../../../common/services/selected-listings-state.service';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { validateEmailListString } from '../../../common/consts/validators.const';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { ComplianceNoticeBulk } from '../../../common/models/compliance-notice';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

@Component({
  selector: 'app-bulk-compliance-notice',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    TableModule,
    PanelModule,
    TooltipModule,
    CheckboxModule,
    InputTextareaModule,
    DialogModule,
    ChipsModule,
    ReactiveFormsModule,
    FormsModule,
    OverlayPanelModule,
  ],
  templateUrl: './bulk-compliance-notice.component.html',
  styleUrl: './bulk-compliance-notice.component.scss'
})
export class BulkComplianceNoticeComponent implements OnInit {
  listings!: Array<ListingDetails>;
  returnUrl!: string;
  myForm!: FormGroup;
  extendedListings = new Array<ListingDetailsWithHostCheckboxExtension>();

  previewText = '';
  showPreviewDialog = false;

  submissionArray!: Array<ComplianceNoticeBulk>;

  selectedListings!: Array<ListingDetailsWithHostCheckboxExtension>;
  addressWarningScoreLimit = Number.parseInt(environment.ADDRESS_SCORE);
  sort!: { prop: string, dir: 'asc' | 'desc' }

  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }
  public get lgContactEmailControl(): AbstractControl {
    return this.myForm.controls['lgContactEmail'];
  }

  constructor(private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private delistingService: DelistingService,
    private router: Router,
    private route: ActivatedRoute,
    private searchStateService: SelectedListingsStateService,
    private loaderService: GlobalLoaderService,
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      (param) => {
        if (!this.searchStateService?.selectedListings || !param['returnUrl']) {
          this.router.navigateByUrl('/listings');
        }
        else {
          this.returnUrl = param['returnUrl'];
          this.listings = [...this.searchStateService.selectedListings];
          this.extendedListings = this.listings.map((listing) => ({ ...listing, sendNoticeToHosts: true }));
          this.searchStateService.selectedListings = new Array<ListingDetailsWithHostCheckboxExtension>();
          this.selectedListings = this.extendedListings;

          this.initForm();
          this.cloakParams();
        }
      });
  }

  onSort(property: keyof ListingDetailsWithHostCheckboxExtension): void {
    if (this.sort) {
      if (this.sort.prop === property) {
        this.sort.dir = this.sort.dir === 'asc' ? 'desc' : 'asc';
      }
      else {
        this.sort.prop = property;
        this.sort.dir = 'asc';
      }
    }
    else {
      this.sort = { prop: property, dir: 'asc' };
    }

    this.extendedListings = (this.extendedListings as Array<ListingDetailsWithHostCheckboxExtension>).sort((a, b) => {
      const propA = (a as ListingDetailsWithHostCheckboxExtension)[property];
      const propB = (b as ListingDetailsWithHostCheckboxExtension)[property];

      if (this.sort.dir === 'asc') {
        return propA < propB ? -1 : propA > propB ? 1 : 0;
      } else {
        return propA < propB ? 1 : propA > propB ? -1 : 0;
      }
    });
  }

  submit(): void {
    this.sendPreview();
  }

  cancel(): void {
    this.searchStateService.selectedListings = [];
    this.router.navigateByUrl(this.returnUrl);
  }

  submitAfterPreview(): void {
    this.loaderService.loadingStart();
    this.delistingService.complianceNoticeBulk(this.submissionArray)
      .subscribe({
        next: () => {
          this.messageHandlerService.showSuccess('Notice of non-compliance has been sent successfully');
          this.cancel();
        },
        complete: () => {
          this.loaderService.loadingEnd();
        }
      });
  }

  cancelPreview(): void {
    this.showPreviewDialog = false;
  }

  private sendPreview(): void {
    const formValues = this.myForm.value;
    this.submissionArray = this.selectedListings.map((x: ListingDetailsWithHostCheckboxExtension) => ({
      rentalListingId: x.rentalListingId,
      ccList: formValues.ccList.prototype === Array
        ? formValues
        : (formValues.ccList as string).split(',').filter(x => !!x).map(x => x.trim()),
      hostEmailSent: x.sendNoticeToHosts,
      comment: formValues.comment,
      lgContactEmail: formValues.lgContactEmail,
    }));

    this.loaderService.loadingStart();

    this.delistingService.complianceNoticeBulkPreview(this.submissionArray)
      .subscribe({
        next: (preview: { content: string }) => {
          this.previewText = preview.content;
          this.showPreviewDialog = true;
        },
        complete: () => {
          this.loaderService.loadingEnd();
        }
      });
  }

  private cloakParams(): void {
    var newURL = location.href.split("?")[0];
    window.history.pushState('object', document.title, newURL);
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      ccList: ['', validateEmailListString()],
      lgContactEmail: ['', [Validators.required, Validators.email]],
      comment: [''],
    });
  }
}
