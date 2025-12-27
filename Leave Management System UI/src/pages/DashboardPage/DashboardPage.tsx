import { useEffect, useState } from 'react'
import { Grid, Card, CardContent, Typography, Box, CircularProgress } from '@mui/material'
import {
  Assignment as AssignmentIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  People as PeopleIcon,
} from '@mui/icons-material'
import { LeaveRequest, LeaveRequestStatus } from '../../types/models'
import { leaveRequestService, userService } from '../../services/ApiService'
import { useAuth } from '../../contexts/AuthContext'

/**
 * Stat Card Props
 * Following Interface Segregation Principle - minimal interface
 */
interface StatCardProps {
  title: string
  value: string | number
  icon: React.ReactNode
  color: string
}

/**
 * Stat Card Component
 * Following Single Responsibility Principle - displays only a statistic
 * Following Composition pattern - reusable card component
 */
const StatCard: React.FC<StatCardProps> = ({ title, value, icon, color }) => (
  <Card>
    <CardContent>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <Box>
          <Typography color="text.secondary" gutterBottom variant="body2">
            {title}
          </Typography>
          <Typography variant="h4">{value}</Typography>
        </Box>
        <Box sx={{ color, fontSize: 48 }}>{icon}</Box>
      </Box>
    </CardContent>
  </Card>
)

/**
 * Dashboard Page Component
 * Following Single Responsibility Principle - displays dashboard statistics
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const DashboardPage: React.FC = () => {
  const { user } = useAuth()
  const [stats, setStats] = useState({
    totalUsers: 0,
    pendingRequests: 0,
    approvedRequests: 0,
    rejectedRequests: 0,
  })
  const [isLoading, setIsLoading] = useState<boolean>(true)

  /**
   * Load dashboard statistics
   * Following Single Responsibility Principle - loads only statistics
   * Shows only logged-in user's data for non-admin users
   */
  useEffect(() => {
    const loadStats = async () => {
      if (!user?.id) return

      try {
        setIsLoading(true)
        let users: any[] = []
        let leaveRequests: LeaveRequest[] = []

        // If user is Admin or SuperAdmin, show all data
        // Otherwise, show only the logged-in user's data
        if (user.roleName === 'Admin' || user.roleName === 'SuperAdmin') {
          [users, leaveRequests] = await Promise.all([
            userService.getAllUsers(),
            leaveRequestService.getAllLeaveRequests(),
          ])
        } else {
          // For employees, only show their own data
          leaveRequests = await leaveRequestService.getLeaveRequestsByUserId(user.id)
          users = [] // Employees don't see user count
        }

        // Calculate statistics
        const pendingRequests = leaveRequests.filter(
          (req) => req.status === LeaveRequestStatus.Pending
        ).length
        const approvedRequests = leaveRequests.filter(
          (req) => req.status === LeaveRequestStatus.Approved
        ).length
        const rejectedRequests = leaveRequests.filter(
          (req) => req.status === LeaveRequestStatus.Rejected
        ).length

        setStats({
          totalUsers: users.length,
          pendingRequests,
          approvedRequests,
          rejectedRequests,
        })
      } catch (error) {
        console.error('Failed to load dashboard stats:', error)
      } finally {
        setIsLoading(false)
      }
    }

    loadStats()
  }, [user?.id])

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom sx={{ mb: 3 }}>
        Overview of your leave management system
      </Typography>

      <Grid container spacing={3}>
        {(user?.roleName === 'Admin' || user?.roleName === 'SuperAdmin') && (
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Total Users"
              value={stats.totalUsers}
              icon={<PeopleIcon />}
              color="#1976d2"
            />
          </Grid>
        )}
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Pending Requests"
            value={stats.pendingRequests}
            icon={<AssignmentIcon />}
            color="#ff9800"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Approved Requests"
            value={stats.approvedRequests}
            icon={<CheckCircleIcon />}
            color="#4caf50"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Rejected Requests"
            value={stats.rejectedRequests}
            icon={<CancelIcon />}
            color="#f44336"
          />
        </Grid>
      </Grid>
    </Box>
  )
}

export default DashboardPage

