import axios, { AxiosResponse, AxiosError } from 'axios';
import { ENDPOINTS } from '../endpoints';

const instance = axios.create();

instance.interceptors.response.use(
    (response: AxiosResponse) => {
        return response;
    },
    async function (error: AxiosError) {
        if (error.response && error.response.headers['www-authenticate']) {
            const authHeader = error.response.headers['www-authenticate'];
            
            if (authHeader.includes('Bearer error="invalid_token"')) {
                localStorage.removeItem('token');
                window.location.href = "/login"
            }
        }

        return Promise.reject(error);
    }
);

export default instance;
