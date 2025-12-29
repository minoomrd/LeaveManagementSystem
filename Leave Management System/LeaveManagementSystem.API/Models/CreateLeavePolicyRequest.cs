namespace LeaveManagementSystem.API.Models;

/// <summary>
/// Request model for creating/updating leave policy that accepts string enum values
/// </summary>
public class CreateLeavePolicyRequest
{
    public decimal EntitlementAmount { get; set; }
    public int EntitlementUnit { get; set; }
    public string RenewalPeriod { get; set; } = string.Empty;
}

