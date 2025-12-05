import { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { CircularProgress, Box } from '@mui/material'
import { useAuth } from '../../contexts/AuthContext'

/**
 * ProtectedRoute Props
 * Following Interface Segregation Principle - minimal interface
 */
interface ProtectedRouteProps {
  children: ReactNode
}

/**
 * Protected Route Component
 * Following Single Responsibility Principle - handles only route protection
 * Following Open/Closed Principle - can be extended for role-based access
 * 
 * This component ensures that only authenticated users can access protected routes.
 * If user is not authenticated, redirects to login page.
 */
const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth()

  // Show loading spinner while checking authentication
  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '100vh',
        }}
      >
        <CircularProgress />
      </Box>
    )
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  // Render protected content
  return <>{children}</>
}

export default ProtectedRoute

