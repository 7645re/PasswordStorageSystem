import styles from "./SystemPanel.module.css";
import { ChangeEvent, FormEvent, useEffect, useState } from "react";
import instance from "../../../app/requestInterceptor";
import { ENDPOINTS } from "../../../endpoints";
import { createAuthConfig } from "../../../utils/authConfigCreator";
import useLocalStorage from "../../../hooks/useLocalStorage";
import {
  Credential,
  PasswordSecurityLevel,
  PasswordSecurityLevels,
} from "../CredentialTypes";
import { CredentialComponent } from "../Credential/CredentialComponent";
import { OperationResult } from "../../OperationResult";

interface FormData {
  ResourceName: string;
  ResourceLogin: string;
  ResourcePassword: string;
}

export default function SystemPanel() {
  const [token] = useLocalStorage("token", null);
  const [credentials, setCredentials] = useState<Credential[]>([]);
  const [repeatAnalyze, setRepeatAnalyze] = useState<boolean>(false);
  const [formData, setFormData] = useState<FormData>({
    ResourceName: "",
    ResourceLogin: "",
    ResourcePassword: "",
  });
  const [credentialsCount, setCredentialsCount] = useState<number>(0);
  const [passwordSecurityLevels, setPasswordSecurityLevels] =
    useState<PasswordSecurityLevels>({
      Secure: 0,
      Insecure: 0,
      Compromised: 0,
    });

  async function fetchCredentialsCount() {
    try {
      // TODO: move the request configuration to a different location so that it doesn't have to be created each time
      var response = await instance.get<OperationResult<number>>(
        ENDPOINTS.CREDENTIALS_COUNT,
        createAuthConfig(token)
      );
      if (response.data.isSuccess) {
        setCredentialsCount(response.data.result);
      }
    } catch (e) {
      console.log(e);
    }
  }

  async function fetchPasswordSecurityLevels() {
    try {
      var response = await instance.get<
        OperationResult<PasswordSecurityLevels>
      >(ENDPOINTS.PASSWORDS_SECURITY_LEVELS, createAuthConfig(token));
      if (response.data.isSuccess) {
        setPasswordSecurityLevels(response.data.result);
      }
    } catch (e) {
      console.log(e);
    }
  }

  async function fetchCredentials() {
    try {
      var response = await instance.get<OperationResult<Credential[]>>(
        ENDPOINTS.CREDENTIALS,
        createAuthConfig(token)
      );
      if (response.data.isSuccess) {
        setCredentials(response.data.result);
      }
    } catch (e) {
      console.log(e);
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
      let response = await instance.post<OperationResult<Credential>>(
        ENDPOINTS.CREDENTIALS,
        formData,
        createAuthConfig(token)
      );
      if (response.data.isSuccess) {
        setCredentials((prevCred) =>
          [...prevCred, response.data.result].sort((a, b) =>
            a.resourceName.localeCompare(b.resourceName)
          )
        );

        setCredentialsCount((prevCred) => prevCred + 1);
        switch (response.data.result.passwordSecurityLevel) {
          case PasswordSecurityLevel.Secure:
            setPasswordSecurityLevels((prevLevels) => ({
              ...prevLevels,
              Secure: prevLevels.Secure + 1,
            }));
            break;
          case PasswordSecurityLevel.Insecure:
            setPasswordSecurityLevels((prevLevels) => ({
              ...prevLevels,
              Insecure: prevLevels.Insecure + 1,
            }));
            break;
          case PasswordSecurityLevel.Compromised:
            setPasswordSecurityLevels((prevLevels) => ({
              ...prevLevels,
              Compromised: prevLevels.Compromised + 1,
            }));
            break;
          default:
            break;
        }
      }
      setFormData({
        ResourceName: "",
        ResourceLogin: "",
        ResourcePassword: "",
      });
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
  }, []);

  return (
    <div className={styles.systemPanel}>
      <div className={styles.panelHeader}>
        <div className={styles.label}>
          <div className={styles.panelLabel}>Панель управления</div>
          <div className={styles.safetyStatus}>Ваш статус безопастности:</div>
        </div>
        <button
          onClick={handleRepeatAnalysis}
          className={styles.updateSafetyStatus}
        >
          Повторить анализ
        </button>
      </div>
      <div className={styles.analytics}>
        <div className={styles.leftBlock}>
          <div className={styles.passwordCounterBlock}>
            <div className={styles.counterLabel}>Кол-во паролей:</div>
            <div className={styles.passwordCounterValue}>
              <span>{credentialsCount}</span>
            </div>
          </div>
        </div>
        <div className={styles.passwordIndicators}>
          <div className={styles.passwordIndicator}>
            <div
              className={`${styles.indicatorCircle} ${styles.greenIndicator}`}
            >
              <span>{passwordSecurityLevels.Secure}</span>
            </div>
            <span className={styles.indicatorLabel}>Надежные</span>
          </div>
          <div className={styles.passwordIndicator}>
            <div
              className={`${styles.indicatorCircle} ${styles.yellowIndicator}`}
            >
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
          {credentials.length > 0
            ? credentials.map((cred, index) => (
                <CredentialComponent key={index} credential={cred} />
              ))
            : ""}
        </div>
      </div>
    </div>
  );
}
