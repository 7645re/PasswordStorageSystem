import { useState } from "react";
import styles from "./Credential.module.css";
import {
  Credential,
  CredentialUpdate,
  PasswordSecurityLevel,
} from "../CredentialTypes";
import { useOutsideClick } from "../../../hooks/useOutsideClick";
import instance from "../../../app/requestInterceptor";
import { OperationResult } from "../../OperationResult";
import { ENDPOINTS } from "../../../endpoints";
import { createAuthConfig } from "../../../utils/authConfigCreator";

import { ReactComponent as EditIcon } from "../../../assets/editIcon.svg";
import { ReactComponent as AcceptIcon } from "../../../assets/acceptIcon.svg";
import { ReactComponent as CancelIcon } from "../../../assets/cancelIcon.svg";

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

  const handleCancelEdit = () => {
    setChangedPassword(props.credential.resourcePassword);
    setEditingState((prevState) => !prevState);
  };

  // async function patchCredentialPassword() {
  //   try {
  //     var response = await instance.get<OperationResult<CredentialUpdate>>(
  //       ENDPOINTS.CREDENTIALS,
  //       createAuthConfig(token)
  //     );
  //     if (response.data.isSuccess) {
  //       setCredentials(response.data.result);
  //     }
  //   } catch (e) {
  //     console.log(e);
  //   }
  // }

  // const handleChangeCredentialPassword = () => {};

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
        <EditIcon width={30} height={30} />
      </button>
      <button
        className={styles.credentialButton}
        style={{
          display:
            props.credential.resourcePassword !== changedPassword
              ? "block"
              : "none",
        }}
      >
        <AcceptIcon width={30} height={30} />
        <CancelIcon onClick={handleCancelEdit} width={30} height={30} />
      </button>
    </div>
  );
}
