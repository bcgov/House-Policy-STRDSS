import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { UserDataService } from './common/services/user-data.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('AppComponent', () => {
	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [
				AppComponent,
				HttpClientTestingModule,
			],
			providers: [UserDataService]
		}).compileComponents();
	});

	it('should create the app', () => {
		const fixture = TestBed.createComponent(AppComponent);
		const app = fixture.componentInstance;
		expect(app).toBeTruthy();
	});
});
