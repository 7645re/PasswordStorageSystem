import { AxiosRequestConfig } from 'axios';

export const createAuthConfig = (token: string): AxiosRequestConfig => {
    return {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    };
};
