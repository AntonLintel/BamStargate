import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { provideTanStackQuery, QueryClient} from '@tanstack/angular-query-experimental';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar'
import { MatToolbar } from '@angular/material/toolbar'
import { MatIcon } from '@angular/material/icon'
import { MatButton } from '@angular/material/button'
import { MatCardModule } from '@angular/material/card'

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule, MatFormFieldModule,
    MatSelectModule, FormsModule,
    ReactiveFormsModule, MatInputModule,
    BrowserAnimationsModule, MatProgressBarModule,
    MatToolbar, MatIcon,
    MatButton, MatCardModule
  ],
  providers: [provideTanStackQuery(new QueryClient())],
  bootstrap: [AppComponent]
})
export class AppModule { }
