namespace AxiCore.Contracts;

/// <summary>
/// Carries the signed AxiPlus-to-AxiForge practice launch context.
/// Returns enough product context for AxiForge to open the exact lesson practice flow while preserving product database boundaries.
/// </summary>
public sealed record PracticeLaunchRequest(
    string SourceProduct,
    Guid StudentId,
    Guid LessonId,
    Guid? CourseId,
    string PracticeType,
    string TargetReference,
    DateTimeOffset ExpiresAt,
    string Signature);
