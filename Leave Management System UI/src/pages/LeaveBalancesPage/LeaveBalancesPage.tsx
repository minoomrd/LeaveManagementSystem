import { useEffect, useState } from 'react'
import {
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  CircularProgress,
} from '@mui/material'
import { LeaveBalance } from '../../types/models'
import { leaveBalanceService } from '../../services/ApiService'
import { useAuth } from '../../contexts/AuthContext'

/**
 * Leave Balances Page Component
 * Following Single Responsibility Principle - displays only leave balances
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const LeaveBalancesPage: React.FC = () => {
  const { user } = useAuth()
  const [leaveBalances, setLeaveBalances] = useState<LeaveBalance[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  /**
   * Load leave balances
   * Following Single Responsibility Principle - loads only leave balances
   */
  useEffect(() => {
    if (user?.id) {
      loadLeaveBalances()
    }
  }, [user?.id])

  /**
   * Load all leave balances from API
   * Following Command pattern - encapsulates load operation
   * Shows only logged-in user's balances for non-admin users
   */
  const loadLeaveBalances = async () => {
    try {
      setIsLoading(true)
      if (!user?.id) {
        return
      }

      let balances: LeaveBalance[]

      // If user is Admin or SuperAdmin, show all balances
      // Otherwise, show only the logged-in user's balances
      if (user.roleName === 'Admin' || user.roleName === 'SuperAdmin') {
        balances = await leaveBalanceService.getAllLeaveBalances()
      } else {
        balances = await leaveBalanceService.getLeaveBalancesByUserId(user.id)
      }

      setLeaveBalances(balances)
    } catch (error) {
      console.error('Failed to load leave balances:', error)
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Format date for display
   * Following Single Responsibility Principle - single purpose function
   */
  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  }

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
        Leave Balances
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom sx={{ mb: 3 }}>
        View current leave balances for employees
      </Typography>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Employee</TableCell>
              <TableCell>Leave Type</TableCell>
              <TableCell>Balance</TableCell>
              <TableCell>Unit</TableCell>
              <TableCell>Last Updated</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {leaveBalances.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <Typography color="text.secondary">No leave balances found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              leaveBalances.map((balance) => (
                <TableRow key={balance.id} hover>
                  <TableCell>{balance.userName}</TableCell>
                  <TableCell>{balance.leaveTypeName}</TableCell>
                  <TableCell>
                    <Typography variant="h6" component="span">
                      {balance.balanceAmount}
                    </Typography>
                  </TableCell>
                  <TableCell>{balance.balanceUnit === 1 ? 'Day(s)' : 'Hour(s)'}</TableCell>
                  <TableCell>{formatDate(balance.updatedAt)}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  )
}

export default LeaveBalancesPage

