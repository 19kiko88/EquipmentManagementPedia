import { SweetAlertModule } from './sweet-alert/sweet-alert.module';
import { NgxModule } from './ngx/ngx.module';
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
    NgxModule,
    NgBootstrapModule,
    NgMaterialModule,
    SweetAlertModule
  ],
  exports:[
    NgxModule,
    NgBootstrapModule,
    NgMaterialModule,
    UploadComponent,
    SweetAlertModule
  ]
})
export class SharedModule { }
