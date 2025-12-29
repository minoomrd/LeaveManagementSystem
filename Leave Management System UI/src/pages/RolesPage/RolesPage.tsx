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
  IconButton,
  Switch,
  FormControlLabel,
} from '@mui/material'
import { Add as AddIcon, Delete as DeleteIcon, Edit as EditIcon } from '@mui/icons-material'
import { Role } from '../../types/models'
import { roleService } from '../../services/ApiService'

/**
 * Roles Page Component
 * Following Single Responsibility Principle - manages only roles
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const RolesPage: React.FC = () => {
  const [roles, setRoles] = useState<Role[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [openDialog, setOpenDialog] = useState<boolean>(false)
  const [openDeleteDialog, setOpenDeleteDialog] = useState<boolean>(false)
  const [selectedRole, setSelectedRole] = useState<Role | null>(null)
  const [isEditMode, setIsEditMode] = useState<boolean>(false)
  const [roleData, setRoleData] = useState<{ name: string; description: string; isActive: boolean }>({
    name: '',
    description: '',
    isActive: true,
  })
  const [error, setError] = useState<string>('')

  /**
   * Load roles
   * Following Single Responsibility Principle - loads only roles
   */
  useEffect(() => {
    loadRoles()
  }, [])

  /**
   * Load roles from API
   * Following Command pattern - encapsulates load operation
   */
  const loadRoles = async () => {
    try {
      setIsLoading(true)
      const rolesData = await roleService.getAllRoles()
      setRoles(rolesData)
      setError('')
    } catch (error) {
      console.error('Failed to load roles:', error)
      setError('Failed to load roles')
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Handle open create dialog
   */
  const handleOpenCreateDialog = () => {
    setIsEditMode(false)
    setRoleData({ name: '', description: '', isActive: true })
    setSelectedRole(null)
    setError('')
    setOpenDialog(true)
  }

  /**
   * Handle open edit dialog
   */
  const handleOpenEditDialog = (role: Role) => {
    setIsEditMode(true)
    setRoleData({
      name: role.name,
      description: role.description || '',
      isActive: role.isActive,
    })
    setSelectedRole(role)
    setError('')
    setOpenDialog(true)
  }

  /**
   * Handle open delete dialog
   */
  const handleOpenDeleteDialog = (role: Role) => {
    setSelectedRole(role)
    setError('')
    setOpenDeleteDialog(true)
  }

  /**
   * Handle save role (create or update)
   * Following Command pattern - encapsulates save operation
   */
  const handleSaveRole = async () => {
    try {
      setError('')
      if (!roleData.name.trim()) {
        setError('Role name is required')
        return
      }

      if (isEditMode && selectedRole) {
        // Update existing role
        await roleService.updateRole(selectedRole.id, {
          name: roleData.name.trim(),
          description: roleData.description.trim() || undefined,
          isActive: roleData.isActive,
        })
      } else {
        // Create new role
        await roleService.createRole({
          name: roleData.name.trim(),
          description: roleData.description.trim() || undefined,
          isActive: roleData.isActive,
        })
      }

      setOpenDialog(false)
      setRoleData({ name: '', description: '', isActive: true })
      setSelectedRole(null)
      loadRoles()
    } catch (error: any) {
      console.error('Failed to save role:', error)
      let errorMessage = isEditMode ? 'Failed to update role' : 'Failed to create role'
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
      setError(errorMessage)
    }
  }

  /**
   * Handle delete role
   * Following Command pattern - encapsulates delete operation
   */
  const handleDeleteRole = async () => {
    if (!selectedRole) return

    try {
      setError('')
      await roleService.deleteRole(selectedRole.id)
      setOpenDeleteDialog(false)
      setSelectedRole(null)
      loadRoles()
    } catch (error: any) {
      console.error('Failed to delete role:', error)
      let errorMessage = 'Failed to delete role'
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
      setError(errorMessage)
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
            Roles Management
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage system roles and permissions
          </Typography>
        </Box>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleOpenCreateDialog}
        >
          New Role
        </Button>
      </Box>

      {error && (
        <Alert 
          severity="error" 
          sx={{ mb: 2 }} 
          onClose={() => setError('')}
        >
          {error}
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Created At</TableCell>
              <TableCell>Updated At</TableCell>
              <TableCell align="center">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {roles.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography color="text.secondary">No roles found</Typography>
                </TableCell>
              </TableRow>
            ) : (
              roles.map((role) => (
                <TableRow key={role.id} hover>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {role.name}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" color="text.secondary">
                      {role.description || '-'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={role.isActive ? 'Active' : 'Inactive'}
                      color={role.isActive ? 'success' : 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{formatDate(role.createdAt)}</TableCell>
                  <TableCell>{formatDate(role.updatedAt)}</TableCell>
                  <TableCell align="center">
                    <IconButton
                      color="primary"
                      size="small"
                      onClick={() => handleOpenEditDialog(role)}
                      sx={{ mr: 1 }}
                    >
                      <EditIcon />
                    </IconButton>
                    <IconButton
                      color="error"
                      size="small"
                      onClick={() => handleOpenDeleteDialog(role)}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create/Edit Role Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{isEditMode ? 'Edit Role' : 'Create New Role'}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <TextField
              label="Role Name"
              value={roleData.name}
              onChange={(e) => setRoleData({ ...roleData, name: e.target.value })}
              fullWidth
              required
              disabled={isEditMode && (selectedRole?.name === 'Admin' || selectedRole?.name === 'SuperAdmin')}
              helperText={
                isEditMode && (selectedRole?.name === 'Admin' || selectedRole?.name === 'SuperAdmin')
                  ? 'Cannot edit system roles'
                  : ''
              }
            />
            <TextField
              label="Description"
              value={roleData.description}
              onChange={(e) => setRoleData({ ...roleData, description: e.target.value })}
              fullWidth
              multiline
              rows={3}
            />
            <FormControlLabel
              control={
                <Switch
                  checked={roleData.isActive}
                  onChange={(e) => setRoleData({ ...roleData, isActive: e.target.checked })}
                  disabled={isEditMode && (selectedRole?.name === 'Admin' || selectedRole?.name === 'SuperAdmin')}
                />
              }
              label="Active"
            />
            {error && (
              <Alert severity="error" onClose={() => setError('')}>
                {error}
              </Alert>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleSaveRole} variant="contained">
            {isEditMode ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Role</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the role <strong>"{selectedRole?.name}"</strong>?
            {selectedRole?.name === 'Admin' || selectedRole?.name === 'SuperAdmin' ? (
              <Alert severity="warning" sx={{ mt: 2 }}>
                System roles cannot be deleted.
              </Alert>
            ) : (
              <Alert severity="warning" sx={{ mt: 2 }}>
                This action cannot be undone. Users with this role may be affected.
              </Alert>
            )}
          </Typography>
          {error && (
            <Alert severity="error" sx={{ mt: 2 }} onClose={() => setError('')}>
              {error}
            </Alert>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button
            onClick={handleDeleteRole}
            variant="contained"
            color="error"
            disabled={selectedRole?.name === 'Admin' || selectedRole?.name === 'SuperAdmin'}
          >
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}

export default RolesPage

