import React from 'react';
import {BrowserRouter as Router, Route, Routes} from 'react-router-dom';
import Home from '../pages/Home';
import styles from "./App.module.css";

function App() {
  return (
    <div className={styles.app}>
      <Router>
        <Routes>
            <Route path="/" Component={Home}></Route>
        </Routes>
      </Router>
    </div>
  );
}

export default App;
