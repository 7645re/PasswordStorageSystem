import { UUID } from "crypto";

export enum PasswordSecurityLevel {
  Secure,
  Insecure,
  Compromised,
}

export interface Credential {
  resourceName: string;
  resourceLogin: string;
  resourcePassword: string;
  createAt: string;
  changeAt: string;
  passwordSecurityLevel: PasswordSecurityLevel;
  id: UUID;
}

export interface PasswordSecurityLevels {
  Secure: number;
  Insecure: number;
  Compromised: number;
}

export interface CredentialUpdate {
  resourcePassword: string;
  changedAt: string;
  passwordSecurityLevel: PasswordSecurityLevel;
}
