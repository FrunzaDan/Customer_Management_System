import { GenericResponse } from './generic-response';

export interface Customer {
  guid: string;
  firstName: string;
  lastName: string;
  msisdn: string;
  email: string;
  gender: number;
  customerStatus: number;
  creationDate: string;
  interactionDate: string;
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

export interface CustomerResponse extends GenericResponse<CustomerResponse> {}

let genderMap = new Map<Customer['gender'], string>();
genderMap.set(0, 'not declared');
genderMap.set(1, 'male');
genderMap.set(2, 'female');

export enum CustomerActivationStatus {
  Active = 1901,
  Deactivated = 1903,
}
