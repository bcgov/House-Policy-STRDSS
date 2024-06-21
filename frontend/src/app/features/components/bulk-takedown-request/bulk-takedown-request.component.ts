import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { environment } from '../../../../environments/environment';
import { TableModule } from 'primeng/table';
import { PanelModule } from 'primeng/panel';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxChangeEvent, CheckboxModule } from 'primeng/checkbox';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ListingDetails } from '../../../common/models/listing-details';
import { DialogModule } from 'primeng/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectedListingsStateService } from '../../../common/services/selected-listings-state.service';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ChipsModule } from 'primeng/chips';
import { DelistingService } from '../../../common/services/delisting.service';
import { validateEmailListString } from '../../../common/consts/validators.const';
import { ErrorHandlingService } from '../../../common/services/error-handling.service';
import { GlobalLoaderService } from '../../../common/services/global-loader.service';

@Component({
  selector: 'app-bulk-takedown-request',
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
  ],
  templateUrl: './bulk-takedown-request.component.html',
  styleUrl: './bulk-takedown-request.component.scss'
})
export class BulkTakedownRequestComponent implements OnInit {
  listings!: Array<ListingDetails>;
  returnUrl!: string;
  myForm!: FormGroup;

  previewText = '';
  showPreviewDialog = false;

  submissionArray!: {
    rentalListingId: number;
    ccList: any;
    isWithStandardDetail: any;
    customDetailTxt: any;
  }[]

  selectedListings!: Array<ListingDetails>;
  addressWarningScoreLimit = Number.parseInt(environment.ADDRESS_SCORE);
  sort!: { prop: string, dir: 'asc' | 'desc' }

  public get ccListControl(): AbstractControl {
    return this.myForm.controls['ccList'];
  }
  public get isWithStandardDetailControl(): AbstractControl {
    return this.myForm.controls['isWithStandardDetail'];
  }
  public get customDetailTxtControl(): AbstractControl {
    return this.myForm.controls['customDetailTxt'];
  }

  constructor(private fb: FormBuilder,
    private messageHandlerService: ErrorHandlingService,
    private delistingService: DelistingService,
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
          this.router.navigateByUrl('/listings');
        }
        else {
          this.returnUrl = param['returnUrl'];
          this.listings = [...this.searchStateService.selectedListings];
          this.searchStateService.selectedListings = new Array<ListingDetails>();
          this.selectedListings = this.listings;
          this.initForm();
          this.cloakParams();
        }
      });
  }

  onSort(property: keyof ListingDetails): void {
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

    this.listings = (this.listings as Array<ListingDetails>).sort((a, b) => {
      const propA = (a as ListingDetails)[property];
      const propB = (b as ListingDetails)[property];

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
    this.router.navigateByUrl(this.returnUrl)
  }

  submitAfterPreview(): void {
    this.loaderService.loadingStart();
    this.delistingService.delistingRequestBulk(this.submissionArray)
      .subscribe({
        next: () => {
          this.messageHandlerService.showSuccess('Takedown request has been sent successfully');
          this.cancel();
        },
        complete: () => {
          this.loaderService.loadingEnd();
          this.cd.detectChanges();
        }
      })
  }

  cancelPreview(): void {
    this.showPreviewDialog = false;
  }

  onWithStandardDetailChanged(value: CheckboxChangeEvent): void {
    if (value.checked)
      this.customDetailTxtControl.removeValidators([Validators.required]);
    else
      this.customDetailTxtControl.addValidators([Validators.required]);

    this.customDetailTxtControl.updateValueAndValidity();
    this.myForm.updateValueAndValidity();
  }

  private sendPreview(): void {
    const formValues = this.myForm.value;
    this.submissionArray = this.selectedListings.map((x: ListingDetails) => ({
      rentalListingId: x.rentalListingId,
      ccList: formValues.ccList.prototype === Array
        ? formValues
        : (formValues.ccList as string).split(',').filter(x => !!x).map(x => x.trim()),
      isWithStandardDetail: formValues.isWithStandardDetail,
      customDetailTxt: formValues.customDetailTxt,
    }));

    this.loaderService.loadingStart();
    this.delistingService.delistingRequestBulkPreview(this.submissionArray)
      .subscribe({
        next: (preview: { content: string }) => {
          this.previewText = preview.content;
          this.showPreviewDialog = true;
        },
        complete: () => {
          this.loaderService.loadingEnd();
          this.cd.detectChanges();
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
      isWithStandardDetail: [true],
      customDetailTxt: [''],
    });
  }
}
