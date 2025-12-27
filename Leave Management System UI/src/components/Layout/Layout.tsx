import { Outlet, useNavigate, useLocation } from 'react-router-dom'
import {
  Box,
  Drawer,
  AppBar,
  Toolbar,
  List,
  Typography,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Avatar,
  Menu,
  MenuItem,
  IconButton,
} from '@mui/material'
import {
  Dashboard as DashboardIcon,
  Assignment as AssignmentIcon,
  AccountBalance as AccountBalanceIcon,
  People as PeopleIcon,
  Logout as LogoutIcon,
  Menu as MenuIcon,
} from '@mui/icons-material'
import { useAuth } from '../../contexts/AuthContext'
import { useState } from 'react'

/**
 * Navigation item interface
 * Following Interface Segregation Principle - focused interface
 */
interface NavItem {
  text: string
  icon: React.ReactNode
  path: string
}

/**
 * Navigation items configuration
 * Following Open/Closed Principle - easy to extend with new items
 */
const getNavItems = (userRole: string | undefined): NavItem[] => {
  const baseItems: NavItem[] = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
  ]

  // Show different leave requests page based on role
  if (userRole === 'Admin' || userRole === 'SuperAdmin') {
    baseItems.push({ text: 'Leave Requests', icon: <AssignmentIcon />, path: '/leave-requests' })
  } else {
    baseItems.push({ text: 'My Leave Requests', icon: <AssignmentIcon />, path: '/user-leave-requests' })
  }

  baseItems.push({ text: 'Leave Balances', icon: <AccountBalanceIcon />, path: '/leave-balances' })

  // Only show Users menu for Admin and SuperAdmin
  if (userRole === 'Admin' || userRole === 'SuperAdmin') {
    baseItems.push({ text: 'Users', icon: <PeopleIcon />, path: '/users' })
  }

  return baseItems
}

const DRAWER_WIDTH = 240

/**
 * Layout Component
 * Following Single Responsibility Principle - handles only layout structure
 * Following Composition pattern - composes navigation and content areas
 */
const Layout: React.FC = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { user, logout } = useAuth()
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)

  /**
   * Handle navigation to a route
   * Following Command pattern - encapsulates navigation operation
   */
  const handleNavigation = (path: string) => {
    navigate(path)
  }

  /**
   * Handle user menu open
   */
  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  /**
   * Handle user menu close
   */
  const handleMenuClose = () => {
    setAnchorEl(null)
  }

  /**
   * Handle logout
   * Following Command pattern - encapsulates logout operation
   */
  const handleLogout = () => {
    logout()
    handleMenuClose()
    navigate('/login')
  }

  /**
   * Get user initials for avatar
   * Following Single Responsibility Principle - single purpose function
   */
  const getUserInitials = (): string => {
    if (!user) return 'U'
    const names = user.fullName.split(' ')
    if (names.length >= 2) {
      return `${names[0][0]}${names[names.length - 1][0]}`.toUpperCase()
    }
    return user.fullName.substring(0, 2).toUpperCase()
  }

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* App Bar */}
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` },
          ml: { sm: `${DRAWER_WIDTH}px` },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            Leave Management System
          </Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <Typography variant="body2" sx={{ display: { xs: 'none', sm: 'block' } }}>
              {user?.fullName}
            </Typography>
            <IconButton onClick={handleMenuOpen} size="small">
              <Avatar sx={{ width: 32, height: 32, bgcolor: 'secondary.main' }}>
                {getUserInitials()}
              </Avatar>
            </IconButton>
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={handleMenuClose}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'right',
              }}
            >
              <MenuItem onClick={handleLogout}>
                <ListItemIcon>
                  <LogoutIcon fontSize="small" />
                </ListItemIcon>
                <ListItemText>Logout</ListItemText>
              </MenuItem>
            </Menu>
          </Box>
        </Toolbar>
      </AppBar>

      {/* Sidebar Drawer */}
      <Drawer
        variant="permanent"
        sx={{
          width: DRAWER_WIDTH,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: DRAWER_WIDTH,
            boxSizing: 'border-box',
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            {getNavItems(user?.roleName).map((item) => (
              <ListItem key={item.path} disablePadding>
                <ListItemButton
                  selected={location.pathname === item.path}
                  onClick={() => handleNavigation(item.path)}
                  sx={{
                    '&.Mui-selected': {
                      backgroundColor: 'primary.light',
                      color: 'white',
                      '&:hover': {
                        backgroundColor: 'primary.main',
                      },
                      '& .MuiListItemIcon-root': {
                        color: 'white',
                      },
                    },
                  }}
                >
                  <ListItemIcon
                    sx={{
                      color: location.pathname === item.path ? 'white' : 'inherit',
                    }}
                  >
                    {item.icon}
                  </ListItemIcon>
                  <ListItemText primary={item.text} />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>

      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` },
          mt: 8,
        }}
      >
        <Outlet />
      </Box>
    </Box>
  )
}

export default Layout

