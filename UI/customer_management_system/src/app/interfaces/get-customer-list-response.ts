export interface GetCustomerListResponse {
    responseCode: string;
    responseMessage: string;
    customerList: Customer[];
}

export interface Customer {
    guid: string;
    firstName: string;
    lastName: string;
    msisdn: string;
    email: string;
    gender: number;
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

let genderMap = new Map<Customer["gender"], string>();
genderMap.set(0, "not declared"); 
genderMap.set(1, "male"); 
genderMap.set(2, "female"); 