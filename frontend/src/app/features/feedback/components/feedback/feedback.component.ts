import { Component } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-feedback',
  imports: [MATERIAL_IMPORTS, RouterModule],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.scss'
})
export class FeedbackComponent {

}
