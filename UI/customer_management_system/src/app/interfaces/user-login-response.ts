import { GenericResponse } from './generic-response';

export interface UserLoginResponse extends GenericResponse {
  accessToken: string;
  validUntil: string;
}
