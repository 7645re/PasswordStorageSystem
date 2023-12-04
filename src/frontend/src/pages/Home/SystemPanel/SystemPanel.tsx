import styles from "./SystemPanel.module.css"
import {ChangeEvent, FormEvent, useEffect, useState} from "react";
import instance from "../../../app/requestInterceptor";
import { ENDPOINTS } from "../../../endpoints";
import { createAuthConfig } from "../../../utils/authConfigCreator";
import useLocalStorage from "../../../hooks/useLocalStorage";


interface CredentialsCountResponseBody {
    isSuccess: boolean;
    errorMessage: string;
    result: number
}

interface PasswordsSecutiryLevelsResponseBody {
    isSuccess: boolean;
    errorMessage: string | null;
    result: PasswordSecurityLevels
}

interface CredentialsResponseBody {
    isSuccess: boolean;
    errorMessage: string | null;
    result: Credential[]
}

interface CredentialResponseBody {
    isSuccess: boolean;
    errorMessage: string | null;
    result: Credential
}

interface PasswordSecurityLevels {
    Secure: number;
    Insecure: number;
    Compromised: number;
}

interface FormData {
    ResourceName: string;
    ResourceLogin: string;
    ResourcePassword: string;
}

interface CredentialHistoryItem {
    ResourceName: string;
    ResourceLogin: string;
    ResourcePassword: string;
    ChangeAt: string;
}

enum PasswordSecurityLevel
{
    Secure,
    Insecure,
    Compromised
}

interface Credential {
    resourceName: string;
    resourceLogin: string;
    resourcePassword: string;
    createAt: string;
    changeAt: string;
    history: CredentialHistoryItem[];
    passwordSecurityLevel: PasswordSecurityLevel
}

type Props = {
    
};

