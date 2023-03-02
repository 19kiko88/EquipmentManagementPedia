import { NgBootstrapModule } from './modules/shared/ng-bootstrap/ng-bootstrap.module';
import { CoreModule } from './modules/core/core.module';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeModule } from './modules/home/home.module';

@NgModule({
    declarations: [
        AppComponent
    ],
    providers: [],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        FormsModule,        
        CoreModule,
        HomeModule      
    ]
})
export class AppModule { }
