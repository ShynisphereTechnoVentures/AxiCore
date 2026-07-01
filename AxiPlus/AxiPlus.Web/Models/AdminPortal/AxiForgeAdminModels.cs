namespace AxiPlus.Web.Models.AdminPortal;

public sealed class AxiForgeAdminProblemModel
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InputFormat { get; set; } = string.Empty;
    public string OutputFormat { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Easy";
    public string Topic { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string StarterCode { get; set; } = string.Empty;
    public string StarterCodeByLanguage { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public int TimeLimitMilliseconds { get; set; } = 1000;
    public int MemoryLimitKb { get; set; } = 262144;
    public bool IsPublished { get; set; } = true;
    public List<AxiForgeAdminTestCaseModel> TestCases { get; set; } = new();
}

public sealed class AxiForgeAdminTestCaseModel
{
    public Guid Id { get; set; }
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public int Order { get; set; } = 1;
}

public sealed class SaveAxiForgeProblemModel
{
    public Guid? Id { get; set; }
    public string? Slug { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InputFormat { get; set; } = string.Empty;
    public string OutputFormat { get; set; } = string.Empty;
    public string Constraints { get; set; } = string.Empty;
    public string Examples { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Easy";
    public string Topic { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string StarterCode { get; set; } = string.Empty;
    public string StarterCodeByLanguage { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public int TimeLimitMilliseconds { get; set; } = 1000;
    public int MemoryLimitKb { get; set; } = 262144;
    public bool IsPublished { get; set; } = true;
    public List<SaveAxiForgeTestCaseModel> TestCases { get; set; } = new();
}

public sealed class SaveAxiForgeTestCaseModel
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public int Order { get; set; } = 1;
}

public sealed class AxiForgeAdminRoadmapModel
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "Beginner";
    public bool IsPublished { get; set; } = true;
    public List<AxiForgeAdminRoadmapStepModel> Steps { get; set; } = new();
}

public sealed class AxiForgeAdminRoadmapStepModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; } = 1;
}

public sealed class SaveAxiForgeRoadmapModel
{
    public Guid? Id { get; set; }
    public string? Slug { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Level { get; set; } = "Beginner";
    public bool IsPublished { get; set; } = true;
    public List<SaveAxiForgeRoadmapStepModel> Steps { get; set; } = new();
}

public sealed class SaveAxiForgeRoadmapStepModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; } = 1;
}

public sealed class AxiForgeAdminAssessmentModel
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 30;
    public int PassingScore { get; set; } = 70;
    public bool IsPublished { get; set; } = true;
    public List<AxiForgeAdminQuestionModel> Questions { get; set; } = new();
}

public sealed class AxiForgeAdminQuestionModel
{
    public Guid Id { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = "A";
    public int Order { get; set; } = 1;
}

public sealed class SaveAxiForgeAssessmentModel
{
    public Guid? Id { get; set; }
    public string? Slug { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 30;
    public int PassingScore { get; set; } = 70;
    public bool IsPublished { get; set; } = true;
    public List<SaveAxiForgeQuestionModel> Questions { get; set; } = new();
}

public sealed class SaveAxiForgeQuestionModel
{
    public string Prompt { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string CorrectOption { get; set; } = "A";
    public int Order { get; set; } = 1;
}

public sealed class AxiForgeLessonOptionModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ModuleTitle { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}

public sealed class AxiForgeLessonPracticeMappingModel
{
    public Guid Id { get; set; }
    public Guid SourceLessonId { get; set; }
    public string SourceLessonTitle { get; set; } = string.Empty;
    public Guid ProblemId { get; set; }
    public string ProblemTitle { get; set; } = string.Empty;
    public string SourceProduct { get; set; } = "AxiPlus";
}

public sealed class SaveAxiForgeLessonPracticeMappingModel
{
    public Guid SourceLessonId { get; set; }
    public Guid ProblemId { get; set; }
    public string SourceProduct { get; set; } = "AxiPlus";
}

public sealed class AxiForgeAdminImportExportModel
{
    public List<AxiForgeAdminProblemModel> Problems { get; set; } = new();
    public List<AxiForgeAdminRoadmapModel> Roadmaps { get; set; } = new();
    public List<AxiForgeAdminAssessmentModel> Assessments { get; set; } = new();
}
