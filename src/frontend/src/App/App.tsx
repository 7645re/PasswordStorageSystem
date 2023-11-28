import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Home from '../pages/Home';
import useLocalStorage from '../hooks/useLocalStorage';
import Login from '../pages/Login/Login';
import ProtectedRoute from '../components/ProtectedRoute';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                    <Route path="/" element={
                        <ProtectedRoute>
                            <Home/>
                        </ProtectedRoute>
                    } />
            </Routes>
        </Router>
    );
}

export default App;
