import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavbarService {
  showNavbar: BehaviorSubject<boolean>;

  constructor() { 
    this.showNavbar = new BehaviorSubject(true);
  }

  hideNavbar(){
    this.showNavbar.next(false);
  }

  displayNavbar(){
    this.showNavbar.next(true);
  }
}
