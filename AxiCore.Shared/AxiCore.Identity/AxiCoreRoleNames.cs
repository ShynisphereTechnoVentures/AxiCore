namespace AxiCore.Identity;

/// <summary>
/// Defines shared role names used across AxiPlus, AxiForge, and AxiHire.
/// Returns stable role constants so authorization policies do not duplicate string literals.
/// </summary>
public static class AxiCoreRoleNames
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Student = "Student";
    public const string Mentor = "Mentor";
    public const string Recruiter = "Recruiter";
}
