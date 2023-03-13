import { CommonService } from './../http/common.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {

  userName: string| undefined;

  constructor(
    private commonService: CommonService
  ){

  }
  
  async ngOnInit() 
  {
    this.userName = await this.commonService.GetUserName().toPromise();
  }

}
