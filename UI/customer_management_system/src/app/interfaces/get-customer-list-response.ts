import { GenericResponse } from './generic-response';

export interface GetCustomerListResponse extends GenericResponse {
  customerList: Customer[];
}

export interface Customer {
  guid: string;
  firstName: string;
  lastName: string;
  msisdn: string;
  email: string;
  gender: number;
  customerStatus: number;
  birthdate: string;
  address: Address;
}

export interface Address {
  country: string;
  county: string;
  town: string;
  zip: string;
  street: string;
  number: string;
}

let genderMap = new Map<Customer['gender'], string>();
genderMap.set(0, 'not declared');
genderMap.set(1, 'male');
genderMap.set(2, 'female');
