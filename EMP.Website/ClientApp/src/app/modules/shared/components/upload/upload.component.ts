import { IResultDto } from '../../../shared/models/dto/result-dto';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})

export class UploadComponent 
{
  @Input() public inputApiUrl: string = "";
  @Input() public inputUploadType: string = "";
  @Output() private outputUploadInfo: EventEmitter<IResultDto<string>>  = new EventEmitter();//檔案server路徑
  @Output() private startUpload: EventEmitter<boolean>  = new EventEmitter();
  @ViewChild('myFileInput') myFileInput: any;
  
  isLoading: boolean = true;
  selectedFiles?: FileList;
  currentFile?: File;
  progress = 0;
  message = '';
  fileInfos?: Observable<any>;
  fullFilePath: string = '';
  fileName: string = '';
  uploadInfoMsg: string = '';


  constructor(
    //private _bsEighteenService: BsEighteenService,
    //private swl: SweetAlertService
    private httpClient: HttpClient
  ) { }

  selectFile(event: any): void
  {
    this.fileName = '';
    this.message = '';
    this.progress = 0;
    this.selectedFiles = undefined;

    if (event.target.files.length > 0)
    {
      if (event.target.files[0].type != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      if (event.target.files[0].size > 100000000)
      {
        this.selectedFiles = undefined;
        this.myFileInput.nativeElement.value = '';
        return
      }

      this.selectedFiles = event.target.files;      
    }
  }

  upload()
  {
    this.startUpload?.emit(true);
    console.log(`upload ${this.inputUploadType} start.`);
    this.progress = 0;
    this.uploadInfoMsg = '';

    if (this.selectedFiles) 
    {
      const file: File | null = this.selectedFiles.item(0);

      if (file)
      {
        this.currentFile = file;

        const formData: FormData = new FormData();
        formData.append('postFile', file);
        
        this.httpClient.post(this.inputApiUrl, formData, { reportProgress: true, observe: 'events', withCredentials: true })
        .subscribe({
          next: async (event:any) => {
            if (event.type === HttpEventType.UploadProgress)
            {
              this.progress = Math.round(100 * event.loaded / event.total);
            }
            else if (event.type === HttpEventType.Response)
            {            
              if (event.body)
              {
                //res = event.body;
                console.log(`upload ${this.inputUploadType} end.`);
                this.selectedFiles = undefined;
                this.myFileInput.nativeElement.value = '';

                await this.outputUploadInfo?.emit(event.body);
              }
            }
          },
          complete: () => { 
            this.uploadInfoMsg = `${file.name}上傳成功, 請點選匯出產出比對報表`;
            this.startUpload?.emit(false);
          },
          error: () => { 
            this.startUpload?.emit(false);
          },
        })
      }

      this.selectedFiles = undefined;
    }
  } 
}
