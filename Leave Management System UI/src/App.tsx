import { Routes, Route, Navigate } from 'react-router-dom'
import { Box } from '@mui/material'
import Layout from './components/Layout/Layout'
import LoginPage from './pages/LoginPage/LoginPage'
import DashboardPage from './pages/DashboardPage/DashboardPage'
import LeaveRequestsPage from './pages/LeaveRequestsPage/LeaveRequestsPage'
import LeaveBalancesPage from './pages/LeaveBalancesPage/LeaveBalancesPage'
import UsersPage from './pages/UsersPage/UsersPage'
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute'
import { AuthProvider } from './contexts/AuthContext'

/**
 * Main App component
 * Following Single Responsibility Principle - handles routing configuration
 * Following Dependency Inversion Principle - depends on abstractions (components)
 */
function App() {
  return (
    <AuthProvider>
      <Box sx={{ minHeight: '100vh', backgroundColor: 'background.default' }}>
        <Routes>
          {/* Public route - Login page */}
          <Route path="/login" element={<LoginPage />} />
          
          {/* Protected routes - require authentication */}
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="leave-requests" element={<LeaveRequestsPage />} />
            <Route path="leave-balances" element={<LeaveBalancesPage />} />
            <Route path="users" element={<UsersPage />} />
          </Route>
          
          {/* Catch all - redirect to dashboard */}
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </Box>
    </AuthProvider>
  )
}

export default App

