import { Component } from '@angular/core';
import { UserInfoComponent } from '../user-info/user-info.component';

@Component({
  selector: 'app-user-tools',
  standalone: true,
  imports: [UserInfoComponent],
  templateUrl: './user-tools.component.html',
  styleUrl: './user-tools.component.scss'
})
export class UserToolsComponent {

}
