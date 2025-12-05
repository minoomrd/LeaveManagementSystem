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
  Chip,
  CircularProgress,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Alert,
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
const LeaveRequestsPage: React.FC = () => {
  const { user } = useAuth()
  const [leaveRequests, setLeaveRequests] = useState<LeaveRequest[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [openDialog, setOpenDialog] = useState<boolean>(false)
  const [newRequest, setNewRequest] = useState<CreateLeaveRequestDto>({
    leaveTypeId: '',
    startDateTime: '',
    endDateTime: '',
    reason: '',
  })
  const [error, setError] = useState<string>('')

  /**
   * Load leave requests
   * Following Single Responsibility Principle - loads only leave requests
   */
  useEffect(() => {
    loadLeaveRequests()
  }, [])

  /**
   * Load all leave requests from API
   * Following Command pattern - encapsulates load operation
   */
  const loadLeaveRequests = async () => {
    try {
      setIsLoading(true)
      const requests = await leaveRequestService.getAllLeaveRequests()
      setLeaveRequests(requests)
    } catch (error) {
      console.error('Failed to load leave requests:', error)
      setError('Failed to load leave requests')
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
      await leaveRequestService.createLeaveRequest(newRequest)
      setOpenDialog(false)
      setNewRequest({ leaveTypeId: '', startDateTime: '', endDateTime: '', reason: '' })
      loadLeaveRequests()
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Failed to create leave request')
    }
  }

  /**
   * Get status chip color
   * Following Single Responsibility Principle - single purpose function
   */
  const getStatusColor = (status: LeaveRequestStatus): 'default' | 'primary' | 'success' | 'error' => {
    switch (status) {
      case LeaveRequestStatus.Pending:
        return 'default'
      case LeaveRequestStatus.Approved:
        return 'success'
      case LeaveRequestStatus.Rejected:
        return 'error'
      default:
        return 'default'
    }
  }

  /**
   * Get status label
   * Following Single Responsibility Principle - single purpose function
   */
  const getStatusLabel = (status: LeaveRequestStatus): string => {
    switch (status) {
      case LeaveRequestStatus.Pending:
        return 'Pending'
      case LeaveRequestStatus.Approved:
        return 'Approved'
      case LeaveRequestStatus.Rejected:
        return 'Rejected'
      default:
        return 'Unknown'
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
            Leave Requests
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage and track leave requests
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
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Employee</TableCell>
              <TableCell>Leave Type</TableCell>
              <TableCell>Start Date</TableCell>
              <TableCell>End Date</TableCell>
              <TableCell>Duration</TableCell>
              <TableCell>Reason</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {leaveRequests.length === 0 ? (
              <TableRow>
                <TableCell colSpan={8} align="center">
                  <Typography color="text.secondary">No leave requests found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              leaveRequests.map((request) => (
                <TableRow key={request.id} hover>
                  <TableCell>{request.userName}</TableCell>
                  <TableCell>{request.leaveTypeName}</TableCell>
                  <TableCell>{formatDate(request.startDateTime)}</TableCell>
                  <TableCell>{formatDate(request.endDateTime)}</TableCell>
                  <TableCell>
                    {request.durationAmount} {request.durationUnit === 1 ? 'Day(s)' : 'Hour(s)'}
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
            <TextField
              label="Leave Type ID"
              value={newRequest.leaveTypeId}
              onChange={(e) => setNewRequest({ ...newRequest, leaveTypeId: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Start Date & Time"
              type="datetime-local"
              value={newRequest.startDateTime}
              onChange={(e) => setNewRequest({ ...newRequest, startDateTime: e.target.value })}
              fullWidth
              required
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="End Date & Time"
              type="datetime-local"
              value={newRequest.endDateTime}
              onChange={(e) => setNewRequest({ ...newRequest, endDateTime: e.target.value })}
              fullWidth
              required
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="Reason"
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

export default LeaveRequestsPage

