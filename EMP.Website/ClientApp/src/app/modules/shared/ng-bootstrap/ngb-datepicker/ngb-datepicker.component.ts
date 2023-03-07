import { Component, Input, OnInit } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-ngb-datepicker',
  templateUrl: './ngb-datepicker.component.html',
  styleUrls: ['./ngb-datepicker.component.scss']
})
export class NgbDatepickerComponent implements OnInit
{
  @Input() public inputAddDays?: number;
	model?: NgbDateStruct
  _calendar: NgbCalendar

  constructor(private calendar: NgbCalendar) 
  { 
    this._calendar = calendar;
  }

  ngOnInit()
  {
    this.model = this._calendar.getToday()

    if (this.inputAddDays && this.inputAddDays > 0)
    {
      let day :Date = new Date(`${this._calendar.getToday().year.toString()}-${this._calendar.getToday().month}-${this._calendar.getToday().day.toString()}`);
      day = new Date(day.setDate(day.getDate() + 30));
      let year = day.getFullYear();
      let month = day.getMonth() + 1; //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/getMonth
      let date = day.getDate();
      this.model = new NgbDate(year, month, date);
    }
  }
}
