import { Navigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { ReactNode } from 'react'

/**
 * Admin Route Component
 * Protects routes that require Admin or SuperAdmin role
 * Following Single Responsibility Principle - handles only admin route protection
 */
interface AdminRouteProps {
  children: ReactNode
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
  const { user } = useAuth()

  // Check if user is Admin or SuperAdmin
  if (!user || (user.roleName !== 'Admin' && user.roleName !== 'SuperAdmin')) {
    // Redirect to dashboard if not admin
    return <Navigate to="/dashboard" replace />
  }

  return <>{children}</>
}

export default AdminRoute

