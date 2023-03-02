import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbDatepickerModule, NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import { NgbDatepickerComponent } from './ngb-datepicker/ngb-datepicker.component';


@NgModule({
  declarations: [
    NgbDatepickerComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgbAlertModule,    
    NgbDatepickerModule
  ],
  exports:[
    NgbDatepickerComponent
  ]
})
export class NgBootstrapModule { }
