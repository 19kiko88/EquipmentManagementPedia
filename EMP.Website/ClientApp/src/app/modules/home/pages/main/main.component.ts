import { SweetAlertService } from 'src/app/modules/shared/sweet-alert/sweet-alert.service';
import { environment } from './../../../../../environments/environment';
import { EmpService } from './../../../core/http/emp.service';
import { UploadComponent } from './../../../shared/components/upload/upload.component';
import { NgbDatepickerComponent } from './../../../shared/ng-bootstrap/ngb-datepicker/ngb-datepicker.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IResultDto } from '../../../shared/models/dto/result-dto';
import { ExportPK } from 'src/app/modules/shared/models/dto/requests/export-pk';
import { concatMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { NgxSpinnerService } from 'ngx-spinner';


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
  loadingMsg: string = 'Loading...';
  uploadedInfoMsg: string = '';

      /**
     * ngx-spinner ref：
     * https://www.npmjs.com/package/ngx-spinner?activeTab=readme
     * https://hackernoon.com/adding-the-loading-component-ngx-spinner-to-an-angular-application
     */

  constructor(
    private empService: EmpService,
    private spinnerService: NgxSpinnerService,
    private saService: SweetAlertService
  ){    

  }

  ngOnInit() 
  {
    this.uploadApiURL = `${environment.apiBaseUrl}/EqmAFortyCompare/Upload`;
  } 

  //form submit
  onSubmit()
  {
    this.loadingMsg = '資料匯出中...';
    this.spinnerService.show();

    let sd = `${this.sd.model?.year.toString()}-${this.sd.model?.month.toString().padStart(2, "0")}-${this.sd.model?.day.toString().padStart(2, "0")}`
    let ed = `${this.ed.model?.year.toString()}-${this.ed.model?.month.toString().padStart(2, "0")}-${this.ed.model?.day.toString().padStart(2, "0")}`

    let data: ExportPK = {
      filePath : this.filePath,
      startDate: new Date(sd),
      endDate: new Date(ed),
      pNs:this.pns
    }

    /**
     * API Chain Ref：
     * https://fullstackladder.dev/blog/2020/10/04/mastering-rxjs-19-switchmap-concatmap-mergemap-exhaustmap/
     * concatMap：https://stackoverflow.com/questions/53482188/rxjs-pipe-chaining-with-if-statement-in-the-middle
     */
    this.empService.DataCheck(data).pipe(
      concatMap(res => {
        if (!res)
        {
          //debugger;
          return this.empService.ExportPK(data)//執行Excel下載
        }
        else
        {
          //debugger;
          return of(res);//return observable
        }
      }
      ))
    .subscribe({
      next: (res) => 
      {
        if (typeof(res) == 'string')
        {//回傳型別為string表示執行return of(res)，回傳檢核錯誤訊息
          this.saService.showSwal("", res, 'error');
          return;
        }

        this.spinnerService.hide();

        this.saService.showSwal('', '比對報表匯出完成.', 'success');
      },
      error: () => {
        this.saService.showSwal('', '系統異常,請聯絡CAE Team...', 'error');
        this.spinnerService.hide();
      },
      complete: () => { 
        this.spinnerService.hide(); 
      }
    })

  }

  /**
   * Output Method
   * 接收上傳元件回傳的上傳資訊(server存放路徑)
   */  
  FileUpload(data: IResultDto<string>)
  {
    this.filePath = data.content;
  } 

  /**
   * Output Method
   * 上傳元件g遮罩控制
   */
  setLoading(event: number)
  {
    if(event == 0)
    {
      this.loadingMsg = '檔案上傳中...'
      this.spinnerService.show();
    }
    else if (event == 1)
    {
      let array: string[] = this.filePath.split('\\');
      let fileName = array[array.length - 1];

      let msg: string = `${fileName}上傳完畢,請點選匯出產出比對報表.`
      this.saService.showSwal('', msg, 'success');
      this.spinnerService.hide();
      this.uploadedInfoMsg = msg;
    }
    else if(event == -1)
    {
      let msg: string = '上傳失敗,請聯絡CAE Team.';
      this.saService.showSwal('', msg, 'error');
      this.spinnerService.hide();
      this.uploadedInfoMsg = msg;
    }
  }  
}
