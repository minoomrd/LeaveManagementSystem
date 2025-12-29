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
  Alert,
} from '@mui/material'
import { LeaveBalance } from '../../types/models'
import { leaveBalanceService } from '../../services/ApiService'
import { useAuth } from '../../contexts/AuthContext'

/**
 * User Leave Balances Page Component
 * Following Single Responsibility Principle - displays only user's own leave balances
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const UserLeaveBalancesPage: React.FC = () => {
  const { user } = useAuth()
  const [leaveBalances, setLeaveBalances] = useState<LeaveBalance[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [error, setError] = useState<string>('')

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
   * Load leave balances for the logged-in user
   * Following Command pattern - encapsulates load operation
   */
  const loadLeaveBalances = async () => {
    try {
      setIsLoading(true)
      if (!user?.id) {
        setError('User not authenticated')
        return
      }

      console.log('Loading leave balances for user:', user.id, 'Role:', user.roleName)
      const balances = await leaveBalanceService.getLeaveBalancesByUserId(user.id)
      console.log('Successfully loaded', balances.length, 'leave balances')

      setLeaveBalances(balances)
      setError('')
    } catch (error: any) {
      console.error('Failed to load leave balances:', error)
      let errorMessage = 'Failed to load leave balances'
      if (error?.response?.data) {
        const errorData = error.response.data
        if (typeof errorData === 'string') {
          errorMessage = errorData
        } else if (errorData?.error) {
          errorMessage = errorData.error
        } else if (errorData?.message) {
          errorMessage = errorData.message
        }
      } else if (error?.message) {
        errorMessage = error.message
      }
      if (error?.status === 500 || error?.response?.status === 500) {
        errorMessage = 'Server error: The API may need to be restarted. Please contact your administrator.'
      }
      setError(errorMessage)
      setLeaveBalances([])
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
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          My Leave Balances
        </Typography>
        <Typography variant="body1" color="text.secondary">
          View your current leave balance for each leave type
        </Typography>
      </Box>

      {error && (
        <Alert
          severity="error"
          sx={{ mb: 2, wordBreak: 'break-word' }}
          onClose={() => setError('')}
        >
          <Typography variant="body2" component="div">
            <strong>Error loading leave balances:</strong>
            <br />
            {error.length > 500 ? `${error.substring(0, 500)}...` : error}
          </Typography>
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Leave Type</TableCell>
              <TableCell>Balance</TableCell>
              <TableCell>Unit</TableCell>
              <TableCell>Last Updated</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {leaveBalances.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} align="center">
                  <Typography color="text.secondary">No leave balances found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              leaveBalances.map((balance) => (
                <TableRow key={balance.id} hover>
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

export default UserLeaveBalancesPage



