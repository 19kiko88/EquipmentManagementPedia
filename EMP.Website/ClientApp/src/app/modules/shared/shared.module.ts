import { NgMaterialModule } from './ng-material/ng-material.module';
import { NgBootstrapModule } from './ng-bootstrap/ng-bootstrap.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    NgBootstrapModule,
    NgMaterialModule
  ],
  exports:[]
})
export class SharedModule { }
