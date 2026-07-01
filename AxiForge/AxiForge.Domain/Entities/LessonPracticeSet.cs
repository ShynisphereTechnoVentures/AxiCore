namespace AxiForge.Domain.Entities;

public sealed class LessonPracticeSet
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SourceLessonId { get; set; }

    public Guid ProblemId { get; set; }

    public CodingProblem Problem { get; set; } = null!;

    public string SourceProduct { get; set; } = "AxiPlus";
}
