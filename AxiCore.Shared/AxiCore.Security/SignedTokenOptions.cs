namespace AxiCore.Security;

/// <summary>
/// Stores shared signing settings for cross-product launch tokens.
/// Returns configuration values used by token signing and validation services.
/// </summary>
public sealed class SignedTokenOptions
{
    public string Issuer{ get; set; } = "AxiCore";

    public string Audience{ get; set; } = "AxiCore.Products";

    public string SigningKey{ get; set; } = string.Empty;
}
