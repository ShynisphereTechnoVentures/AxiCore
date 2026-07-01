namespace AxiForge.Domain.Entities;

public sealed class CodingTestCase
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProblemId { get; set; }

    public CodingProblem Problem { get; set; } = null!;

    public string Input { get; set; } = string.Empty;

    public string ExpectedOutput { get; set; } = string.Empty;

    public bool IsHidden { get; set; }

    public int Order { get; set; }
}
