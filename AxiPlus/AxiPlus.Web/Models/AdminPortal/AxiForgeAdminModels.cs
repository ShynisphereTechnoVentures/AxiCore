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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public string StarterCode { get; set; } = string.Empty;
    public string StarterCodeByLanguage { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public int TimeLimitMilliseconds { get; set; } = 1000;
    public int MemoryLimitKb { get; set; } = 262144;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Approved";
    public DateTime? SubmittedForApprovalAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public string StarterCode { get; set; } = string.Empty;
    public string StarterCodeByLanguage { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public int TimeLimitMilliseconds { get; set; } = 1000;
    public int MemoryLimitKb { get; set; } = 262144;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Draft";
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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Approved";
    public DateTime? SubmittedForApprovalAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Draft";
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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Approved";
    public DateTime? SubmittedForApprovalAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
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
    public string ClassTags { get; set; } = string.Empty;
    public string CompanyTags { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public string ApprovalStatus { get; set; } = "Draft";
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

public sealed class AxiForgeAdminAuditEntryModel
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ActorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class AxiForgeTaxonomyItemModel
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "Class";
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public sealed class SaveAxiForgeTaxonomyItemModel
{
    public Guid? Id { get; set; }
    public string Type { get; set; } = "Class";
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public sealed class AxiForgeApprovalActionModel
{
    public string? Reason { get; set; }
}
