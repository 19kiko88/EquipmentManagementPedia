import { NgMaterialModule } from './ng-material/ng-material.module';
import { NgBootstrapModule } from './ng-bootstrap/ng-bootstrap.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UploadComponent } from './components/upload/upload.component';



@NgModule({
  declarations: [
    UploadComponent
  ],
  imports: [
    CommonModule,
    NgBootstrapModule,
    NgMaterialModule
  ],
  exports:[UploadComponent]
})
export class SharedModule { }
