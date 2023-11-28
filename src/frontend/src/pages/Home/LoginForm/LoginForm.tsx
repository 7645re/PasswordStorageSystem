import React, { useState, ChangeEvent, FormEvent, SetStateAction } from 'react';
import styles from './LoginForm.module.css';
import useLocalStorage from '../../../hooks/useLocalStorage';

interface FormData {
    userLogin: string;
    password: string;
}

interface ResponseBody {
    isSuccess: boolean,
    errorMessage: string,
    result: {
        token: string
    }
}

interface Props {
    setUserSignIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const LoginForm: React.FC<Props> = (props: Props) => {
    const [formData, setFormData] = useState<FormData>({ userLogin: '', password: '' });
    const [requestResult, setRequestResult] = useState<ResponseBody | null>(null);
    const [token, setToken] = useLocalStorage('token', null);

    const handleFormSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const response = await fetch('http://localhost:5164/users/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData),
            });
            let body : ResponseBody = await response.json();
            setRequestResult(body);
            if (body.isSuccess) {
                props.setUserSignIn(true);
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

    return (
        <form onSubmit={handleFormSubmit} className={styles.authForm}>
            <div className={styles.formLabel}>
                <span>Sign up</span>
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
            <button type="submit">Войти</button>
        </form>
    );
};

export default LoginForm;
