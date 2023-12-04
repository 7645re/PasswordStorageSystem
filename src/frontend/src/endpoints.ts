const BASE_URL = 'http://localhost:5164';

export const ENDPOINTS = {
    LOG_IN: `${BASE_URL}/user/login`,
    REGISTER: `${BASE_URL}/user/register`,
    CREDENTIALS_COUNT: `${BASE_URL}/credentials/count`,
    PASSWORDS_SECURITY_LEVELS: `${BASE_URL}/credentials/passwordsSecurityLevels`,
    CREDENTIAL_CREATE: `${BASE_URL}/credentials/create`,
    CREDENTIALS: `${BASE_URL}/credentials`,
};
