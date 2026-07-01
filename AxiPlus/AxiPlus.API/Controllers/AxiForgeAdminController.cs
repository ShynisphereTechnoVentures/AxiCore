using AxiForge.Domain.Entities;
using AxiForge.Infrastructure.Data;
using AxiPlus.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin-portal/axiforge")]
public sealed class AxiForgeAdminController : ControllerBase
{
    private readonly AxiForgeDbContext _context;
    private readonly AppDbContext _axiPlusContext;

    public AxiForgeAdminController(
        AxiForgeDbContext context,
        AppDbContext axiPlusContext)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> AxiForgeAdminController");
        try
        {
            _context = context;
            _axiPlusContext = axiPlusContext;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> AxiForgeAdminController -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> AxiForgeAdminController");
        }
    }

    /// <summary>
    /// Gets all AxiForge coding problems with their test cases.
    /// Returns authoring records so AxiPlus Admin can manage the student practice bank.
    /// </summary>
    [HttpGet("problems")]
    public async Task<ActionResult<List<AxiForgeAdminProblemDto>>> GetProblems(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetProblems");
        try
        {
            var problems = await _context.CodingProblems
                .AsNoTracking()
                .Include(x => x.TestCases)
                .OrderBy(x => x.Topic)
                .ThenBy(x => x.Title)
                .Select(x => ToProblemDto(x))
                .ToListAsync(cancellationToken);

            return Ok(problems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetProblems -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetProblems");
        }
    }

    /// <summary>
    /// Creates or updates an AxiForge coding problem and its test cases.
    /// Returns the saved problem so the student portal can use the same database content immediately.
    /// </summary>
    [HttpPost("problems")]
    public async Task<ActionResult<AxiForgeAdminProblemDto>> SaveProblem(
        SaveAxiForgeProblemDto dto,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveProblem");
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Topic))
            {
                return BadRequest("Problem title and topic are required.");
            }

            var problem = dto.Id.HasValue
                ? await _context.CodingProblems
                    .Include(x => x.TestCases)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value, cancellationToken)
                : null;

            if (problem == null)
            {
                problem = new CodingProblem();
                _context.CodingProblems.Add(problem);
            }

            problem.Title = dto.Title.Trim();
            problem.Slug = BuildSlug(dto.Slug, dto.Title);
            problem.Description = dto.Description.Trim();
            problem.InputFormat = dto.InputFormat.Trim();
            problem.OutputFormat = dto.OutputFormat.Trim();
            problem.Constraints = dto.Constraints.Trim();
            problem.Examples = dto.Examples.Trim();
            problem.Explanation = dto.Explanation.Trim();
            problem.Difficulty = string.IsNullOrWhiteSpace(dto.Difficulty) ? "Easy" : dto.Difficulty.Trim();
            problem.Topic = dto.Topic.Trim();
            problem.Tags = dto.Tags.Trim();
            problem.StarterCode = dto.StarterCode;
            problem.StarterCodeByLanguage = dto.StarterCodeByLanguage;
            problem.ExpectedOutput = dto.ExpectedOutput;
            problem.TimeLimitMilliseconds = Math.Max(100, dto.TimeLimitMilliseconds);
            problem.MemoryLimitKb = Math.Max(1024, dto.MemoryLimitKb);
            problem.IsPublished = dto.IsPublished;

            problem.TestCases.Clear();
            foreach (var testCase in dto.TestCases.OrderBy(x => x.Order))
            {
                problem.TestCases.Add(new CodingTestCase
                {
                    Input = testCase.Input,
                    ExpectedOutput = testCase.ExpectedOutput,
                    IsHidden = testCase.IsHidden,
                    Order = testCase.Order
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ToProblemDto(problem));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveProblem -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveProblem");
        }
    }

    /// <summary>
    /// Archives an AxiForge coding problem by unpublishing it.
    /// Returns no content because student pages automatically hide unpublished records.
    /// </summary>
    [HttpPost("problems/{id:guid}/archive")]
    public async Task<IActionResult> ArchiveProblem(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveProblem");
        try
        {
            var problem = await _context.CodingProblems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (problem == null)
            {
                return NotFound();
            }

            problem.IsPublished = false;
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("ArchiveProblem", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveProblem -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveProblem");
        }
    }

    /// <summary>
    /// Deletes an AxiForge coding problem and its child test cases/mappings.
    /// Returns no content after the database row is removed.
    /// </summary>
    [HttpDelete("problems/{id:guid}")]
    public async Task<IActionResult> DeleteProblem(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteProblem");
        try
        {
            var problem = await _context.CodingProblems
                .Include(x => x.TestCases)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (problem == null)
            {
                return NotFound();
            }

            _context.CodingProblems.Remove(problem);
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("DeleteProblem", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteProblem -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteProblem");
        }
    }

    /// <summary>
    /// Gets all AxiForge roadmap templates with their steps.
    /// Returns authoring records for the AxiForge study plan page.
    /// </summary>
    [HttpGet("roadmaps")]
    public async Task<ActionResult<List<AxiForgeAdminRoadmapDto>>> GetRoadmaps(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetRoadmaps");
        try
        {
            var roadmaps = await _context.RoadmapTemplates
                .AsNoTracking()
                .Include(x => x.Steps)
                .OrderBy(x => x.Title)
                .Select(x => ToRoadmapDto(x))
                .ToListAsync(cancellationToken);

            return Ok(roadmaps);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetRoadmaps -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetRoadmaps");
        }
    }

    /// <summary>
    /// Creates or updates an AxiForge roadmap and its ordered steps.
    /// Returns the saved roadmap so student study plans reflect admin changes.
    /// </summary>
    [HttpPost("roadmaps")]
    public async Task<ActionResult<AxiForgeAdminRoadmapDto>> SaveRoadmap(
        SaveAxiForgeRoadmapDto dto,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveRoadmap");
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest("Roadmap title is required.");
            }

            var roadmap = dto.Id.HasValue
                ? await _context.RoadmapTemplates
                    .Include(x => x.Steps)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value, cancellationToken)
                : null;

            if (roadmap == null)
            {
                roadmap = new RoadmapTemplate();
                _context.RoadmapTemplates.Add(roadmap);
            }

            roadmap.Title = dto.Title.Trim();
            roadmap.Slug = BuildSlug(dto.Slug, dto.Title);
            roadmap.Description = dto.Description.Trim();
            roadmap.Level = string.IsNullOrWhiteSpace(dto.Level) ? "Beginner" : dto.Level.Trim();
            roadmap.IsPublished = dto.IsPublished;

            roadmap.Steps.Clear();
            foreach (var step in dto.Steps.OrderBy(x => x.Order))
            {
                roadmap.Steps.Add(new RoadmapStep
                {
                    Title = step.Title.Trim(),
                    Description = step.Description.Trim(),
                    Order = step.Order
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ToRoadmapDto(roadmap));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveRoadmap -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveRoadmap");
        }
    }

    /// <summary>
    /// Archives an AxiForge roadmap by unpublishing it.
    /// Returns no content after the roadmap is hidden from the student portal.
    /// </summary>
    [HttpPost("roadmaps/{id:guid}/archive")]
    public async Task<IActionResult> ArchiveRoadmap(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveRoadmap");
        try
        {
            var roadmap = await _context.RoadmapTemplates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (roadmap == null)
            {
                return NotFound();
            }

            roadmap.IsPublished = false;
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("ArchiveRoadmap", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveRoadmap -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveRoadmap");
        }
    }

    /// <summary>
    /// Deletes an AxiForge roadmap and its ordered steps.
    /// Returns no content after removal from the authoring database.
    /// </summary>
    [HttpDelete("roadmaps/{id:guid}")]
    public async Task<IActionResult> DeleteRoadmap(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteRoadmap");
        try
        {
            var roadmap = await _context.RoadmapTemplates
                .Include(x => x.Steps)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (roadmap == null)
            {
                return NotFound();
            }

            _context.RoadmapTemplates.Remove(roadmap);
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("DeleteRoadmap", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteRoadmap -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteRoadmap");
        }
    }

    /// <summary>
    /// Gets all AxiForge assessment templates with MCQ questions.
    /// Returns authoring records for assessment management.
    /// </summary>
    [HttpGet("assessments")]
    public async Task<ActionResult<List<AxiForgeAdminAssessmentDto>>> GetAssessments(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetAssessments");
        try
        {
            var assessments = await _context.AssessmentTemplates
                .AsNoTracking()
                .Include(x => x.Questions)
                .OrderBy(x => x.Title)
                .Select(x => ToAssessmentDto(x))
                .ToListAsync(cancellationToken);

            return Ok(assessments);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetAssessments -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetAssessments");
        }
    }

    /// <summary>
    /// Creates or updates an AxiForge assessment template and MCQ questions.
    /// Returns the saved assessment so AxiForge assessment pages use database content.
    /// </summary>
    [HttpPost("assessments")]
    public async Task<ActionResult<AxiForgeAdminAssessmentDto>> SaveAssessment(
        SaveAxiForgeAssessmentDto dto,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveAssessment");
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                return BadRequest("Assessment title is required.");
            }

            var assessment = dto.Id.HasValue
                ? await _context.AssessmentTemplates
                    .Include(x => x.Questions)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value, cancellationToken)
                : null;

            if (assessment == null)
            {
                assessment = new AssessmentTemplate();
                _context.AssessmentTemplates.Add(assessment);
            }

            assessment.Title = dto.Title.Trim();
            assessment.Slug = BuildSlug(dto.Slug, dto.Title);
            assessment.Description = dto.Description.Trim();
            assessment.DurationMinutes = Math.Max(1, dto.DurationMinutes);
            assessment.PassingScore = Math.Clamp(dto.PassingScore, 0, 100);
            assessment.IsPublished = dto.IsPublished;

            assessment.Questions.Clear();
            foreach (var question in dto.Questions.OrderBy(x => x.Order))
            {
                assessment.Questions.Add(new AssessmentQuestion
                {
                    Prompt = question.Prompt.Trim(),
                    OptionA = question.OptionA.Trim(),
                    OptionB = question.OptionB.Trim(),
                    OptionC = question.OptionC.Trim(),
                    OptionD = question.OptionD.Trim(),
                    CorrectOption = NormalizeOption(question.CorrectOption),
                    Order = question.Order
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(ToAssessmentDto(assessment));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveAssessment -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveAssessment");
        }
    }

    /// <summary>
    /// Archives an AxiForge assessment by unpublishing it.
    /// Returns no content after the assessment is hidden from students.
    /// </summary>
    [HttpPost("assessments/{id:guid}/archive")]
    public async Task<IActionResult> ArchiveAssessment(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveAssessment");
        try
        {
            var assessment = await _context.AssessmentTemplates.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (assessment == null)
            {
                return NotFound();
            }

            assessment.IsPublished = false;
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("ArchiveAssessment", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveAssessment -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ArchiveAssessment");
        }
    }

    /// <summary>
    /// Deletes an AxiForge assessment and its MCQ questions.
    /// Returns no content after removal from the database.
    /// </summary>
    [HttpDelete("assessments/{id:guid}")]
    public async Task<IActionResult> DeleteAssessment(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteAssessment");
        try
        {
            var assessment = await _context.AssessmentTemplates
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (assessment == null)
            {
                return NotFound();
            }

            _context.AssessmentTemplates.Remove(assessment);
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("DeleteAssessment", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteAssessment -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteAssessment");
        }
    }

    /// <summary>
    /// Gets AxiPlus lessons for mapping lessons to AxiForge practice problems.
    /// Returns published and draft lessons so admins can wire practice before release.
    /// </summary>
    [HttpGet("lessons")]
    public async Task<ActionResult<List<AxiForgeLessonOptionDto>>> GetLessons(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessons");
        try
        {
            var lessons = await _axiPlusContext.Lessons
                .AsNoTracking()
                .Include(x => x.Module)
                .OrderBy(x => x.Module.Order)
                .ThenBy(x => x.Order)
                .Select(x => new AxiForgeLessonOptionDto(
                    x.Id,
                    x.Title,
                    x.Module.Title,
                    x.IsPublished))
                .ToListAsync(cancellationToken);

            return Ok(lessons);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessons -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessons");
        }
    }

    /// <summary>
    /// Gets existing AxiPlus lesson to AxiForge problem mappings.
    /// Returns mapping rows used by AxiPlus practice launch routing.
    /// </summary>
    [HttpGet("lesson-mappings")]
    public async Task<ActionResult<List<AxiForgeLessonPracticeMappingDto>>> GetLessonMappings(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessonMappings");
        try
        {
            var lessonTitles = await _axiPlusContext.Lessons
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Title, cancellationToken);

            var mappings = await _context.LessonPracticeSets
                .AsNoTracking()
                .Include(x => x.Problem)
                .OrderBy(x => x.Problem.Title)
                .ToListAsync(cancellationToken);

            return Ok(mappings.Select(x => new AxiForgeLessonPracticeMappingDto(
                x.Id,
                x.SourceLessonId,
                lessonTitles.TryGetValue(x.SourceLessonId, out var lessonTitle) ? lessonTitle : "Unknown lesson",
                x.ProblemId,
                x.Problem.Title,
                x.SourceProduct)).ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessonMappings -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> GetLessonMappings");
        }
    }

    /// <summary>
    /// Creates or replaces a lesson practice mapping.
    /// Returns the saved mapping so AxiPlus launches the selected AxiForge problem.
    /// </summary>
    [HttpPost("lesson-mappings")]
    public async Task<ActionResult<AxiForgeLessonPracticeMappingDto>> SaveLessonMapping(
        SaveAxiForgeLessonPracticeMappingDto dto,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveLessonMapping");
        try
        {
            var lesson = await _axiPlusContext.Lessons
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.SourceLessonId, cancellationToken);
            var problem = await _context.CodingProblems
                .FirstOrDefaultAsync(x => x.Id == dto.ProblemId, cancellationToken);

            if (lesson == null || problem == null)
            {
                return BadRequest("Valid lesson and problem are required.");
            }

            var mapping = await _context.LessonPracticeSets
                .FirstOrDefaultAsync(x => x.SourceLessonId == dto.SourceLessonId, cancellationToken);

            if (mapping == null)
            {
                mapping = new LessonPracticeSet { SourceLessonId = dto.SourceLessonId };
                _context.LessonPracticeSets.Add(mapping);
            }

            mapping.ProblemId = dto.ProblemId;
            mapping.SourceProduct = string.IsNullOrWhiteSpace(dto.SourceProduct) ? "AxiPlus" : dto.SourceProduct.Trim();
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("SaveLessonMapping", mapping.Id);

            return Ok(new AxiForgeLessonPracticeMappingDto(
                mapping.Id,
                mapping.SourceLessonId,
                lesson.Title,
                mapping.ProblemId,
                problem.Title,
                mapping.SourceProduct));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveLessonMapping -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> SaveLessonMapping");
        }
    }

    /// <summary>
    /// Deletes a lesson practice mapping.
    /// Returns no content after AxiPlus no longer redirects that lesson to a fixed problem.
    /// </summary>
    [HttpDelete("lesson-mappings/{id:guid}")]
    public async Task<IActionResult> DeleteLessonMapping(Guid id, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteLessonMapping");
        try
        {
            var mapping = await _context.LessonPracticeSets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (mapping == null)
            {
                return NotFound();
            }

            _context.LessonPracticeSets.Remove(mapping);
            await _context.SaveChangesAsync(cancellationToken);
            WriteAudit("DeleteLessonMapping", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteLessonMapping -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> DeleteLessonMapping");
        }
    }

    /// <summary>
    /// Exports AxiForge authoring content as one JSON bundle.
    /// Returns problems, roadmaps, and assessments for backup or migration.
    /// </summary>
    [HttpGet("export")]
    public async Task<ActionResult<AxiForgeAdminImportExportDto>> ExportContent(CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ExportContent");
        try
        {
            var bundle = new AxiForgeAdminImportExportDto(
                await _context.CodingProblems.AsNoTracking().Include(x => x.TestCases).Select(x => ToProblemDto(x)).ToListAsync(cancellationToken),
                await _context.RoadmapTemplates.AsNoTracking().Include(x => x.Steps).Select(x => ToRoadmapDto(x)).ToListAsync(cancellationToken),
                await _context.AssessmentTemplates.AsNoTracking().Include(x => x.Questions).Select(x => ToAssessmentDto(x)).ToListAsync(cancellationToken));

            return Ok(bundle);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ExportContent -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ExportContent");
        }
    }

    /// <summary>
    /// Imports AxiForge authoring content from a JSON bundle.
    /// Returns no content after records are upserted into the database.
    /// </summary>
    [HttpPost("import")]
    public async Task<IActionResult> ImportContent(AxiForgeAdminImportExportDto bundle, CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ImportContent");
        try
        {
            foreach (var problem in bundle.Problems)
            {
                await SaveProblem(new SaveAxiForgeProblemDto(
                    problem.Id == Guid.Empty ? null : problem.Id,
                    problem.Slug,
                    problem.Title,
                    problem.Description,
                    problem.InputFormat,
                    problem.OutputFormat,
                    problem.Constraints,
                    problem.Examples,
                    problem.Explanation,
                    problem.Difficulty,
                    problem.Topic,
                    problem.Tags,
                    problem.StarterCode,
                    problem.StarterCodeByLanguage,
                    problem.ExpectedOutput,
                    problem.TimeLimitMilliseconds,
                    problem.MemoryLimitKb,
                    problem.IsPublished,
                    problem.TestCases.Select(x => new SaveAxiForgeTestCaseDto(x.Input, x.ExpectedOutput, x.IsHidden, x.Order)).ToList()), cancellationToken);
            }

            foreach (var roadmap in bundle.Roadmaps)
            {
                await SaveRoadmap(new SaveAxiForgeRoadmapDto(
                    roadmap.Id == Guid.Empty ? null : roadmap.Id,
                    roadmap.Slug,
                    roadmap.Title,
                    roadmap.Description,
                    roadmap.Level,
                    roadmap.IsPublished,
                    roadmap.Steps.Select(x => new SaveAxiForgeRoadmapStepDto(x.Title, x.Description, x.Order)).ToList()), cancellationToken);
            }

            foreach (var assessment in bundle.Assessments)
            {
                await SaveAssessment(new SaveAxiForgeAssessmentDto(
                    assessment.Id == Guid.Empty ? null : assessment.Id,
                    assessment.Slug,
                    assessment.Title,
                    assessment.Description,
                    assessment.DurationMinutes,
                    assessment.PassingScore,
                    assessment.IsPublished,
                    assessment.Questions.Select(x => new SaveAxiForgeQuestionDto(x.Prompt, x.OptionA, x.OptionB, x.OptionC, x.OptionD, x.CorrectOption, x.Order)).ToList()), cancellationToken);
            }

            WriteAudit("ImportContent", Guid.Empty);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ImportContent -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ImportContent");
        }
    }

    private static string BuildSlug(string? requestedSlug, string title)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> BuildSlug");
        try
        {
            var source = string.IsNullOrWhiteSpace(requestedSlug) ? title : requestedSlug;
            var cleaned = new string(source
                .Trim()
                .ToLowerInvariant()
                .Select(x => char.IsLetterOrDigit(x) ? x : '-')
                .ToArray());

            while (cleaned.Contains("--", StringComparison.Ordinal))
            {
                cleaned = cleaned.Replace("--", "-", StringComparison.Ordinal);
            }

            return cleaned.Trim('-');
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> BuildSlug -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> BuildSlug");
        }
    }

    private static string NormalizeOption(string option)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> NormalizeOption");
        try
        {
            var normalized = string.IsNullOrWhiteSpace(option)
                ? "A"
                : option.Trim().ToUpperInvariant()[0].ToString();

            return normalized is "A" or "B" or "C" or "D" ? normalized : "A";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> NormalizeOption -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> NormalizeOption");
        }
    }

    private static AxiForgeAdminProblemDto ToProblemDto(CodingProblem problem)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToProblemDto");
        try
        {
            return new AxiForgeAdminProblemDto(
                problem.Id,
                problem.Slug,
                problem.Title,
                problem.Description,
                problem.InputFormat,
                problem.OutputFormat,
                problem.Constraints,
                problem.Examples,
                problem.Explanation,
                problem.Difficulty,
                problem.Topic,
                problem.Tags,
                problem.StarterCode,
                problem.StarterCodeByLanguage,
                problem.ExpectedOutput,
                problem.TimeLimitMilliseconds,
                problem.MemoryLimitKb,
                problem.IsPublished,
                problem.TestCases.OrderBy(x => x.Order).Select(x =>
                    new AxiForgeAdminTestCaseDto(x.Id, x.Input, x.ExpectedOutput, x.IsHidden, x.Order)).ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToProblemDto -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToProblemDto");
        }
    }

    private static AxiForgeAdminRoadmapDto ToRoadmapDto(RoadmapTemplate roadmap)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToRoadmapDto");
        try
        {
            return new AxiForgeAdminRoadmapDto(
                roadmap.Id,
                roadmap.Slug,
                roadmap.Title,
                roadmap.Description,
                roadmap.Level,
                roadmap.IsPublished,
                roadmap.Steps.OrderBy(x => x.Order).Select(x =>
                    new AxiForgeAdminRoadmapStepDto(x.Id, x.Title, x.Description, x.Order)).ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToRoadmapDto -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToRoadmapDto");
        }
    }

    private static AxiForgeAdminAssessmentDto ToAssessmentDto(AssessmentTemplate assessment)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToAssessmentDto");
        try
        {
            return new AxiForgeAdminAssessmentDto(
                assessment.Id,
                assessment.Slug,
                assessment.Title,
                assessment.Description,
                assessment.DurationMinutes,
                assessment.PassingScore,
                assessment.IsPublished,
                assessment.Questions.OrderBy(x => x.Order).Select(x =>
                    new AxiForgeAdminQuestionDto(
                        x.Id,
                        x.Prompt,
                        x.OptionA,
                        x.OptionB,
                        x.OptionC,
                        x.OptionD,
                        x.CorrectOption,
                        x.Order)).ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToAssessmentDto -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> ToAssessmentDto");
        }
    }

    private static void WriteAudit(string action, Guid entityId)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> WriteAudit");
        try
        {
            Console.WriteLine($"AUDIT -> AxiPlus.API -> AxiForgeAdmin -> {action} -> EntityId:{entityId} -> Utc:{DateTime.UtcNow:o}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> WriteAudit -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Controllers -> AxiForgeAdminController.cs -> WriteAudit");
        }
    }
}

public sealed record AxiForgeAdminProblemDto(
    Guid Id,
    string Slug,
    string Title,
    string Description,
    string InputFormat,
    string OutputFormat,
    string Constraints,
    string Examples,
    string Explanation,
    string Difficulty,
    string Topic,
    string Tags,
    string StarterCode,
    string StarterCodeByLanguage,
    string ExpectedOutput,
    int TimeLimitMilliseconds,
    int MemoryLimitKb,
    bool IsPublished,
    List<AxiForgeAdminTestCaseDto> TestCases);

public sealed record SaveAxiForgeProblemDto(
    Guid? Id,
    string? Slug,
    string Title,
    string Description,
    string InputFormat,
    string OutputFormat,
    string Constraints,
    string Examples,
    string Explanation,
    string Difficulty,
    string Topic,
    string Tags,
    string StarterCode,
    string StarterCodeByLanguage,
    string ExpectedOutput,
    int TimeLimitMilliseconds,
    int MemoryLimitKb,
    bool IsPublished,
    List<SaveAxiForgeTestCaseDto> TestCases);

public sealed record AxiForgeAdminTestCaseDto(
    Guid Id,
    string Input,
    string ExpectedOutput,
    bool IsHidden,
    int Order);

public sealed record SaveAxiForgeTestCaseDto(
    string Input,
    string ExpectedOutput,
    bool IsHidden,
    int Order);

public sealed record AxiForgeAdminRoadmapDto(
    Guid Id,
    string Slug,
    string Title,
    string Description,
    string Level,
    bool IsPublished,
    List<AxiForgeAdminRoadmapStepDto> Steps);

public sealed record SaveAxiForgeRoadmapDto(
    Guid? Id,
    string? Slug,
    string Title,
    string Description,
    string Level,
    bool IsPublished,
    List<SaveAxiForgeRoadmapStepDto> Steps);

public sealed record AxiForgeAdminRoadmapStepDto(
    Guid Id,
    string Title,
    string Description,
    int Order);

public sealed record SaveAxiForgeRoadmapStepDto(
    string Title,
    string Description,
    int Order);

public sealed record AxiForgeAdminAssessmentDto(
    Guid Id,
    string Slug,
    string Title,
    string Description,
    int DurationMinutes,
    int PassingScore,
    bool IsPublished,
    List<AxiForgeAdminQuestionDto> Questions);

public sealed record SaveAxiForgeAssessmentDto(
    Guid? Id,
    string? Slug,
    string Title,
    string Description,
    int DurationMinutes,
    int PassingScore,
    bool IsPublished,
    List<SaveAxiForgeQuestionDto> Questions);

public sealed record AxiForgeAdminQuestionDto(
    Guid Id,
    string Prompt,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string CorrectOption,
    int Order);

public sealed record SaveAxiForgeQuestionDto(
    string Prompt,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string CorrectOption,
    int Order);

public sealed record AxiForgeLessonOptionDto(
    Guid Id,
    string Title,
    string ModuleTitle,
    bool IsPublished);

public sealed record AxiForgeLessonPracticeMappingDto(
    Guid Id,
    Guid SourceLessonId,
    string SourceLessonTitle,
    Guid ProblemId,
    string ProblemTitle,
    string SourceProduct);

public sealed record SaveAxiForgeLessonPracticeMappingDto(
    Guid SourceLessonId,
    Guid ProblemId,
    string SourceProduct);

public sealed record AxiForgeAdminImportExportDto(
    List<AxiForgeAdminProblemDto> Problems,
    List<AxiForgeAdminRoadmapDto> Roadmaps,
    List<AxiForgeAdminAssessmentDto> Assessments);
