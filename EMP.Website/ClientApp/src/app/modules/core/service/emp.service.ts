import { IResultDto } from '../../shared/models/dto/result-dto'
import { environment } from './../../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from '../http/base.service';
import { ExportPK } from '../../shared/models/dto/requests/export-pk';
import { map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EmpService extends BaseService  {

  constructor(
    private httpClient: HttpClient
  ) {
    super(); 
   }

   GetDiffDatas(data: ExportPK):Observable<IResultDto<string[]>>
   {
    const url = `${environment.apiBaseUrl}/EqmAFortyCompare/GetDiffDatas`;
    const options:any = this.generatePostOptions();
    //options.responseType = 'arraybuffer';
    //return this.httpClient.post<IResultDto<string[]>>(url, { filePath: data.filePath, startDate:data.startDate, endDate:data.endDate, pNs:data.pNs}, options);
     return this.httpClient.post<IResultDto<string[]>>(url, { filePath: data.filePath, startDate:data.startDate, endDate:data.endDate, pNs:data.pNs}, options)
         .pipe(map((res: any) => this.processResult(res)))
    }

    
  DataCheck(data: ExportPK):Observable<any>{
    const url = `${environment.apiBaseUrl}/EqmAFortyCompare/DataCheck`;
    const options:any = this.generatePostOptions();

    return this.httpClient.post(url, { filePath: data.filePath, startDate:data.startDate, endDate:data.endDate, pNs:data.pNs}, options)
  }

  ExportPK(data: ExportPK):Observable<any>{
    const url = `${environment.apiBaseUrl}/EqmAFortyCompare/ExportPK`;
    const options:any = this.generatePostOptions();
    options.responseType = 'arraybuffer';

    return this.httpClient.post(url, { filePath: data.filePath, startDate:data.startDate, endDate:data.endDate, pNs:data.pNs}, options)
          .pipe(map(data => {
            this.downloadFile(
              'PK_RESULT.xlsx',
              data,
              'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
            ) 
          })
          )
  }

  private downloadFile(name: string, data: any, type: string)
  {     
    const blob = new Blob([data], { type: type });
    const url = window.URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    link.remove();
  }
}
