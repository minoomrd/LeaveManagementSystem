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
 * Leave Requests Page Component
 * Following Single Responsibility Principle - manages only leave requests
 * Following Dependency Inversion Principle - depends on service abstractions
 */
// Static leave types - Daily (Day=2) and Hourly (Hour=1)
// Using LeaveUnit enum values: Hour = 1, Day = 2
const LEAVE_TYPES = [
  { id: '2', name: 'Daily', unit: 2 }, // 2 = Day (LeaveUnit.Day)
  { id: '1', name: 'Hourly', unit: 1 }, // 1 = Hour (LeaveUnit.Hour)
] as const

type Order = 'asc' | 'desc'
type OrderBy = 'createdAt' | 'startDateTime' | 'endDateTime' | 'userName' | 'status'

const LeaveRequestsPage: React.FC = () => {
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
  const [actionDialogOpen, setActionDialogOpen] = useState<boolean>(false)
  const [selectedRequestId, setSelectedRequestId] = useState<string | null>(null)
  const [selectedRequest, setSelectedRequest] = useState<LeaveRequest | null>(null)
  const [actionType, setActionType] = useState<'approve' | 'reject' | ''>('')
  const [adminComment, setAdminComment] = useState<string>('')
  const [order, setOrder] = useState<Order>('desc')
  const [orderBy, setOrderBy] = useState<OrderBy>('createdAt')

  /**
   * Load leave requests
   * Following Single Responsibility Principle - loads only leave requests
   */
  useEffect(() => {
    if (user?.id) {
      loadLeaveRequests()
    }
  }, [user?.id])

  /**
   * Update admin comment when dialog opens or selected request changes
   * This ensures we always have the latest adminComment data when reopening the dialog
   */
  useEffect(() => {
    if (selectedRequest && actionDialogOpen) {
      const latestRequest = leaveRequests.find(r => r.id === selectedRequest.id)
      if (latestRequest) {
        // Always update adminComment from the latest request data when dialog opens
        const latestComment = latestRequest.adminComment || ''
        console.log('useEffect: Updating adminComment', {
          latestComment,
          currentAdminComment: adminComment,
          latestRequestHasComment: !!latestRequest.adminComment
        })
        // Force update adminComment from latest request
        setAdminComment(latestComment)
        // Also update selectedRequest to ensure we have the latest data
        if (latestRequest.adminComment !== selectedRequest.adminComment || 
            latestRequest.status !== selectedRequest.status) {
          setSelectedRequest(latestRequest)
        }
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [actionDialogOpen])

  /**
   * Load all leave requests from API
   * Following Command pattern - encapsulates load operation
   * This page is for ADMIN ONLY - shows all requests from all users
   */
  const loadLeaveRequests = async () => {
    try {
      setIsLoading(true)
      if (!user?.id) {
        setError('User not authenticated')
        return
      }

      // Admin page - show all requests from all users
      const requests = await leaveRequestService.getAllLeaveRequests()
      console.log('Loaded leave requests:', requests)
      console.log('Status values in requests:', requests.map(r => ({ 
        id: r.id, 
        status: r.status, 
        statusType: typeof r.status,
        statusLabel: getStatusLabel(r.status)
      })))
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
      case 'userName':
        aValue = a.userName.toLowerCase()
        bValue = b.userName.toLowerCase()
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

  /**
   * Handle action button click
   */
  const handleActionClick = (request: LeaveRequest) => {
    // Find the latest request data from the current list to ensure we have the most up-to-date data
    const latestRequest = leaveRequests.find(r => r.id === request.id) || request
    
    setSelectedRequestId(latestRequest.id)
    setSelectedRequest(latestRequest)
    
    // Pre-populate action type and comment based on current status
    // Handle status as number, string, or enum
    let statusValue: number
    if (typeof latestRequest.status === 'string') {
      // Handle string status values like "Approved", "Rejected", "Pending"
      const statusUpper = latestRequest.status.toUpperCase()
      if (statusUpper === 'APPROVED' || statusUpper === '2') {
        statusValue = LeaveRequestStatus.Approved
      } else if (statusUpper === 'REJECTED' || statusUpper === '3') {
        statusValue = LeaveRequestStatus.Rejected
      } else {
        statusValue = LeaveRequestStatus.Pending
      }
    } else if (typeof latestRequest.status === 'number') {
      statusValue = latestRequest.status
    } else {
      // It's already an enum value, convert to number
      statusValue = latestRequest.status as number
    }
    
    // Set the radio button based on current status
    // Always pre-select if status is Approved or Rejected
    if (statusValue === LeaveRequestStatus.Approved || statusValue === 2) {
      setActionType('approve')
    } else if (statusValue === LeaveRequestStatus.Rejected || statusValue === 3) {
      setActionType('reject')
    } else {
      // For Pending or unknown status, don't pre-select
      setActionType('')
    }
    
    // Pre-populate admin comment if it exists (always fill with existing data from latest request)
    // Use the latest request data to ensure we get the most recent adminComment
    // Handle both null, undefined, and empty string cases
    const existingComment = latestRequest.adminComment 
      ? String(latestRequest.adminComment).trim() 
      : ''
    
    console.log('Opening action dialog:', {
      requestId: latestRequest.id,
      status: latestRequest.status,
      statusValue,
      actionType: statusValue === LeaveRequestStatus.Approved || statusValue === 2 ? 'approve' : statusValue === LeaveRequestStatus.Rejected || statusValue === 3 ? 'reject' : '',
      adminComment: existingComment,
      hasAdminComment: !!latestRequest.adminComment,
      latestRequestAdminComment: latestRequest.adminComment,
      rawAdminComment: latestRequest.adminComment,
      typeofAdminComment: typeof latestRequest.adminComment
    })
    
    // Set admin comment FIRST, then open dialog to ensure state is ready
    setAdminComment(existingComment)
    
    // Use requestAnimationFrame to ensure state is set before dialog renders
    requestAnimationFrame(() => {
      setActionDialogOpen(true)
    })
  }

  /**
   * Handle submit action (approve or reject) using review endpoint
   */
  const handleSubmitAction = async () => {
    if (!selectedRequestId || !actionType) {
      setError('Please select an action (Approve or Reject)')
      return
    }

    if (actionType === 'reject' && !adminComment.trim()) {
      setError('Please provide a comment for rejection')
      return
    }

    try {
      setError('')
      
      // Convert actionType to review status: 'approve' -> 'accept', 'reject' -> 'reject'
      const reviewStatus = actionType === 'approve' ? 'accept' : 'reject'
      
      console.log(`Reviewing request ${selectedRequestId} with status: ${reviewStatus}, comment:`, adminComment)
      
      // Use the review endpoint
      const result = await leaveRequestService.reviewLeaveRequest(
        selectedRequestId,
        reviewStatus,
        adminComment && adminComment.trim() ? adminComment.trim() : undefined
      )
      
      console.log(`Review result:`, result)
      console.log(`Status after review:`, result.status)
      
      // Update the request in the local state immediately
      setLeaveRequests(prevRequests => 
        prevRequests.map(req => 
          req.id === selectedRequestId 
            ? { ...req, status: result.status, adminComment: result.adminComment }
            : req
        )
      )
      
      setActionDialogOpen(false)
      setSelectedRequestId(null)
      setSelectedRequest(null)
      setActionType('')
      setAdminComment('')
      
      // Also refresh from server to ensure consistency
      await loadLeaveRequests()
    } catch (error: any) {
      console.error(`Error reviewing leave request:`, error)
      let errorMessage = `Failed to review leave request`
      if (error?.response?.data) {
        const errorData = error.response.data
        errorMessage = errorData?.error || errorData?.message || errorMessage
      } else if (error?.message) {
        errorMessage = error.message
      }
      setError(errorMessage)
    }
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
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            All Leave Requests
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage and track all employee leave requests
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
              <TableCell>
                <TableSortLabel
                  active={orderBy === 'userName'}
                  direction={orderBy === 'userName' ? order : 'asc'}
                  onClick={() => handleRequestSort('userName')}
                >
                  Employee
                </TableSortLabel>
              </TableCell>
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
              <TableCell align="center">Action</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sortedLeaveRequests.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} align="center">
                  <Typography color="text.secondary">No leave requests found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              sortedLeaveRequests.map((request) => (
                <TableRow key={request.id} hover>
                  <TableCell>{request.userName}</TableCell>
                  <TableCell>{request.leaveTypeName}</TableCell>
                  <TableCell>{formatDate(request.startDateTime)}</TableCell>
                  <TableCell>{formatDate(request.endDateTime)}</TableCell>
                  <TableCell>
                    {request.durationAmount} {request.durationUnit === 1 ? 'Day(s)' : 'Hour(s)'}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getStatusLabel(request.status)}
                      color={getStatusColor(request.status)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{formatDate(request.createdAt)}</TableCell>
                  <TableCell align="center">
                    <Button
                      variant="outlined"
                      color="primary"
                      size="small"
                      onClick={() => handleActionClick(request)}
                    >
                      Action
                    </Button>
                  </TableCell>
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

      {/* Action Dialog */}
      <Dialog open={actionDialogOpen} onClose={() => setActionDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Leave Request Action</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, pt: 2 }}>
            {selectedRequest && (
              <Box sx={{ mb: 2, p: 2, bgcolor: 'background.default', borderRadius: 1 }}>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  <strong>Employee:</strong> {selectedRequest.userName}
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  <strong>Leave Type:</strong> {selectedRequest.leaveTypeName}
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  <strong>Duration:</strong> {selectedRequest.durationAmount} {selectedRequest.durationUnit === 1 ? 'Day(s)' : 'Hour(s)'}
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom sx={{ mt: 1 }}>
                  <strong>Reason:</strong>
                </Typography>
                <Typography variant="body2" color="text.primary" sx={{ 
                  p: 1.5, 
                  bgcolor: 'grey.50', 
                  borderRadius: 1,
                  border: '1px solid',
                  borderColor: 'divider',
                  minHeight: '60px',
                  whiteSpace: 'pre-wrap',
                  wordBreak: 'break-word'
                }}>
                  {selectedRequest.reason || <em style={{ color: '#999' }}>No reason provided</em>}
                </Typography>
                {selectedRequest.adminComment && (
                  <Box sx={{ mt: 2 }}>
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      <strong>Previous Admin Comment:</strong>
                    </Typography>
                    <Typography variant="body2" color="text.primary" sx={{ 
                      p: 1.5, 
                      bgcolor: 'info.light', 
                      borderRadius: 1,
                      border: '1px solid',
                      borderColor: 'info.main',
                      whiteSpace: 'pre-wrap',
                      wordBreak: 'break-word'
                    }}>
                      {selectedRequest.adminComment}
                    </Typography>
                  </Box>
                )}
              </Box>
            )}
            
            <FormControl component="fieldset" required>
              <FormLabel component="legend">Select Action</FormLabel>
              <RadioGroup
                value={actionType}
                onChange={(e) => {
                  const newActionType = e.target.value as 'approve' | 'reject'
                  setActionType(newActionType)
                  // Always preserve existing admin comment when switching actions
                  // Don't clear it - let the user keep or modify it
                }}
                row
              >
                <FormControlLabel
                  value="approve"
                  control={<Radio />}
                  label="Approve"
                />
                <FormControlLabel
                  value="reject"
                  control={<Radio />}
                  label="Reject"
                />
              </RadioGroup>
            </FormControl>

            <TextField
              key={`admin-comment-${selectedRequestId}-${adminComment}`}
              label={actionType === 'reject' ? 'Admin Comment (Required)' : 'Admin Comment (Optional)'}
              value={adminComment || ''}
              onChange={(e) => setAdminComment(e.target.value)}
              fullWidth
              multiline
              rows={4}
              required={actionType === 'reject'}
              error={actionType === 'reject' && !adminComment.trim()}
              helperText={
                actionType === 'reject' && !adminComment.trim()
                  ? 'Comment is required for rejection'
                  : actionType === 'approve'
                  ? 'Add an optional comment...'
                  : adminComment
                  ? 'Existing comment shown. Select an action to modify.'
                  : 'Please select an action first'
              }
              placeholder={
                actionType === 'reject'
                  ? 'Enter reason for rejection...'
                  : actionType === 'approve'
                  ? 'Add an optional comment...'
                  : adminComment
                  ? 'Existing comment is shown above'
                  : 'Select approve or reject first...'
              }
              disabled={!actionType && !adminComment}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => {
            setActionDialogOpen(false)
            // Don't clear adminComment here - let it persist for next time
            // It will be set correctly when handleActionClick is called again
            setActionType('')
            setSelectedRequestId(null)
            setSelectedRequest(null)
            // Clear adminComment only after a short delay to avoid race conditions
            setTimeout(() => {
              setAdminComment('')
            }, 100)
          }}>
            Cancel
          </Button>
          <Button
            onClick={handleSubmitAction}
            variant="contained"
            color="primary"
            disabled={!actionType || (actionType === 'reject' && !adminComment.trim())}
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default LeaveRequestsPage

