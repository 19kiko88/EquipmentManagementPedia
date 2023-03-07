import { environment } from './../../../../../environments/environment';
import { EmpService } from './../../../core/service/emp.service';
import { UploadComponent } from './../../../shared/components/upload/upload.component';
import { NgbDatepickerComponent } from './../../../shared/ng-bootstrap/ngb-datepicker/ngb-datepicker.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IResultDto } from '../../../shared/models/dto/result-dto';
import { ExportPK } from 'src/app/modules/shared/models/dto/requests/export-pk';
import { concatMap, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit{
  
  @ViewChild("txtStartDate") sd!: NgbDatepickerComponent;
  @ViewChild("txtEndDate") ed!: NgbDatepickerComponent;
  @ViewChild("fileUpload") upload: UploadComponent | undefined;
  uploadApiURL: string = "";
  filePath: string = "";
  pns: string = "";



  constructor(
    private empService: EmpService
  ){

  }

  ngOnInit() 
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/EqmAFortyCompare/Upload`;
  } 

  //form submit
  onSubmit()
  {
    //let pns = this.pns;// && this.pns?.indexOf(';') > 0 ? this.pns.split(';') : [this.pns];
    let sd = `${this.sd.model?.year.toString()}-${this.sd.model?.month.toString().padStart(2, "0")}-${this.sd.model?.day.toString().padStart(2, "0")}`
    let ed = `${this.ed.model?.year.toString()}-${this.ed.model?.month.toString().padStart(2, "0")}-${this.ed.model?.day.toString().padStart(2, "0")}`

    let data: ExportPK = {
      filePath : this.filePath,
      startDate: new Date(sd),
      endDate: new Date(ed),
      pNs:this.pns
    }

    this.empService.DataCheck(data).pipe(
      concatMap(res => {
        if (!res.content)
        {
          //debugger;
          return this.empService.ExportPK(data)
        }
        else
        {
          //debugger;
          window.alert(res.content);
          return of(res);//return observable
        }
      })
      )
    .subscribe((c) => 
      {
        //debugger;
        //let qq = c;
      }
      )
  }

  /*upload元件，output method
   * 接收上傳資訊(server存放路徑)
   */
  FileUpload(data: IResultDto<string>)
  {
    this.filePath = data.content;
  } 
}
