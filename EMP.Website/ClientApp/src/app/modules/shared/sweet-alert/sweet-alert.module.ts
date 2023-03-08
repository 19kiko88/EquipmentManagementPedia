import { SweetAlertService } from 'src/app/modules/shared/sweet-alert/sweet-alert.service';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  providers:[SweetAlertService],//SweetAlertService不用Lazy loading，可以用同一個instance
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class SweetAlertModule { }