export default function SystemPanel(props: Props) {
    const [token] = useLocalStorage("token", null)
    const [credentials, setCredentials] = useState<Credential[]>([])
    const [repeatAnalyze, setRepeatAnalyze] = useState<boolean>(false);
    const [formData, setFormData] = useState<FormData>({
        ResourceName: "",
        ResourceLogin: "",
        ResourcePassword: ""
    })
    const [credentialsCount, setCredentialsCount] = useState<number>(0);
    const [passwordSecurityLevels, setPasswordSecurityLevels] = useState<PasswordSecurityLevels>({
        Secure: 0,
        Insecure: 0,
        Compromised: 0
    });

    async function fetchCredentialsCount() {
        try {
            // TODO: move the request configuration to a different location so that it doesn't have to be created each time
            var response = await instance.get<CredentialsCountResponseBody>(ENDPOINTS.CREDENTIALS_COUNT, createAuthConfig(token));
            if (response.data.isSuccess) {
                setCredentialsCount(response.data.result)
            }
        } catch (e) {
            console.log(e)
        }
    }

    async function fetchPasswordSecurityLevels() {
        try {
            var response = await instance.get<PasswordsSecutiryLevelsResponseBody>(ENDPOINTS.PASSWORDS_SECURITY_LEVELS,
                createAuthConfig(token));
            if (response.data.isSuccess) {
                setPasswordSecurityLevels(response.data.result)
            }
        } catch (e) {
            console.log(e)
        }
    }

    async function fetchCredentials() {
        try {
            var response = await instance.get<CredentialsResponseBody>(ENDPOINTS.CREDENTIALS,
                createAuthConfig(token));
            if (response.data.isSuccess) {
                setCredentials(response.data.result)
            }
        } catch (e) {
            console.log(e)
        }
    }

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        e.preventDefault();
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleFormSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            let response = await instance.post<CredentialResponseBody>(ENDPOINTS.CREDENTIAL_CREATE,
                formData, createAuthConfig(token))
            if (response.data.isSuccess) {
                setCredentials((prevCred) => [...prevCred, response.data.result]
                    .sort((a, b) => a.resourceName.localeCompare(b.resourceName)))

                setCredentialsCount((prevCred) => prevCred + 1)
                switch (response.data.result.passwordSecurityLevel) {
                    case PasswordSecurityLevel.Secure:
                        setPasswordSecurityLevels((prevLevels) => ({
                            ...prevLevels,
                            Secure: prevLevels.Secure + 1
                        }));
                        break;
                    case PasswordSecurityLevel.Insecure:
                        setPasswordSecurityLevels((prevLevels) => ({
                            ...prevLevels,
                            Insecure: prevLevels.Insecure + 1
                        }));
                        break;
                    case PasswordSecurityLevel.Compromised:
                        setPasswordSecurityLevels((prevLevels) => ({
                            ...prevLevels,
                            Compromised: prevLevels.Compromised + 1
                        }));
                        break;
                    default:
                        break;
                }

            }
            setFormData({
                ResourceName: "",
                ResourceLogin: "",
                ResourcePassword: ""
            })
        } catch (e) {
            console.log(e);
        }
    };

    const handleRepeatAnalysis = async () => {
        try {
            await fetchCredentialsCount();
            await fetchPasswordSecurityLevels();
            await fetchCredentials();
        } catch (e) {
            console.log(e);
        }
    };

    useEffect(() => {
        fetchCredentialsCount();
        fetchPasswordSecurityLevels();
        fetchCredentials();
    }, [])


    return (
        <div className={styles.systemPanel}>
            <div className={styles.panelHeader}>
                <div className={styles.label}>
                    <div className={styles.panelLabel}>Панель управления</div>
                    <div className={styles.safetyStatus}>Ваш статус безопастности:</div>
                </div>
                <button onClick={handleRepeatAnalysis} className={styles.updateSafetyStatus}>Повторить анализ</button>
            </div>
            <div className={styles.analytics}>
                <div className={styles.leftBlock}>
                    <div className={styles.passwordCounterBlock}>
                        <div className={styles.counterLabel}>
                            Кол-во паролей:
                        </div>
                        <div className={styles.passwordCounterValue}>
                            <span>{credentialsCount}</span>
                        </div>
                    </div>
                </div>
                <div className={styles.passwordIndicators}>
                    <div className={styles.passwordIndicator}>
                        <div className={`${styles.indicatorCircle} ${styles.greenIndicator}`}>
                            <span>{passwordSecurityLevels.Secure}</span>
                        </div>
                        <span className={styles.indicatorLabel}>Надежные</span>
                    </div>
                    <div className={styles.passwordIndicator}>
                        <div className={`${styles.indicatorCircle} ${styles.yellowIndicator}`}>
                            <span>{passwordSecurityLevels.Insecure}</span>
                        </div>
                        <span className={styles.indicatorLabel}>Устаревшие или слабые</span>
                    </div>
                    <div className={styles.passwordIndicator}>
                        <div className={`${styles.indicatorCircle} ${styles.redIndicator}`}>
                            <span>{passwordSecurityLevels.Compromised}</span>
                        </div>
                        <span className={styles.indicatorLabel}>Украденные</span>
                    </div>
                </div>
            </div>
            <div className={styles.credentialsExplorer}>
                <form onSubmit={handleFormSubmit} className={styles.addCredential}>
                    <input
                        type="text"
                        id="ResourceName"
                        name="ResourceName"
                        placeholder="Имя ресурса"
                        value={formData.ResourceName}
                        onChange={handleInputChange}
                    />
                    <input
                        type="text"
                        id="ResourceLogin"
                        name="ResourceLogin"
                        placeholder="Логин на ресурсе"
                        value={formData.ResourceLogin}
                        onChange={handleInputChange}
                    />
                    <input
                        type="password"
                        id="ResourcePassword"
                        name="ResourcePassword"
                        placeholder="Пароль на ресурсе"
                        value={formData.ResourcePassword}
                        onChange={handleInputChange}
                    />
                    <button type="submit">Добавить</button>
                </form>
                <div className={styles.credentials}>
                    {
                        credentials.length > 0 ? credentials.map((cred, index) =>
                            <div className={styles.credential} key={index}>
                                <div className={styles.credentialInfoItem}>
                                    {cred.resourceName}
                                </div>
                                <div className={styles.credentialInfoItem}>
                                    {cred.resourceLogin}
                                </div>
                                <div className={styles.credentialInfoItem}>
                                    {cred.resourcePassword}
                                </div>
                                <div className={styles.credentialInfoItem}>
                                    {cred.createAt}
                                </div>
                                <div className={styles.credentialInfoItem}>
                                    {cred.changeAt}
                                </div>
                                <div className={styles.credentialInfoItem}>
                                    {PasswordSecurityLevel[cred.passwordSecurityLevel]}
                                </div>
                                <button className={styles.credentialEditButton}>
                                    <svg width={20} height={20} fill="#000000" version="1.1" id="Capa_1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 528.899 528.899"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g><g id="SVGRepo_iconCarrier"> <g> <path d="M328.883,89.125l107.59,107.589l-272.34,272.34L56.604,361.465L328.883,89.125z M518.113,63.177l-47.981-47.981 c-18.543-18.543-48.653-18.543-67.259,0l-45.961,45.961l107.59,107.59l53.611-53.611 C532.495,100.753,532.495,77.559,518.113,63.177z M0.3,512.69c-1.958,8.812,5.998,16.708,14.811,14.565l119.891-29.069 L27.473,390.597L0.3,512.69z"></path> </g> </g></svg>
                                </button>
                            </div>
                        ) : ""
                    }
                </div>
            </div>
        </div>
    );
};