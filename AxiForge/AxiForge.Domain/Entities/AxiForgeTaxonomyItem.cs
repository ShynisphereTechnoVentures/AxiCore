namespace AxiForge.Domain.Entities;

public sealed class AxiForgeTaxonomyItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Type { get; set; } = "Class";

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
