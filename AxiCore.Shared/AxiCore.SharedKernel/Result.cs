namespace AxiCore.SharedKernel;

/// <summary>
/// Represents the success or failure of an operation without forcing every caller to use exceptions for expected outcomes.
/// Returns a typed result state so APIs, services, and engines can share consistent success and error handling.
/// </summary>
public sealed record Result(bool Succeeded, string? Error)
{
    public static Result Success()
    {
        Console.WriteLine("Entering -> Result.cs -> Success");
        try
        {
            return new Result(true, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> Result.cs -> Success -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> Result.cs -> Success");
        }
    }

    public static Result Failure(string error)
    {
        Console.WriteLine("Entering -> Result.cs -> Failure");
        try
        {
            return new Result(false, error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> Result.cs -> Failure -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> Result.cs -> Failure");
        }
    }
}
