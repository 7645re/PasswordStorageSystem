import { useState } from "react";
import styles from "./Credential.module.css";
import { Credential, CredentialUpdate, PasswordSecurityLevel } from "../CredentialTypes";
import { useOutsideClick } from "../../../hooks/useOutsideClick";
import instance from "../../../app/requestInterceptor";
import { OperationResult } from "../../OperationResult";
import { ENDPOINTS } from "../../../endpoints";

interface Props {
  credential: Credential;
}

export function CredentialComponent(props: Props) {
  const [isEditing, setEditingState] = useState<boolean>(false);
  const [changedPassword, setChangedPassword] = useState<string>(
    props.credential.resourcePassword
  );

  const ref = useOutsideClick(() => {
    if (isEditing) {
      setEditingState((prevState) => !prevState);
    }
  });

  async function patchCredentialPassword() {
    try {
      var response = await instance.get<OperationResult<CredentialUpdate>>(
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

  const handleChangeCredentialPassword = () => {

  }

  return (
    <div ref={ref} className={styles.credential}>
      <div className={styles.credentialInfoItem}>
        {props.credential.resourceName}
      </div>
      <div className={styles.credentialInfoItem}>
        {props.credential.resourceLogin}
      </div>
      <input
        className={`${styles.credentialInfoItem} ${
          !isEditing && styles.passwordInputReadOnly
        }`}
        type={isEditing ? "text" : "password"}
        value={changedPassword}
        readOnly={!isEditing}
        onChange={(e) => setChangedPassword(e.target.value)}
      />
      <div className={styles.credentialInfoItem}>
        {props.credential.createAt}
      </div>
      <div className={styles.credentialInfoItem}>
        {props.credential.changeAt}
      </div>
      <div className={styles.credentialInfoItem}>
        {PasswordSecurityLevel[props.credential.passwordSecurityLevel]}
      </div>
      <button
        className={styles.credentialButton}
        onClick={() => setEditingState((prevstate) => !prevstate)}
      >
        <svg
          width={20}
          height={20}
          fill="#000000"
          version="1.1"
          id="Capa_1"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 528.899 528.899"
        >
          <g id="SVGRepo_tracerCarrier"></g>
          <g id="SVGRepo_iconCarrier">
            {" "}
            <g>
              {" "}
              <path d="M328.883,89.125l107.59,107.589l-272.34,272.34L56.604,361.465L328.883,89.125z M518.113,63.177l-47.981-47.981 c-18.543-18.543-48.653-18.543-67.259,0l-45.961,45.961l107.59,107.59l53.611-53.611 C532.495,100.753,532.495,77.559,518.113,63.177z M0.3,512.69c-1.958,8.812,5.998,16.708,14.811,14.565l119.891-29.069 L27.473,390.597L0.3,512.69z"></path>{" "}
            </g>{" "}
          </g>
        </svg>
      </button>
      <button
        className={styles.credentialButton}
        onClick={()}
        style={{
          display:
            props.credential.resourcePassword !== changedPassword
              ? "block"
              : "none",
        }}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          version="1.1"
          width={20}
          height={20}
          fill="#000000"
          viewBox="0 0 256 256"
        >
          <defs></defs>
          <g transform="translate(1.4065934065934016 1.4065934065934016) scale(2.81 2.81)">
            <path
              d="M 29.452 78.819 L 1.601 50.968 c -2.134 -2.134 -2.134 -5.595 0 -7.729 l 8.691 -8.691 c 2.134 -2.134 5.595 -2.134 7.729 0 l 13.058 13.058 c 1.236 1.236 3.239 1.236 4.475 0 l 36.425 -36.425 c 2.134 -2.134 5.595 -2.134 7.729 0 l 8.691 8.691 c 2.134 2.134 2.134 5.595 0 7.729 L 37.181 78.819 C 35.046 80.953 31.586 80.953 29.452 78.819 z"
              transform=" matrix(1 0 0 1 0 0) "
            />
          </g>
        </svg>
      </button>
    </div>
  );
}
