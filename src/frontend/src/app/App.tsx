import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from '../pages/Login';
import useLocalStorage from '../hooks/useLocalStorage';
import ProtectedRoute from '../components/ProtectedRoute';
import { Home } from '../pages/Home/Home';

export default function App() {
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