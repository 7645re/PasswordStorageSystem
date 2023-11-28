import { SetStateAction, useState } from "react";
import styles from "./index.module.css";
import LoginForm from "./LoginForm/LoginForm";
import useLocalStorage from "../../hooks/useLocalStorage";

export default function Index() {
    const [userSignIn, setUserSignIn] = useState(false);
    const [token] = useLocalStorage('token', null);

    return (
        <div className={styles.page}>
            {userSignIn || token ? "" : <LoginForm setUserSignIn={setUserSignIn} ></LoginForm>}
        </div>
    );
};