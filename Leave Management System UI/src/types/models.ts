/**
 * Domain Models
 * Following Single Responsibility Principle - each interface represents a single entity
 * Following Interface Segregation Principle - interfaces are specific and focused
 */

/**
 * User model representing a user in the system
 */
export interface User {
  id: string
  fullName: string
  email: string
  roleId: string
  roleName: string
  status: UserStatus
  createdAt: string
}

/**
 * Role model representing a role in the system
 */
export interface Role {
  id: string
  name: string
  description?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

/**
 * Leave Request model representing a leave request
 */
export interface LeaveRequest {
  id: string
  userId: string
  userName: string
  leaveTypeId: string
  leaveTypeName: string
  startDateTime: string
  endDateTime: string
  durationAmount: number
  durationUnit: LeaveUnit
  reason?: string
  status: LeaveRequestStatus | number | string // API may send enum as string, number, or enum
  adminComment?: string
  createdAt: string
}

/**
 * Leave Balance model representing a user's leave balance
 */
export interface LeaveBalance {
  id: string
  userId: string
  userName: string
  leaveTypeId: string
  leaveTypeName: string
  balanceAmount: number
  balanceUnit: LeaveUnit
  currentYearEntitlement?: number
  usedThisYear?: number
  carryoverFromPreviousYears?: number
  remainingBalance?: number
  updatedAt: string
}

/**
 * Leave Type model representing types of leave
 */
export interface LeaveType {
  id: string
  name: string
  unit: LeaveUnit
  description?: string
  isSickLeave: boolean
}

/**
 * Company model representing a company
 */
export interface Company {
  id: string
  name: string
  contactEmail: string
  ownerId: string
  status: CompanyStatus
  createdAt: string
}

/**
 * Enums matching the backend
 */
export enum UserStatus {
  Active = 1,
  Inactive = 2,
}

export enum LeaveRequestStatus {
  Pending = 1,
  Approved = 2,
  Rejected = 3,
}

export enum LeaveUnit {
  Day = 1,
  Hour = 2,
}

export enum CompanyStatus {
  Active = 1,
  Inactive = 2,
  Suspended = 3,
}

/**
 * DTOs for creating/updating entities
 */
export interface CreateUserDto {
  fullName: string
  email: string
  password: string
  roleId: string
}

export interface CreateLeaveRequestDto {
  leaveTypeUnit: number // 1 = Hour, 2 = Day
  startDateTime: string
  endDateTime: string
  reason?: string
}

export interface LoginDto {
  email: string
  password: string
}

