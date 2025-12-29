import { useEffect, useState } from 'react'
import {
  Box,
  Typography,
  Paper,
  CircularProgress,
  Button,
  TextField,
  Alert,
  Grid,
  Card,
  CardContent,
  Divider,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
} from '@mui/material'
import { Save as SaveIcon } from '@mui/icons-material'
import { leaveTypeService, leavePolicyService, LeavePolicy } from '../../services/ApiService'
import { LeaveUnit } from '../../types/models'

/**
 * Leave Type Configuration Interface
 */
interface LeaveTypeConfig {
  id: string
  name: string
  unit: number // 1 = Hour, 2 = Day
  description?: string
  isSickLeave: boolean
  entitlementAmount?: number
  renewalPeriod?: string
}

/**
 * Configuration Page Component
 * Following Single Responsibility Principle - manages only company leave configuration
 * Following Dependency Inversion Principle - depends on service abstractions
 */
const ConfigurationPage: React.FC = () => {
  const [leaveTypes, setLeaveTypes] = useState<LeaveTypeConfig[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [isSaving, setIsSaving] = useState<boolean>(false)
  const [error, setError] = useState<string>('')
  const [success, setSuccess] = useState<string>('')

  // Configuration state
  const [sickLeaveHours, setSickLeaveHours] = useState<number>(40)
  const [sickLeaveRenewalPeriod, setSickLeaveRenewalPeriod] = useState<string>('Yearly')
  const [annualLeaveDays, setAnnualLeaveDays] = useState<number>(20)
  const [annualLeaveRenewalPeriod, setAnnualLeaveRenewalPeriod] = useState<string>('Yearly')

  /**
   * Load leave types and configuration
   */
  useEffect(() => {
    loadConfiguration()
  }, [])

  /**
   * Load leave types and existing policies from API
   */
  const loadConfiguration = async () => {
    try {
      setIsLoading(true)
      setError('')
      
      // Load leave types first
      const types = await leaveTypeService.getAllLeaveTypes()
      
      // Try to load policies, but don't fail if they don't exist yet
      let policies: LeavePolicy[] = []
      try {
        policies = await leavePolicyService.getAllLeavePolicies()
      } catch (policyError) {
        // Policies might not exist yet - that's okay, we'll use defaults
        console.log('No policies found yet, using defaults:', policyError)
      }
      
      // Find sick leave and annual leave types
      const sickLeave = types.find(t => t.isSickLeave)
      const annualLeave = types.find(t => !t.isSickLeave)
      
      // Load existing policies or use defaults
      if (sickLeave) {
        const sickPolicy = policies.find(p => p.leaveTypeId === sickLeave.id)
        if (sickPolicy) {
          setSickLeaveHours(sickPolicy.entitlementAmount)
          setSickLeaveRenewalPeriod(sickPolicy.renewalPeriod)
        } else {
          setSickLeaveHours(40) // Default 40 hours
          setSickLeaveRenewalPeriod('Yearly')
        }
      }
      
      if (annualLeave) {
        const annualPolicy = policies.find(p => p.leaveTypeId === annualLeave.id)
        if (annualPolicy) {
          setAnnualLeaveDays(annualPolicy.entitlementAmount)
          setAnnualLeaveRenewalPeriod(annualPolicy.renewalPeriod)
        } else {
          setAnnualLeaveDays(20) // Default 20 days
          setAnnualLeaveRenewalPeriod('Yearly')
        }
      }
      
      setLeaveTypes(types.map(t => ({
        id: t.id,
        name: t.name,
        unit: t.unit,
        description: t.description,
        isSickLeave: t.isSickLeave,
      })))
    } catch (error: any) {
      console.error('Failed to load configuration:', error)
      let errorMessage = 'Failed to load leave configuration'
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
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Handle save configuration
   */
  const handleSaveConfiguration = async () => {
    try {
      setIsSaving(true)
      setError('')
      setSuccess('')

      // Validate inputs
      if (sickLeaveHours <= 0) {
        setError('Sick leave hours must be greater than 0')
        return
      }
      if (annualLeaveDays <= 0) {
        setError('Annual leave days must be greater than 0')
        return
      }

      // Find leave types
      const sickLeave = leaveTypes.find(t => t.isSickLeave)
      const annualLeave = leaveTypes.find(t => !t.isSickLeave)

      if (!sickLeave || !annualLeave) {
        setError('Leave types not found. Please ensure sick leave and annual leave types exist.')
        return
      }

      // Save both policies
      await Promise.all([
        leavePolicyService.createOrUpdateLeavePolicy(sickLeave.id, {
          entitlementAmount: sickLeaveHours,
          entitlementUnit: LeaveUnit.Hour, // Hour = 2
          renewalPeriod: sickLeaveRenewalPeriod
        }),
        leavePolicyService.createOrUpdateLeavePolicy(annualLeave.id, {
          entitlementAmount: annualLeaveDays,
          entitlementUnit: LeaveUnit.Day, // Day = 1
          renewalPeriod: annualLeaveRenewalPeriod
        })
      ])

      setSuccess('Configuration saved successfully!')
      
      // Reload configuration to show updated values
      await loadConfiguration()
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(''), 3000)
    } catch (error: any) {
      console.error('Failed to save configuration:', error)
      let errorMessage = 'Failed to save configuration'
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
    } finally {
      setIsSaving(false)
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
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" gutterBottom>
          Company Configuration
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Configure leave balance settings for your company
        </Typography>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess('')}>
          {success}
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Sick Leave Configuration */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Sick Leave Configuration
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                Configure sick leave entitlement in hours
              </Typography>
              
              <TextField
                label="Sick Leave Hours"
                type="number"
                value={sickLeaveHours}
                onChange={(e) => setSickLeaveHours(Number(e.target.value))}
                fullWidth
                required
                inputProps={{ min: 1, step: 1 }}
                helperText="Total sick leave hours per renewal period"
                sx={{ mb: 2 }}
              />

              <FormControl fullWidth>
                <InputLabel>Renewal Period</InputLabel>
                <Select
                  value={sickLeaveRenewalPeriod}
                  onChange={(e) => setSickLeaveRenewalPeriod(e.target.value)}
                  label="Renewal Period"
                >
                  <MenuItem value="Weekly">Weekly</MenuItem>
                  <MenuItem value="Monthly">Monthly</MenuItem>
                  <MenuItem value="Yearly">Yearly</MenuItem>
                </Select>
              </FormControl>

              <Box sx={{ mt: 2, p: 2, bgcolor: 'info.light', borderRadius: 1 }}>
                <Typography variant="body2">
                  <strong>Current Setting:</strong> {sickLeaveHours} hours per {sickLeaveRenewalPeriod.toLowerCase()}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Annual Leave Configuration */}
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Annual Leave Configuration
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                Configure annual leave entitlement in days
              </Typography>
              
              <TextField
                label="Annual Leave Days"
                type="number"
                value={annualLeaveDays}
                onChange={(e) => setAnnualLeaveDays(Number(e.target.value))}
                fullWidth
                required
                inputProps={{ min: 1, step: 1 }}
                helperText="Total annual leave days per renewal period"
                sx={{ mb: 2 }}
              />

              <FormControl fullWidth>
                <InputLabel>Renewal Period</InputLabel>
                <Select
                  value={annualLeaveRenewalPeriod}
                  onChange={(e) => setAnnualLeaveRenewalPeriod(e.target.value)}
                  label="Renewal Period"
                >
                  <MenuItem value="Weekly">Weekly</MenuItem>
                  <MenuItem value="Monthly">Monthly</MenuItem>
                  <MenuItem value="Yearly">Yearly</MenuItem>
                </Select>
              </FormControl>

              <Box sx={{ mt: 2, p: 2, bgcolor: 'success.light', borderRadius: 1 }}>
                <Typography variant="body2">
                  <strong>Current Setting:</strong> {annualLeaveDays} days per {annualLeaveRenewalPeriod.toLowerCase()}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end' }}>
        <Button
          variant="contained"
          startIcon={<SaveIcon />}
          onClick={handleSaveConfiguration}
          disabled={isSaving}
          size="large"
        >
          {isSaving ? 'Saving...' : 'Save Configuration'}
        </Button>
      </Box>

      {/* Information Card */}
      <Card sx={{ mt: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Configuration Information
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Typography variant="body2" color="text.secondary" paragraph>
            <strong>Sick Leave:</strong> This is typically used for medical emergencies and health-related absences. 
            Configure the total number of hours employees are entitled to per renewal period.
          </Typography>
          <Typography variant="body2" color="text.secondary" paragraph>
            <strong>Annual Leave:</strong> This is the standard annual/vacation leave. 
            Configure the total number of days employees are entitled to per renewal period.
          </Typography>
          <Typography variant="body2" color="text.secondary">
            <strong>Renewal Period:</strong> Defines how often the leave balance is reset. 
            Choose Weekly, Monthly, or Yearly based on your company policy.
          </Typography>
        </CardContent>
      </Card>
    </Box>
  )
}

export default ConfigurationPage

