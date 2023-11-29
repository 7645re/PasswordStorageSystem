import React, { useState, ChangeEvent, FormEvent, SetStateAction, useEffect } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import useLocalStorage from '../../hooks/useLocalStorage';
import styles from './Login.module.css';

interface FormData {
    userLogin: string;
    password: string;
}

interface ResponseBody {
    isSuccess: boolean;
    errorMessage: string;
    result: {
        token: string;
    };
}

enum FormMode {
    LOGIN = 'LOGIN',
    REGISTER = 'REGISTER',
}

const Login: React.FC = () => {
    const [formData, setFormData] = useState<FormData>({ userLogin: '', password: '' });
    const [requestResult, setRequestResult] = useState<ResponseBody | null>(null);
    const [formMode, setFormMode] = useState<FormMode>(FormMode.LOGIN);
    const [token, setToken] = useLocalStorage("token", null)
    const navigate = useNavigate();

    const handleFormSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        
        try {
            let url = formMode === FormMode.LOGIN ? 'http://localhost:5164/user/login' : 'http://localhost:5164/user/register';

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });

            let body: ResponseBody = await response.json();
            setRequestResult(body);
            if (body.isSuccess) {
                setToken(body.result.token);
            }
        } catch (error) {
            console.error('Произошла ошибка', error);
        }
    };

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const switchFormMode = () => {
        setFormMode(formMode === FormMode.LOGIN ? FormMode.REGISTER : FormMode.LOGIN);
        setFormData({ userLogin: '', password: '' });
        setRequestResult(null);
    };
    
    return (
        token !== null ? (<Navigate to={"/"}/>) : (<div className={styles.page}>
            <form onSubmit={handleFormSubmit} className={styles.authForm}>
                <div className={styles.formLabel}>
                    <span>{formMode === FormMode.LOGIN ? 'Sign in' : 'Sign up'}</span>
                </div>
                <div>
                    <input
                        type="text"
                        id="userLogin"
                        name="userLogin"
                        placeholder="Login"
                        value={formData.userLogin}
                        onChange={handleInputChange}
                        className={styles.input}
                    />
                </div>
                <div>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        placeholder="Password"
                        value={formData.password}
                        onChange={handleInputChange}
                        className={styles.input}
                    />
                </div>
                <div>{requestResult?.errorMessage}</div>
                <button type="submit">{formMode === FormMode.LOGIN ? 'Sign in' : 'Sign up'}</button>
                <div>
                <span onClick={switchFormMode}>
                    {formMode === FormMode.LOGIN ? 'Don\'t have an account? Sign up' : 'Already have an account? Sign in'}
                </span>
                </div>
            </form>
        </div>)
    );
};

export default Login;
