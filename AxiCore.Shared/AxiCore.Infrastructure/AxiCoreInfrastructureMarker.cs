namespace AxiCore.Infrastructure;

/// <summary>
/// Marks the shared infrastructure assembly for dependency scanning and registration.
/// Returns no behavior directly because concrete shared infrastructure services will be added as integrations are implemented.
/// </summary>
public sealed class AxiCoreInfrastructureMarker;
