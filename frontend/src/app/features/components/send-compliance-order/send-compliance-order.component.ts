import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormsModule, FormGroup, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipsModule } from 'primeng/chips';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { ListingDetails, ListingDetailsWithHostCheckboxExtension } from '../../../common/models/listing-details';
import { ListingTableRow } from '../../../common/models/listing-table-row';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectedListingsStateService } from '../../../common/services/selected-listings-state.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';
import { validateEmailListString } from '../../../common/consts/validators.const';
import { TooltipOptions } from 'primeng/api';
import { ComplianceOrder } from '../../../common/models/compliance-order';
import { ComplianceService } from '../../../common/services/compliance.service';

@Component({
  selector: 'app-send-compliance-order',
  standalone: true,
  imports: [
    CommonModule,
    TooltipModule,
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
  templateUrl: './send-compliance-order.component.html',
  styleUrl: './send-compliance-order.component.scss'
})
export class SendComplianceOrderComponent implements OnInit {
  listings!: Array<ListingDetails | ListingTableRow>;
  returnUrl!: string;
  myForm!: FormGroup;

  extendedListings = new Array<ListingDetailsWithHostCheckboxExtension | any>();

  previewText = '';
  showPreviewDialog = false;

  submissionArray!: Array<any>;
  selectedListings!: Array<ListingDetailsWithHostCheckboxExtension>;
  sort!: { prop: string, dir: 'asc' | 'desc' }

  tooltipOptions: TooltipOptions = {
    tooltipStyleClass: 'wide',
  }

  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }
  public get commentControl(): AbstractControl {
    return this.myForm.controls['comment'];
  }

  constructor(private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private complianceService: ComplianceService,
    private router: Router,
    private route: ActivatedRoute,
    private searchStateService: SelectedListingsStateService,
    private loaderService: GlobalLoaderService,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      (param) => {
        if (!this.searchStateService?.selectedListings || !param['returnUrl']) {
          this.router.navigateByUrl('/');
        }
        else {
          this.returnUrl = param['returnUrl'];
          this.listings = [...this.searchStateService.selectedListings];
          this.extendedListings = this.listings.map((listing) => ({ ...listing, sendNoticeToHosts: (listing as any).hasAtLeastOneValidHostEmail }));
          this.searchStateService.selectedListings = new Array<ListingDetailsWithHostCheckboxExtension>();
          this.selectedListings = this.extendedListings.filter(x => x.hasAtLeastOneValidHostEmail);

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

  submitPreview(): void {
    const form: { ccList: string, comment: string } = this.myForm.getRawValue();
    this.submissionArray = this.selectedListings.map<ComplianceOrder>((item) => ({
      bccList: form.ccList.split(',').filter(x => !!x).map(x => x.trim()),
      comment: form.comment,
      rentalListingId: item.rentalListingId,
    }));

    this.loaderService.loadingStart();
    this.complianceService.sendComplianceOrdersPreview(this.submissionArray).subscribe({
      next: (preview) => {
        this.previewText = preview.content;
        this.showPreviewDialog = true;
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      },
    });
  }

  submitAfterPreview(): void {
    this.loaderService.loadingStart();
    this.complianceService.sendComplianceOrdersConfirm(this.submissionArray).subscribe({
      next: () => {
        this.messageHandlerService.showSuccess('Your Message to Hosts Were Successfully Sent.');
        this.cancel();
      },
      complete: () => {
        this.loaderService.loadingEnd();
        this.cd.detectChanges();
      },
    });
  }

  cancel(): void {
    this.searchStateService.selectedListings = [];
    this.router.navigateByUrl(this.returnUrl);
  }

  cancelPreview(): void {
    this.showPreviewDialog = false;
  }

  private cloakParams(): void {
    var newURL = location.href.split("?")[0];
    window.history.pushState('object', document.title, newURL);
  }

  private initForm(): void {
    this.myForm = this.fb.group({
      ccList: ['', [Validators.required, validateEmailListString()]],
      comment: ['', [Validators.required]],
    });
  }
}
