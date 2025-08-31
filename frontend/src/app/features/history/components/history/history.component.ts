import { Component } from '@angular/core';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { CommonModule } from '@angular/common';

interface Interview {
  id: string;
  date: string;
  duration: string;
  score: number;
}

@Component({
  selector: 'app-history',
  imports: [...MATERIAL_IMPORTS, CommonModule],
  templateUrl: './history.component.html',
  styleUrl: './history.component.scss'
})
export class HistoryComponent {

  interviews: Interview[] = [
    { id: '1', date: '14/08/25', duration: '15m', score: 8.2 },
    { id: '2', date: '10/08/25', duration: '12m', score: 7.5 },
    { id: '3', date: '05/08/25', duration: '20m', score: 9.0 },
  ];  

}
