import { Routes, Route, Navigate } from 'react-router-dom'
import { Box } from '@mui/material'
import Layout from './components/Layout/Layout'
import LoginPage from './pages/LoginPage/LoginPage'
import DashboardPage from './pages/DashboardPage/DashboardPage'
import LeaveRequestsPage from './pages/LeaveRequestsPage/LeaveRequestsPage'
import UserLeaveRequestsPage from './pages/UserLeaveRequestsPage/UserLeaveRequestsPage'
import LeaveBalancesPage from './pages/LeaveBalancesPage/LeaveBalancesPage'
import UserLeaveBalancesPage from './pages/UserLeaveBalancesPage/UserLeaveBalancesPage'
import UsersPage from './pages/UsersPage/UsersPage'
import RolesPage from './pages/RolesPage/RolesPage'
import ConfigurationPage from './pages/ConfigurationPage/ConfigurationPage'
import ProtectedRoute from './components/ProtectedRoute/ProtectedRoute'
import AdminRoute from './components/AdminRoute/AdminRoute'
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
            <Route
              path="leave-requests"
              element={
                <AdminRoute>
                  <LeaveRequestsPage />
                </AdminRoute>
              }
            />
            <Route path="user-leave-requests" element={<UserLeaveRequestsPage />} />
            {/* Admin-only route for all leave balances */}
            <Route
              path="leave-balances"
              element={
                <AdminRoute>
                  <LeaveBalancesPage />
                </AdminRoute>
              }
            />
            {/* User-specific route for their own leave balances */}
            <Route path="user-leave-balances" element={<UserLeaveBalancesPage />} />
            <Route
              path="users"
              element={
                <AdminRoute>
                  <UsersPage />
                </AdminRoute>
              }
            />
            <Route
              path="roles"
              element={
                <AdminRoute>
                  <RolesPage />
                </AdminRoute>
              }
            />
            <Route
              path="configuration"
              element={
                <AdminRoute>
                  <ConfigurationPage />
                </AdminRoute>
              }
            />
          </Route>
          
          {/* Catch all - redirect to dashboard */}
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </Box>
    </AuthProvider>
  )
}

export default App

