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

  /**
   * Create a new role
   * @param roleData - Role creation data
   * @returns Created role
   */
  async createRole(roleData: { name: string; description?: string; isActive?: boolean }): Promise<Role> {
    return this.post<Role>('/roles', roleData)
  }

  /**
   * Update a role
   * @param id - Role ID
   * @param roleData - Role update data
   * @returns Updated role
   */
  async updateRole(id: string, roleData: { name: string; description?: string; isActive?: boolean }): Promise<Role> {
    return this.put<Role>(`/roles/${id}`, roleData)
  }

  /**
   * Delete a role
   * @param id - Role ID
   */
  async deleteRole(id: string): Promise<void> {
    return this.delete<void>(`/roles/${id}`)
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
    // Send empty string instead of null if no comment provided
    const payload = { 
      adminComment: comment && comment.trim() ? comment.trim() : '' 
    }
    console.log(`API: Approving request ${id} with payload:`, payload)
    const result = await this.post<LeaveRequest>(`/LeaveRequests/${id}/approve`, payload)
    console.log(`API: Approve result:`, result)
    return result
  }

  /**
   * Reject a leave request
   * @param id - Leave request ID
   * @param comment - Admin comment
   * @returns Updated leave request
   */
  async rejectLeaveRequest(id: string, comment: string): Promise<LeaveRequest> {
    const payload = { adminComment: comment.trim() }
    console.log(`API: Rejecting request ${id} with payload:`, payload)
    return this.post<LeaveRequest>(`/LeaveRequests/${id}/reject`, payload)
  }

  /**
   * Review a leave request (approve or reject)
   * @param id - Leave request ID
   * @param status - Status: "accept" or "reject"
   * @param adminComment - Optional admin comment
   * @returns Updated leave request
   */
  async reviewLeaveRequest(id: string, status: 'accept' | 'reject', adminComment?: string): Promise<LeaveRequest> {
    const payload = {
      Status: status,
      AdminComment: adminComment && adminComment.trim() ? adminComment.trim() : undefined
    }
    console.log(`API: Reviewing request ${id} with payload:`, payload)
    const result = await this.post<LeaveRequest>(`/LeaveRequests/${id}/review`, payload)
    console.log(`API: Review result:`, result)
    return result
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
    return this.get<LeaveBalance[]>(`/LeaveBalances/user/${userId}`)
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

/**
 * Leave Policy Interface
 */
export interface LeavePolicy {
  id: string
  leaveTypeId: string
  leaveTypeName: string
  isSickLeave: boolean
  entitlementAmount: number
  entitlementUnit: number // 1 = Hour, 2 = Day
  renewalPeriod: string // 'Weekly' | 'Monthly' | 'Yearly'
  createdAt: string
  updatedAt: string
}

/**
 * Leave Policy Service
 * Following Single Responsibility Principle - handles only leave policy operations
 */
export class LeavePolicyService extends ApiService {
  /**
   * Get all leave policies
   * @returns Array of leave policies
   */
  async getAllLeavePolicies(): Promise<LeavePolicy[]> {
    return this.get<LeavePolicy[]>('/LeavePolicies')
  }

  /**
   * Get leave policies by leave type ID
   * @param leaveTypeId - Leave type ID
   * @returns Array of leave policies
   */
  async getLeavePoliciesByLeaveTypeId(leaveTypeId: string): Promise<LeavePolicy[]> {
    return this.get<LeavePolicy[]>(`/LeavePolicies/leave-type/${leaveTypeId}`)
  }

  /**
   * Create or update a leave policy for a leave type
   * @param leaveTypeId - Leave type ID
   * @param policyData - Leave policy data
   * @returns Created or updated leave policy
   */
  async createOrUpdateLeavePolicy(
    leaveTypeId: string,
    policyData: {
      entitlementAmount: number
      entitlementUnit: number // 1 = Hour, 2 = Day
      renewalPeriod: string // 'Weekly' | 'Monthly' | 'Yearly'
    }
  ): Promise<LeavePolicy> {
    return this.post<LeavePolicy>(`/LeavePolicies/leave-type/${leaveTypeId}`, policyData)
  }
}

// Export singleton instances
export const authService = new AuthService()
export const userService = new UserService()
export const roleService = new RoleService()
export const leaveRequestService = new LeaveRequestService()
export const leaveBalanceService = new LeaveBalanceService()
export const leaveTypeService = new LeaveTypeService()
export const leavePolicyService = new LeavePolicyService()

