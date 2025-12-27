import axios, { AxiosInstance, AxiosError } from 'axios'
import { User, Role, LeaveRequest, LeaveBalance, LeaveType, CreateUserDto, CreateLeaveRequestDto, LoginDto } from '../types/models'

/**
 * API Service Base Class
 * Following Single Responsibility Principle - handles only HTTP communication
 * Following Open/Closed Principle - can be extended for new endpoints
 * Following Dependency Inversion Principle - depends on axios abstraction
 */
class ApiService {
  private api: AxiosInstance

  /**
   * Constructor initializes axios instance with base configuration
   * Following Dependency Injection principle
   */
  constructor() {
    this.api = axios.create({
      baseURL: '/api',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('authToken')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        if (error.response?.status === 401) {
          // Unauthorized - clear token and redirect to login
          localStorage.removeItem('authToken')
          window.location.href = '/login'
        }
        
        // Extract error message from response
        if (error.response?.data) {
          const errorData = error.response.data as any
          let errorMessage = 'An error occurred'
          
          // Handle different error response formats
          if (typeof errorData === 'string') {
            errorMessage = errorData
          } else if (errorData?.error) {
            errorMessage = errorData.error
          } else if (errorData?.message) {
            errorMessage = errorData.message
          } else if (typeof errorData === 'object') {
            errorMessage = JSON.stringify(errorData)
          }
          
          const enhancedError = new Error(errorMessage)
          ;(enhancedError as any).status = error.response.status
          ;(enhancedError as any).response = error.response
          return Promise.reject(enhancedError)
        }
        
        // Handle network errors or other issues
        const networkError = new Error(error.message || 'Network error occurred')
        ;(networkError as any).status = error.response?.status || 0
        return Promise.reject(networkError)
      }
    )
  }

  /**
   * Generic GET method
   * Following DRY principle - reusable for all GET requests
   */
  protected async get<T>(url: string): Promise<T> {
    const response = await this.api.get<T>(url)
    return response.data
  }

  /**
   * Generic POST method
   * Following DRY principle - reusable for all POST requests
   */
  protected async post<T>(url: string, data?: unknown): Promise<T> {
    const response = await this.api.post<T>(url, data)
    return response.data
  }

  /**
   * Generic PUT method
   * Following DRY principle - reusable for all PUT requests
   */
  protected async put<T>(url: string, data?: unknown): Promise<T> {
    const response = await this.api.put<T>(url, data)
    return response.data
  }

  /**
   * Generic DELETE method
   * Following DRY principle - reusable for all DELETE requests
   */
  protected async delete<T>(url: string): Promise<T> {
    const response = await this.api.delete<T>(url)
    return response.data
  }
}

/**
 * Authentication Service
 * Following Single Responsibility Principle - handles only authentication operations
 */
export class AuthService extends ApiService {
  /**
   * Login user with email and password
   * @param credentials - Login credentials
   * @returns User data if successful
   */
  async login(credentials: LoginDto): Promise<User> {
    const user = await this.post<User>('/auth/login', credentials)
    // Store token (if backend provides one, otherwise use a session identifier)
    if (user) {
      localStorage.setItem('authToken', 'authenticated')
      localStorage.setItem('user', JSON.stringify(user))
    }
    return user
  }

  /**
   * Logout current user
   * Following Command pattern - encapsulates logout operation
   */
  logout(): void {
    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
  }

  /**
   * Get current authenticated user
   * @returns Current user or null
   */
  getCurrentUser(): User | null {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  }
}

/**
 * User Service
 * Following Single Responsibility Principle - handles only user-related operations
 */
export class UserService extends ApiService {
  /**
   * Get all users
   * @returns Array of users
   */
  async getAllUsers(): Promise<User[]> {
    return this.get<User[]>('/database/users')
  }

  /**
   * Get user by ID
   * @param id - User ID
   * @returns User data
   */
  async getUserById(id: string): Promise<User> {
    return this.get<User>(`/users/${id}`)
  }

  /**
   * Create a new user
   * @param userData - User creation data
   * @returns Created user
   */
  async createUser(userData: CreateUserDto): Promise<User> {
    return this.post<User>('/users', userData)
  }

