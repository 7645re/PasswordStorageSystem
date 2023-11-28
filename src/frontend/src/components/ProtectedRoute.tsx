import { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import useLocalStorage from '../hooks/useLocalStorage';

interface ProtectedRouteProps {
    children: ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
    const [token] = useLocalStorage("token", null);
    
    if (!token) {
        return <Navigate to="/login" replace={true}/>;
    }

    return <>{children}</>;
};

export default ProtectedRoute;
