import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NgbDatepickerComponent } from './ngb-datepicker.component';

describe('NgbDatepickerComponent', () => {
  let component: NgbDatepickerComponent;
  let fixture: ComponentFixture<NgbDatepickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NgbDatepickerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NgbDatepickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
