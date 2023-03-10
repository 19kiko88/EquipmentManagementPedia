import { NgxModule } from '../shared/ngx/ngx.module';
import { FormsModule } from '@angular/forms';
import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './pages/main/main.component';


@NgModule({
  declarations: [
    MainComponent
  ],
  imports: [
    FormsModule,
    CommonModule,
    SharedModule
  ]
})
export class HomeModule { }
