import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import { User } from '../types/models'
import { authService } from '../services/ApiService'

/**
 * Authentication Context Interface
 * Following Interface Segregation Principle - defines only auth-related state and methods
 */
interface AuthContextType {
  user: User | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
  isAuthenticated: boolean
}

/**
 * Create Auth Context
 * Following Dependency Inversion Principle - provides abstraction for auth state
 */
const AuthContext = createContext<AuthContextType | undefined>(undefined)

/**
 * AuthProvider Props
 */
interface AuthProviderProps {
  children: ReactNode
}

/**
 * Authentication Provider Component
 * Following Single Responsibility Principle - manages only authentication state
 * Following Open/Closed Principle - can be extended without modification
 */
export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState<boolean>(true)

  /**
   * Initialize auth state on mount
   * Following Lazy Initialization pattern
   */
  useEffect(() => {
    const initializeAuth = () => {
      try {
        const currentUser = authService.getCurrentUser()
        setUser(currentUser)
      } catch (error) {
        console.error('Failed to initialize auth:', error)
        setUser(null)
      } finally {
        setIsLoading(false)
      }
    }

    initializeAuth()
  }, [])

  /**
   * Login function
   * Following Command pattern - encapsulates login operation
   * @param email - User email
   * @param password - User password
   */
  const login = async (email: string, password: string): Promise<void> => {
    try {
      setIsLoading(true)
      const loggedInUser = await authService.login({ email, password })
      setUser(loggedInUser)
    } catch (error) {
      console.error('Login failed:', error)
      throw error
    } finally {
      setIsLoading(false)
    }
  }

  /**
   * Logout function
   * Following Command pattern - encapsulates logout operation
   */
  const logout = (): void => {
    authService.logout()
    setUser(null)
  }

  /**
   * Check if user is authenticated
   * Following Single Responsibility Principle - single purpose getter
   */
  const isAuthenticated = user !== null

  /**
   * Context value
   * Following Immutability principle - value object
   */
  const value: AuthContextType = {
    user,
    isLoading,
    login,
    logout,
    isAuthenticated,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

/**
 * Custom hook to use Auth Context
 * Following DRY principle - reusable hook
 * @throws Error if used outside AuthProvider
 */
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

