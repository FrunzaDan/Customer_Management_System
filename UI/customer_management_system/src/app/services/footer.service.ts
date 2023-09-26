import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FooterService {

  showFooter: BehaviorSubject<boolean>;

  constructor() { 
    this.showFooter = new BehaviorSubject(true);
  }

  hideFooter(){
    this.showFooter.next(false);
  }

  displayFooter(){
    this.showFooter.next(true);
  }
}
