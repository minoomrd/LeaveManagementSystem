import { useEffect, useState, useMemo } from 'react'
import {
  Box,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TableSortLabel,
  Paper,
  CircularProgress,
  TextField,
  InputAdornment,
} from '@mui/material'
import { Search as SearchIcon } from '@mui/icons-material'
import { LeaveBalance } from '../../types/models'
import { leaveBalanceService } from '../../services/ApiService'
import { useAuth } from '../../contexts/AuthContext'

type Order = 'asc' | 'desc'
type OrderBy = 'userName' | 'leaveTypeName' | 'currentYearEntitlement' | 'carryoverFromPreviousYears' | 'usedThisYear' | 'remainingBalance' | 'updatedAt'

/**
 * Leave Balances Page Component
 * Following Single Responsibility Principle - displays only leave balances
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const LeaveBalancesPage: React.FC = () => {
  const { user } = useAuth()
  const [leaveBalances, setLeaveBalances] = useState<LeaveBalance[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [searchTerm, setSearchTerm] = useState<string>('')
  const [order, setOrder] = useState<Order>('asc')
  const [orderBy, setOrderBy] = useState<OrderBy>('userName')

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
   * This page is for Admin/SuperAdmin only - shows all balances
   */
  const loadLeaveBalances = async () => {
    try {
      setIsLoading(true)
      if (!user?.id) {
        return
      }

      // This page is now for Admin/SuperAdmin to see ALL balances
      console.log('Loading ALL leave balances for Admin/SuperAdmin. Role:', user.roleName)
      const balances = await leaveBalanceService.getAllLeaveBalances()
      console.log('Successfully loaded', balances.length, 'leave balances')

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

  /**
   * Handle sort request
   * Following Single Responsibility Principle - handles only sorting logic
   */
  const handleRequestSort = (property: OrderBy) => {
    const isAsc = orderBy === property && order === 'asc'
    setOrder(isAsc ? 'desc' : 'asc')
    setOrderBy(property)
  }

  /**
   * Filter and sort leave balances
   * Following Single Responsibility Principle - handles only filtering and sorting
   */
  const filteredAndSortedBalances = useMemo(() => {
    // Filter by search term (employee name)
    let filtered = leaveBalances.filter((balance) =>
      balance.userName.toLowerCase().includes(searchTerm.toLowerCase())
    )

    // Sort the filtered results
    return [...filtered].sort((a, b) => {
      let aValue: any
      let bValue: any

      switch (orderBy) {
        case 'userName':
          aValue = a.userName.toLowerCase()
          bValue = b.userName.toLowerCase()
          break
        case 'leaveTypeName':
          aValue = a.leaveTypeName.toLowerCase()
          bValue = b.leaveTypeName.toLowerCase()
          break
        case 'currentYearEntitlement':
          aValue = a.currentYearEntitlement ?? 0
          bValue = b.currentYearEntitlement ?? 0
          break
        case 'carryoverFromPreviousYears':
          aValue = a.carryoverFromPreviousYears ?? 0
          bValue = b.carryoverFromPreviousYears ?? 0
          break
        case 'usedThisYear':
          aValue = a.usedThisYear ?? 0
          bValue = b.usedThisYear ?? 0
          break
        case 'remainingBalance':
          aValue = a.remainingBalance ?? a.balanceAmount
          bValue = b.remainingBalance ?? b.balanceAmount
          break
        case 'updatedAt':
          aValue = new Date(a.updatedAt).getTime()
          bValue = new Date(b.updatedAt).getTime()
          break
        default:
          return 0
      }

      if (aValue < bValue) {
        return order === 'asc' ? -1 : 1
      }
      if (aValue > bValue) {
        return order === 'asc' ? 1 : -1
      }
      return 0
    })
  }, [leaveBalances, searchTerm, order, orderBy])

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

      {/* Search Field */}
      <Box sx={{ mb: 2, maxWidth: 400 }}>
        <TextField
          fullWidth
          placeholder="Search by employee name..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ mb: 2 }}
        />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'userName'}
                  direction={orderBy === 'userName' ? order : 'asc'}
                  onClick={() => handleRequestSort('userName')}
                >
                  Employee
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'leaveTypeName'}
                  direction={orderBy === 'leaveTypeName' ? order : 'asc'}
                  onClick={() => handleRequestSort('leaveTypeName')}
                >
                  Leave Type
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">
                <TableSortLabel
                  active={orderBy === 'currentYearEntitlement'}
                  direction={orderBy === 'currentYearEntitlement' ? order : 'asc'}
                  onClick={() => handleRequestSort('currentYearEntitlement')}
                >
                  Current Year Entitlement
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">
                <TableSortLabel
                  active={orderBy === 'carryoverFromPreviousYears'}
                  direction={orderBy === 'carryoverFromPreviousYears' ? order : 'asc'}
                  onClick={() => handleRequestSort('carryoverFromPreviousYears')}
                >
                  Carryover (Previous Years)
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">
                <TableSortLabel
                  active={orderBy === 'usedThisYear'}
                  direction={orderBy === 'usedThisYear' ? order : 'asc'}
                  onClick={() => handleRequestSort('usedThisYear')}
                >
                  Used This Year
                </TableSortLabel>
              </TableCell>
              <TableCell align="right">
                <TableSortLabel
                  active={orderBy === 'remainingBalance'}
                  direction={orderBy === 'remainingBalance' ? order : 'asc'}
                  onClick={() => handleRequestSort('remainingBalance')}
                >
                  Remaining Balance
                </TableSortLabel>
              </TableCell>
              <TableCell>Unit</TableCell>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'updatedAt'}
                  direction={orderBy === 'updatedAt' ? order : 'asc'}
                  onClick={() => handleRequestSort('updatedAt')}
                >
                  Last Updated
                </TableSortLabel>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredAndSortedBalances.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} align="center">
                  <Typography color="text.secondary">
                    {searchTerm ? 'No leave balances found matching your search' : 'No leave balances found'}
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              filteredAndSortedBalances.map((balance) => {
                const unit = balance.balanceUnit === 1 ? 'Day(s)' : 'Hour(s)'
                const currentYearEntitlement = balance.currentYearEntitlement ?? 0
                const carryover = balance.carryoverFromPreviousYears ?? 0
                const usedThisYear = balance.usedThisYear ?? 0
                const remainingBalance = balance.remainingBalance ?? balance.balanceAmount
                
                return (
                  <TableRow key={balance.id} hover>
                    <TableCell>{balance.userName}</TableCell>
                    <TableCell>{balance.leaveTypeName}</TableCell>
                    <TableCell align="right">
                      <Typography variant="body2" fontWeight="medium">
                        {currentYearEntitlement.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="body2" color={carryover > 0 ? 'success.main' : 'text.secondary'}>
                        {carryover.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="body2" color="error.main" fontWeight="medium">
                        {usedThisYear.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell align="right">
                      <Typography variant="h6" component="span" color="primary" fontWeight="bold">
                        {remainingBalance.toFixed(2)}
                      </Typography>
                    </TableCell>
                    <TableCell>{unit}</TableCell>
                    <TableCell>{formatDate(balance.updatedAt)}</TableCell>
                  </TableRow>
                )
              })
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  )
}

export default LeaveBalancesPage

