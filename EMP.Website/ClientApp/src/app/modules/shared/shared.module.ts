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
    NgMaterialModule
  ],
  exports:[
    NgxModule,
    NgBootstrapModule,
    NgMaterialModule,
    UploadComponent
  ]
})
export class SharedModule { }