  /**
   * Update a user
   * @param id - User ID
   * @param userData - User update data
   * @returns Updated user
   */
  async updateUser(id: string, userData: CreateUserDto): Promise<User> {
    return this.put<User>(`/users/${id}`, userData)
  }
}

/**
 * Role Service
 * Following Single Responsibility Principle - handles only role-related operations
 */
export class RoleService extends ApiService {
  /**
   * Get all roles
   * @returns Array of roles
   */
  async getAllRoles(): Promise<Role[]> {
    return this.get<Role[]>('/roles')
  }

  /**
   * Get role by ID
   * @param id - Role ID
   * @returns Role data
   */
  async getRoleById(id: string): Promise<Role> {
    return this.get<Role>(`/roles/${id}`)
  }
}

/**
 * Leave Request Service
 * Following Single Responsibility Principle - handles only leave request operations
 */
export class LeaveRequestService extends ApiService {
  /**
   * Get all leave requests
   * @returns Array of leave requests
   */
  async getAllLeaveRequests(): Promise<LeaveRequest[]> {
    return this.get<LeaveRequest[]>('/database/leave-requests')
  }

  /**
   * Get leave requests by user ID
   * @param userId - User ID
   * @returns Array of leave requests for the user
   */
  async getLeaveRequestsByUserId(userId: string): Promise<LeaveRequest[]> {
    return this.get<LeaveRequest[]>(`/LeaveRequests/user/${userId}`)
  }

  /**
   * Get leave request by ID
   * @param id - Leave request ID
   * @returns Leave request data
   */
  async getLeaveRequestById(id: string): Promise<LeaveRequest> {
    return this.get<LeaveRequest>(`/LeaveRequests/${id}`)
  }

  /**
   * Create a new leave request
   * @param userId - User ID submitting the request
   * @param leaveRequestData - Leave request creation data
   * @returns Created leave request
   */
  async createLeaveRequest(userId: string, leaveRequestData: CreateLeaveRequestDto): Promise<LeaveRequest> {
    return this.post<LeaveRequest>(`/LeaveRequests/user/${userId}`, leaveRequestData)
  }

  /**
   * Approve a leave request
   * @param id - Leave request ID
   * @param comment - Optional admin comment
   * @returns Updated leave request
   */
  async approveLeaveRequest(id: string, comment?: string): Promise<LeaveRequest> {
    return this.post<LeaveRequest>(`/LeaveRequests/${id}/approve`, { comment })
  }

  /**
   * Reject a leave request
   * @param id - Leave request ID
   * @param comment - Admin comment
   * @returns Updated leave request
   */
  async rejectLeaveRequest(id: string, comment: string): Promise<LeaveRequest> {
    return this.post<LeaveRequest>(`/LeaveRequests/${id}/reject`, { comment })
  }
}

/**
 * Leave Balance Service
 * Following Single Responsibility Principle - handles only leave balance operations
 */
export class LeaveBalanceService extends ApiService {
  /**
   * Get all leave balances
   * @returns Array of leave balances
   */
  async getAllLeaveBalances(): Promise<LeaveBalance[]> {
    return this.get<LeaveBalance[]>('/database/leave-balances')
  }

  /**
   * Get leave balances for a specific user
   * @param userId - User ID
   * @returns Array of leave balances for the user
   */
  async getLeaveBalancesByUserId(userId: string): Promise<LeaveBalance[]> {
    const allBalances = await this.getAllLeaveBalances()
    return allBalances.filter((balance) => balance.userId === userId)
  }
}

/**
 * Leave Type Service
 * Following Single Responsibility Principle - handles only leave type operations
 */
export class LeaveTypeService extends ApiService {
  /**
   * Get all leave types
   * @returns Array of leave types
   */
  async getAllLeaveTypes(): Promise<LeaveType[]> {
    return this.get<LeaveType[]>('/database/leave-types')
  }
}

// Export singleton instances
export const authService = new AuthService()
export const userService = new UserService()
export const roleService = new RoleService()
export const leaveRequestService = new LeaveRequestService()
export const leaveBalanceService = new LeaveBalanceService()
export const leaveTypeService = new LeaveTypeService()

