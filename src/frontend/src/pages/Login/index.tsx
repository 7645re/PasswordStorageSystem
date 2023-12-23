import axios, { HttpStatusCode } from "axios";
import React, { useState, ChangeEvent, FormEvent, useEffect } from "react";
import { Navigate } from "react-router-dom";
import { ENDPOINTS } from "../../endpoints";
import useLocalStorage from "../../hooks/useLocalStorage";
import styles from "./index.module.css";

interface FormData {
  login: string;
  password: string;
}

interface ResponseBody {
  token: string;
  expire: Date;
}

enum FormMode {
  LOGIN = "LOGIN",
  REGISTER = "REGISTER",
}

const Login: React.FC = () => {
  const [formData, setFormData] = useState<FormData>({
    login: "",
    password: "",
  });
  const [logInError, setLogInError] = useState<string | null>(null);
  const [formMode, setFormMode] = useState<FormMode>(FormMode.LOGIN);
  const [token, setToken] = useLocalStorage("token", null);

  const handleFormSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    try {
      let url =
        formMode === FormMode.LOGIN ? ENDPOINTS.LOG_IN : ENDPOINTS.REGISTER;
      let response = await axios.post<ResponseBody>(url, formData);
      console.log(response);
      if (response.status === HttpStatusCode.Ok) {
        setToken(response.data.token);
      } else {
        // setLogInError(response.data.errorMessage);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const switchFormMode = () => {
    setFormMode(
      formMode === FormMode.LOGIN ? FormMode.REGISTER : FormMode.LOGIN
    );
    setFormData({ login: "", password: "" });
    setLogInError(null);
  };

  return token !== null ? (
    <Navigate to={"/"} />
  ) : (
    <div className={styles.page}>
      <form onSubmit={handleFormSubmit} className={styles.authForm}>
        <div className={styles.formLabel}>
          <span>{formMode === FormMode.LOGIN ? "Sign in" : "Sign up"}</span>
        </div>
        <div>
          <input
            type="text"
            id="login"
            name="login"
            placeholder="Login"
            value={formData.login}
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
        {logInError === null ? "" : <div>{logInError}</div>}
        <button type="submit">
          {formMode === FormMode.LOGIN ? "Sign in" : "Sign up"}
        </button>
        <div>
          <span onClick={switchFormMode}>
            {formMode === FormMode.LOGIN
              ? "Don't have an account? Sign up"
              : "Already have an account? Sign in"}
          </span>
        </div>
      </form>
    </div>
  );
};

export default Login;
