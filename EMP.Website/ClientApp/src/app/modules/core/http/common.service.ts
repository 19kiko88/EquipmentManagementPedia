import { HttpClient } from '@angular/common/http';
import { BaseService } from './base.service';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommonService extends BaseService {

  constructor(
    private httpClient: HttpClient
  ) { 
    super();
  }


  GetUserName():Observable<string>
  {
    const url = `${environment.apiBaseUrl}/Common/GetUserName`;
    const options:any = this.generatePostOptions();

    return this.httpClient.get(url, options).pipe(map((res: any) => this.processResult(res)))
  } 
}
