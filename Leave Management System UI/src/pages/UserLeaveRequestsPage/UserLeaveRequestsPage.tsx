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
  TableSortLabel,
  Paper,
  Chip,
  CircularProgress,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
  RadioGroup,
  Radio,
  FormControlLabel,
  FormLabel,
  FormControl,
} from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { LeaveRequest, LeaveRequestStatus, CreateLeaveRequestDto } from '../../types/models'
import { leaveRequestService } from '../../services/ApiService'
import { useAuth } from '../../contexts/AuthContext'

/**
 * User Leave Requests Page Component
 * Following Single Responsibility Principle - manages only user's own leave requests
 * Following Dependency Inversion Principle - depends on service abstractions
 */
// Static leave types - Daily (Day=2) and Hourly (Hour=1)
// Using LeaveUnit enum values: Hour = 1, Day = 2
const LEAVE_TYPES = [
  { id: '2', name: 'Daily', unit: 2 }, // 2 = Day (LeaveUnit.Day)
  { id: '1', name: 'Hourly', unit: 1 }, // 1 = Hour (LeaveUnit.Hour)
] as const

type Order = 'asc' | 'desc'
type OrderBy = 'createdAt' | 'startDateTime' | 'endDateTime' | 'status'

const UserLeaveRequestsPage: React.FC = () => {
  const { user } = useAuth()
  const [leaveRequests, setLeaveRequests] = useState<LeaveRequest[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [openDialog, setOpenDialog] = useState<boolean>(false)
  const [newRequest, setNewRequest] = useState<CreateLeaveRequestDto>({
    leaveTypeUnit: 0,
    startDateTime: '',
    endDateTime: '',
    reason: '',
  })
  const [selectedLeaveTypeId, setSelectedLeaveTypeId] = useState<string>('') // For UI state only
  const [hourlyHours, setHourlyHours] = useState<number>(1)
  const [error, setError] = useState<string>('')
  const [order, setOrder] = useState<Order>('desc')
  const [orderBy, setOrderBy] = useState<OrderBy>('createdAt')

  /**
   * Load leave requests
   * Following Single Responsibility Principle - loads only user's own leave requests
   */
  useEffect(() => {
    if (user?.id) {
      loadLeaveRequests()
    }
  }, [user?.id])

  /**
   * Load user's leave requests from API
   * Uses /api/LeaveRequests/user/{userId} endpoint
   */
  const loadLeaveRequests = async () => {
    try {
      setIsLoading(true)
      if (!user?.id) {
        setError('User not authenticated')
        return
      }

      // Get only the logged-in user's requests using the user-specific endpoint
      const requests = await leaveRequestService.getLeaveRequestsByUserId(user.id)
      setLeaveRequests(requests)
      setError('') // Clear any previous errors
    } catch (error: any) {
      console.error('Failed to load leave requests:', error)
      
      // Extract error message
      let errorMessage = 'Failed to load leave requests'
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
      
      // Check if it's a server error (500) - likely the API needs to be restarted
      if (error?.status === 500 || error?.response?.status === 500) {
        errorMessage = 'Server error: The API may need to be restarted. Please contact your administrator.'
      }
      
      setError(errorMessage)
      setLeaveRequests([]) // Clear requests on error
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Handle create new leave request
   * Following Command pattern - encapsulates create operation
   */
  const handleCreateRequest = async () => {
    try {
      setError('')
      
      if (!user?.id) {
        setError('User not authenticated')
        return
      }
      
      // Validate dates for daily leave type
      if (newRequest.leaveTypeUnit === 2) {
        const startDate = newRequest.startDateTime ? newRequest.startDateTime.split('T')[0] : ''
        const endDate = newRequest.endDateTime ? newRequest.endDateTime.split('T')[0] : ''
        if (endDate && startDate && endDate < startDate) {
          setError('End date cannot be before start date')
          return
        }
      }
      
      let startDateTime: string
      let endDateTime: string
      
      // Handle different leave types
      if (newRequest.leaveTypeUnit === 1) {
        // Hourly: use selected date and add hours
        const selectedDate = new Date(newRequest.startDateTime)
        startDateTime = selectedDate.toISOString()
        // Add hours to the start date
        const endDate = new Date(selectedDate)
        endDate.setHours(endDate.getHours() + hourlyHours)
        endDateTime = endDate.toISOString()
      } else {
        // Daily: use dates as-is (already in correct format)
        startDateTime = newRequest.startDateTime
        endDateTime = newRequest.endDateTime
      }
      
      // Ensure no ID is sent - ID is auto-generated by the backend
      const requestData: CreateLeaveRequestDto = {
        leaveTypeUnit: newRequest.leaveTypeUnit,
        startDateTime: startDateTime,
        endDateTime: endDateTime,
        reason: newRequest.reason,
      }
      await leaveRequestService.createLeaveRequest(user.id, requestData)
      setOpenDialog(false)
      setNewRequest({ leaveTypeUnit: 0, startDateTime: '', endDateTime: '', reason: '' })
      setSelectedLeaveTypeId('')
      setHourlyHours(1)
      loadLeaveRequests()
    } catch (error: any) {
      // Extract error message from axios error or regular error
      let errorMessage = 'Failed to create leave request'
      if (error instanceof Error) {
        errorMessage = error.message
      } else if (error?.response?.data) {
        const errorData = error.response.data
        errorMessage = errorData?.error || errorData?.message || JSON.stringify(errorData)
      } else if (typeof error === 'string') {
        errorMessage = error
      }
      setError(errorMessage)
    }
  }

  /**
   * Get status chip color
   * Following Single Responsibility Principle - single purpose function
   * Handles both enum, numeric, and string values from API
   */
  const getStatusColor = (status: LeaveRequestStatus | number | string): 'default' | 'primary' | 'success' | 'error' => {
    // Handle string enum values from API (e.g., "Approved", "Pending", "Rejected")
    if (typeof status === 'string') {
      const statusUpper = status.toUpperCase()
      if (statusUpper === 'PENDING' || statusUpper === '1') return 'default'
      if (statusUpper === 'APPROVED' || statusUpper === '2') return 'success'
      if (statusUpper === 'REJECTED' || statusUpper === '3') return 'error'
    }
    
    // Handle numeric values
    const statusNum = typeof status === 'number' ? status : Number(status)
    switch (statusNum) {
      case LeaveRequestStatus.Pending:
      case 1:
        return 'default'
      case LeaveRequestStatus.Approved:
      case 2:
        return 'success'
      case LeaveRequestStatus.Rejected:
      case 3:
        return 'error'
      default:
        return 'default'
    }
  }

  /**
   * Get status label
   * Following Single Responsibility Principle - single purpose function
   * Handles both enum, numeric, and string values from API
   */
  const getStatusLabel = (status: LeaveRequestStatus | number | string): string => {
    // Handle string enum values from API (e.g., "Approved", "Pending", "Rejected")
    if (typeof status === 'string') {
      const statusUpper = status.toUpperCase()
      if (statusUpper === 'PENDING' || statusUpper === '1') return 'Pending'
      if (statusUpper === 'APPROVED' || statusUpper === '2') return 'Approved'
      if (statusUpper === 'REJECTED' || statusUpper === '3') return 'Rejected'
    }
    
    // Handle numeric values
    const statusNum = typeof status === 'number' ? status : Number(status)
    switch (statusNum) {
      case LeaveRequestStatus.Pending:
      case 1:
        return 'Pending'
      case LeaveRequestStatus.Approved:
      case 2:
        return 'Approved'
      case LeaveRequestStatus.Rejected:
      case 3:
        return 'Rejected'
      default:
        // Default to Pending for unknown statuses instead of showing "Unknown"
        return 'Pending'
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
      hour: '2-digit',
      minute: '2-digit',
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
   * Sort leave requests based on order and orderBy
   * Following Single Responsibility Principle - handles only sorting
   */
  const sortedLeaveRequests = [...leaveRequests].sort((a, b) => {
    let aValue: any
    let bValue: any

    switch (orderBy) {
      case 'createdAt':
        aValue = new Date(a.createdAt).getTime()
        bValue = new Date(b.createdAt).getTime()
        break
      case 'startDateTime':
        aValue = new Date(a.startDateTime).getTime()
        bValue = new Date(b.startDateTime).getTime()
        break
      case 'endDateTime':
        aValue = new Date(a.endDateTime).getTime()
        bValue = new Date(b.endDateTime).getTime()
        break
      case 'status':
        aValue = typeof a.status === 'string' ? a.status : String(a.status)
        bValue = typeof b.status === 'string' ? b.status : String(b.status)
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

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            My Leave Requests
          </Typography>
          <Typography variant="body1" color="text.secondary">
            View and manage your leave requests
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setOpenDialog(true)}
        >
          New Request
        </Button>
      </Box>

      {error && (
        <Alert 
          severity="error" 
          sx={{ mb: 2, wordBreak: 'break-word' }} 
          onClose={() => setError('')}
        >
          <Typography variant="body2" component="div">
            <strong>Error loading leave requests:</strong>
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
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'startDateTime'}
                  direction={orderBy === 'startDateTime' ? order : 'asc'}
                  onClick={() => handleRequestSort('startDateTime')}
                >
                  Start Date
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'endDateTime'}
                  direction={orderBy === 'endDateTime' ? order : 'asc'}
                  onClick={() => handleRequestSort('endDateTime')}
                >
                  End Date
                </TableSortLabel>
              </TableCell>
              <TableCell>Duration</TableCell>
              <TableCell>Reason</TableCell>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'status'}
                  direction={orderBy === 'status' ? order : 'asc'}
                  onClick={() => handleRequestSort('status')}
                >
                  Status
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'createdAt'}
                  direction={orderBy === 'createdAt' ? order : 'asc'}
                  onClick={() => handleRequestSort('createdAt')}
                >
                  Created At
                </TableSortLabel>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sortedLeaveRequests.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  <Typography color="text.secondary">No leave requests found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              sortedLeaveRequests.map((request) => (
                <TableRow key={request.id} hover>
                  <TableCell>{request.leaveTypeName}</TableCell>
                  <TableCell>{formatDate(request.startDateTime)}</TableCell>
                  <TableCell>{formatDate(request.endDateTime)}</TableCell>
                  <TableCell>
                    {request.durationAmount} {request.durationUnit === 1 ? 'Hour(s)' : 'Day(s)'}
                  </TableCell>
                  <TableCell>{request.reason || '-'}</TableCell>
                  <TableCell>
                    <Chip
                      label={getStatusLabel(request.status)}
                      color={getStatusColor(request.status)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{formatDate(request.createdAt)}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create Request Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create New Leave Request</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <FormControl component="fieldset" required>
              <FormLabel component="legend">Leave Type</FormLabel>
              <RadioGroup
                value={selectedLeaveTypeId}
                onChange={(e) => {
                  const selectedId = e.target.value
                  const selectedType = LEAVE_TYPES.find(t => t.id === selectedId)
                  setSelectedLeaveTypeId(selectedId)
                  setNewRequest({ 
                    leaveTypeUnit: selectedType ? selectedType.unit : 0, 
                    startDateTime: '', 
                    endDateTime: '', 
                    reason: newRequest.reason 
                  })
                  setHourlyHours(1)
                }}
                row
              >
                {LEAVE_TYPES.map((type) => (
                  <FormControlLabel
                    key={type.id}
                    value={type.id}
                    control={<Radio />}
                    label={type.name}
                  />
                ))}
              </RadioGroup>
            </FormControl>
            
            {selectedLeaveTypeId === '1' ? (
              // Hourly: Date picker + hours input
              <>
                <TextField
                  label="Date"
                  type="date"
                  value={newRequest.startDateTime ? newRequest.startDateTime.split('T')[0] : ''}
                  onChange={(e) => {
                    const dateValue = e.target.value
                    // Set startDateTime with the date at midnight
                    const dateTime = dateValue ? `${dateValue}T00:00:00` : ''
                    setNewRequest({ ...newRequest, startDateTime: dateTime })
                  }}
                  fullWidth
                  required
                  InputLabelProps={{ shrink: true }}
                  InputProps={{
                    style: { cursor: 'pointer' }
                  }}
                  onClick={(e) => {
                    // Open date picker when clicking anywhere on the field
                    const input = e.currentTarget.querySelector('input')
                    if (input) {
                      input.showPicker?.()
                    }
                  }}
                />
                <TextField
                  label="Number of Hours"
                  type="number"
                  value={hourlyHours}
                  onChange={(e) => setHourlyHours(Math.max(1, parseInt(e.target.value) || 1))}
                  fullWidth
                  required
                  inputProps={{ min: 1, max: 24 }}
                />
              </>
            ) : selectedLeaveTypeId === '2' ? (
              // Daily: Date range pickers (no time)
              <>
                <TextField
                  label="Start Date"
                  type="date"
                  value={newRequest.startDateTime ? newRequest.startDateTime.split('T')[0] : ''}
                  onChange={(e) => {
                    const dateValue = e.target.value
                    const dateTime = dateValue ? `${dateValue}T00:00:00` : ''
                    // If end date is before new start date, clear end date
                    const currentEndDate = newRequest.endDateTime ? newRequest.endDateTime.split('T')[0] : ''
                    let updatedEndDateTime = newRequest.endDateTime
                    if (currentEndDate && dateValue && currentEndDate < dateValue) {
                      updatedEndDateTime = ''
                    }
                    setNewRequest({ 
                      ...newRequest, 
                      startDateTime: dateTime,
                      endDateTime: updatedEndDateTime
                    })
                  }}
                  fullWidth
                  required
                  InputLabelProps={{ shrink: true }}
                  InputProps={{
                    style: { cursor: 'pointer' }
                  }}
                  onClick={(e) => {
                    // Open date picker when clicking anywhere on the field
                    const input = e.currentTarget.querySelector('input')
                    if (input) {
                      input.showPicker?.()
                    }
                  }}
                />
                <TextField
                  label="End Date"
                  type="date"
                  value={newRequest.endDateTime ? newRequest.endDateTime.split('T')[0] : ''}
                  onChange={(e) => {
                    const dateValue = e.target.value
                    const dateTime = dateValue ? `${dateValue}T23:59:59` : ''
                    setNewRequest({ ...newRequest, endDateTime: dateTime })
                  }}
                  fullWidth
                  required
                  InputLabelProps={{ shrink: true }}
                  inputProps={{ 
                    min: newRequest.startDateTime 
                      ? newRequest.startDateTime.split('T')[0] 
                      : undefined
                  }}
                  InputProps={{
                    style: { cursor: 'pointer' }
                  }}
                  onClick={(e) => {
                    // Open date picker when clicking anywhere on the field
                    const input = e.currentTarget.querySelector('input')
                    if (input) {
                      input.showPicker?.()
                    }
                  }}
                  error={
                    !!(newRequest.endDateTime && newRequest.startDateTime && 
                    newRequest.endDateTime.split('T')[0] < newRequest.startDateTime.split('T')[0])
                  }
                  helperText={
                    newRequest.endDateTime && newRequest.startDateTime && 
                    newRequest.endDateTime.split('T')[0] < newRequest.startDateTime.split('T')[0]
                      ? 'End date cannot be before start date'
                      : ''
                  }
                />
              </>
            ) : (
              // No leave type selected yet
              <Alert severity="info">
                Please select a leave type to continue
              </Alert>
            )}
            <TextField
              label="Description"
              value={newRequest.reason}
              onChange={(e) => setNewRequest({ ...newRequest, reason: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleCreateRequest} variant="contained">
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default UserLeaveRequestsPage

