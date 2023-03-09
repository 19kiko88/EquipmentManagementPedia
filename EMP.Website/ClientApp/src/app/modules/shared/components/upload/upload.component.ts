import { IResultDto } from '../../../shared/models/dto/result-dto';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
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
  @Output() private outputUploadStatus: EventEmitter<number>  = new EventEmitter();//0:開始上傳 1:上傳完成 -1:上傳失敗
  @ViewChild('myFileInput') myFileInput: any;
  
  isLoading: boolean = true;
  selectedFiles?: FileList;
  //currentFile?: File;
  progress = 0;
  message = '';
  fileInfos?: Observable<any>;
  fullFilePath: string = '';
  fileName: string = '';  

  constructor(
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
    this.outputUploadStatus?.emit(0);
    console.log(`upload ${this.inputUploadType} start.`);
    this.progress = 0;

    if (this.selectedFiles) 
    {
      const file: File | null = this.selectedFiles.item(0);

      if (file)
      {
        //this.currentFile = file;

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
              console.log(event); 

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
            this.outputUploadStatus?.emit(1);
          },
          error: () => { 
            this.outputUploadStatus?.emit(-1);
          },
        })
      }

      this.selectedFiles = undefined;
    }
  } 
}
