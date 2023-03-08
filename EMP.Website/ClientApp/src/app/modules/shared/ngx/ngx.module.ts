import { NgxSpinnerModule } from 'ngx-spinner';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'


@NgModule({
  declarations: [],
  imports: [    
    CommonModule,
    NgxSpinnerModule.forRoot({ type: 'ball-circus' }),
    BrowserAnimationsModule 
  ],
  exports: [NgxSpinnerModule]
})
export class NgxModule { }
