import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { UserDataService } from './common/services/user-data.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Subject, of } from 'rxjs';
import { GlobalLoaderService } from './common/services/global-loader.service';
import { ChangeDetectorRef } from '@angular/core';

describe('AppComponent', () => {
	let fixture;
	let app: AppComponent;

	const globalLoaderServiceMock = {
		loadingNotification: new Subject()
	};
	const cd = {
		detectChanges: () => { }
	}

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [
				AppComponent,
				HttpClientTestingModule,
			],
			providers: [
				{
					provide: GlobalLoaderService,
					useValue: globalLoaderServiceMock,
				},
				{
					provide: ChangeDetectorRef,
					useValue: cd,
				},
			]
		}).compileComponents();
		fixture = TestBed.createComponent(AppComponent);
		app = fixture.componentInstance;
	});

	it('should create the app', () => {
		expect(app).toBeTruthy();
	});

	it('should call loader service subscribe', () => {
		spyOn(globalLoaderServiceMock.loadingNotification, 'subscribe');

		app.ngOnInit();
		expect(globalLoaderServiceMock.loadingNotification.subscribe).toHaveBeenCalled()

	})

	xit('should call detectChanges when loader is emitted', () => {
		spyOn(cd, 'detectChanges');
		app.ngOnInit();
		globalLoaderServiceMock.loadingNotification.next('')
		expect(cd.detectChanges).toHaveBeenCalled()
	})
});
