import { inject, NgModule } from "@angular/core";
import { Route, RouterModule, PreloadAllModules } from "@angular/router";

import { AppComponent } from './app.component';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';
import { FooterComponent } from './components/footer/footer.component';
import { UserLoginComponent } from './components/user-login/user-login.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HomeComponent } from './components/home/home.component';
import { CustomerListComponent } from './components/customer-list/customer-list.component';
import { CustomerOverviewComponent } from './components/customer-overview/customer-overview.component';
import { AddCustomerComponent } from "./components/add-customer/add-customer.component";
import { EditCustomerComponent } from "./components/edit-customer/edit-customer.component";
import { CustomerDetailsComponent } from "./components/customer-details/customer-details.component";

const ROUTES: Route[] = [
  { path: '', component: UserLoginComponent },
  { path: 'customers', component: HomeComponent },
  { path: 'addCustomer', component: AddCustomerComponent },
  { path: 'editCustomer', component: EditCustomerComponent },
  { path: 'customerDetails', component: CustomerDetailsComponent },
  { path: '**', component: PageNotFoundComponent }
]

@NgModule({
  imports: [
    RouterModule.forRoot(ROUTES, {
      preloadingStrategy: PreloadAllModules,
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}