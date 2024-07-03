import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayoutComponent } from './layout.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { KeycloakService } from 'keycloak-angular';
import { MessageService } from 'primeng/api';
import { TopMenuService } from '../services/top-menu.service';

xdescribe('LayoutComponent', () => {
  let component: LayoutComponent;
  let fixture: ComponentFixture<LayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        LayoutComponent,
        HttpClientTestingModule,
      ],
      providers: [
        MessageService,
        {
          provide: KeycloakService,
          useValue: {

          }
        },
        {
          provide: TopMenuService,
          useValue: {
            getMenuItemsPerUserType: (value: any) => { },
            'initMenu': () => { }
          }
        },
      ],
    })
      .compileComponents();

    fixture = TestBed.createComponent(LayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
