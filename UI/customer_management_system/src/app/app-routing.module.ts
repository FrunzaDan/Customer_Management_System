import { inject, NgModule } from "@angular/core";
import { CanActivateFn, Route, RouterModule, PreloadAllModules } from "@angular/router";

import { AppComponent } from './app.component';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';
import { FooterComponent } from './components/footer/footer.component';
import { UserLoginComponent } from './components/user-login/user-login.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HomeComponent } from './components/home/home.component';
import { AboutComponent } from './components/about/about.component';
import { FeaturesComponent } from './components/features/features.component';
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { AddCustomerComponent } from "./components/add-customer/add-customer.component";
import { EditCustomerComponent } from "./components/edit-customer/edit-customer.component";
import { CustomerDetailsComponent } from "./components/customer-details/customer-details.component";
import { AuthGuardService } from "./services/auth-guard.service";

const authGuardFn: CanActivateFn = () => {
  const authService = inject(AuthGuardService);
  return authService.canActivate();
}

const ROUTES: Route[] = [
  {
    path: 'login',
    component: UserLoginComponent,
    title: 'Login'
  },
  {
    path: '',
    component: HomeComponent,
    canActivate: [authGuardFn],
    title: 'Customers'
  },
  {
    path: 'customers',
    component: HomeComponent,
    canActivate: [authGuardFn],
    title: 'Customers'
  },
  {
    path: 'addCustomer',
    component: AddCustomerComponent,
    canActivate: [authGuardFn],
    title: 'Add Customer'
  },
  {
    path: 'editCustomer',
    component: EditCustomerComponent,
    canActivate: [authGuardFn],
    title: 'Edit Customer'
  },
  {
    path: 'features',
    component: FeaturesComponent,
    canActivate: [authGuardFn],
    title: 'Features'
  },
  {
    path: 'about',
    component: AboutComponent,
    canActivate: [authGuardFn],
    title: 'About'
  },
  {
    path: 'customerDetails',
    component: CustomerDetailsComponent,
    canActivate: [authGuardFn],
    title: 'Customer Details'
  },
  {
    path: '**',
    component: PageNotFoundComponent
  }
]

@NgModule({
  imports: [
    RouterModule.forRoot(ROUTES, {
      preloadingStrategy: PreloadAllModules,
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule { }