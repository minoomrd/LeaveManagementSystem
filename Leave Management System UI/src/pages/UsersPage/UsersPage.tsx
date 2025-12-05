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
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from '@mui/material'
import { Add as AddIcon } from '@mui/icons-material'
import { User, UserStatus, CreateUserDto, Role } from '../../types/models'
import { userService, roleService } from '../../services/ApiService'

/**
 * Users Page Component
 * Following Single Responsibility Principle - manages only users
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<User[]>([])
  const [roles, setRoles] = useState<Role[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [openDialog, setOpenDialog] = useState<boolean>(false)
  const [newUser, setNewUser] = useState<CreateUserDto>({
    fullName: '',
    email: '',
    password: '',
    roleId: '',
  })
  const [error, setError] = useState<string>('')

  /**
   * Load users and roles
   * Following Single Responsibility Principle - loads only users and roles
   */
  useEffect(() => {
    loadData()
  }, [])

  /**
   * Load users and roles from API
   * Following Command pattern - encapsulates load operation
   */
  const loadData = async () => {
    try {
      setIsLoading(true)
      const [usersData, rolesData] = await Promise.all([
        userService.getAllUsers(),
        roleService.getAllRoles(),
      ])
      setUsers(usersData)
      setRoles(rolesData)
    } catch (error) {
      console.error('Failed to load data:', error)
      setError('Failed to load users')
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Handle create new user
   * Following Command pattern - encapsulates create operation
   */
  const handleCreateUser = async () => {
    try {
      setError('')
      if (!newUser.fullName || !newUser.email || !newUser.password || !newUser.roleId) {
        setError('Please fill in all required fields')
        return
      }
      await userService.createUser(newUser)
      setOpenDialog(false)
      setNewUser({ fullName: '', email: '', password: '', roleId: '' })
      loadData()
    } catch (error) {
      setError(error instanceof Error ? error.message : 'Failed to create user')
    }
  }

  /**
   * Get status chip color
   * Following Single Responsibility Principle - single purpose function
   */
  const getStatusColor = (status: UserStatus): 'default' | 'success' | 'error' => {
    switch (status) {
      case UserStatus.Active:
        return 'success'
      case UserStatus.Inactive:
        return 'error'
      default:
        return 'default'
    }
  }

  /**
   * Get status label
   * Following Single Responsibility Principle - single purpose function
   */
  const getStatusLabel = (status: UserStatus): string => {
    switch (status) {
      case UserStatus.Active:
        return 'Active'
      case UserStatus.Inactive:
        return 'Inactive'
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
            Users
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage system users
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => setOpenDialog(true)}
        >
          New User
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
              <TableCell>Full Name</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <Typography color="text.secondary">No users found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              users.map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.fullName}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>
                    <Chip label={user.roleName} size="small" />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getStatusLabel(user.status)}
                      color={getStatusColor(user.status)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{formatDate(user.createdAt)}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create User Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create New User</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <TextField
              label="Full Name"
              value={newUser.fullName}
              onChange={(e) => setNewUser({ ...newUser, fullName: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Email"
              type="email"
              value={newUser.email}
              onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
              fullWidth
              required
            />
            <TextField
              label="Password"
              type="password"
              value={newUser.password}
              onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
              fullWidth
              required
            />
            <FormControl fullWidth required>
              <InputLabel>Role</InputLabel>
              <Select
                value={newUser.roleId}
                label="Role"
                onChange={(e) => setNewUser({ ...newUser, roleId: e.target.value })}
              >
                {roles.map((role) => (
                  <MenuItem key={role.id} value={role.id}>
                    {role.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleCreateUser} variant="contained">
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default UsersPage

