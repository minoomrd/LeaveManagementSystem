namespace LeaveManagementSystem.Domain.Enums;

/// <summary>
/// Represents the status of a leave request.
/// Following Single Responsibility Principle - defines only leave request status values.
/// </summary>
public enum LeaveRequestStatus
{
    /// <summary>
    /// Leave request is pending approval
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Leave request has been approved by admin
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Leave request has been rejected by admin
    /// </summary>
    Rejected = 3
}

