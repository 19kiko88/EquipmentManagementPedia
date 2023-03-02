import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './pages/main/main.component';
import { NgBootstrapModule } from '../shared/ng-bootstrap/ng-bootstrap.module';



@NgModule({
  declarations: [
    MainComponent
  ],
  imports: [
    CommonModule,
    NgBootstrapModule
  ]
})
export class HomeModule { }
