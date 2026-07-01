using AxiCore.Identity;
using AxiCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.API.Services;

public sealed class AxiCoreAccountProvisioningService
{
    private readonly AxiCoreDbContext _context;

    public AxiCoreAccountProvisioningService(AxiCoreDbContext context)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> AxiCoreAccountProvisioningService");
        try
        {
            _context = context;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> AxiCoreAccountProvisioningService -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> AxiCoreAccountProvisioningService");
        }
    }

    /// <summary>
    /// Creates or updates a shared AxiCore student user and grants selected product access.
    /// Returns the shared AxiCore user identifier for cross-product account linking.
    /// </summary>
    public async Task<Guid> ProvisionStudentAsync(
        string fullName,
        string email,
        string passwordHash,
        IEnumerable<string> productCodes,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> ProvisionStudentAsync");
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);

            if (user == null)
            {
                user = new AxiCoreUser
                {
                    FullName = fullName.Trim(),
                    Email = normalizedEmail,
                    PasswordHash = passwordHash,
                    IsActive = true,
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
            }
            else
            {
                user.FullName = string.IsNullOrWhiteSpace(user.FullName) ? fullName.Trim() : user.FullName;
                user.PasswordHash = string.IsNullOrWhiteSpace(user.PasswordHash) ? passwordHash : user.PasswordHash;
                user.IsActive = true;
            }

            await EnsureRoleAsync(user, AxiCoreRoleNames.Student, cancellationToken);

            foreach (var productCode in productCodes.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                EnsureProductAccess(user.Id, productCode);
            }

            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"AxiCore email confirmation pending for {normalizedEmail} covering {string.Join(",", productCodes)}.");
            return user.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> ProvisionStudentAsync -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> ProvisionStudentAsync");
        }
    }

    /// <summary>
    /// Ensures the shared AxiCore student role exists and is assigned to the user.
    /// Returns no value because role assignment is persisted through the shared context.
    /// </summary>
    private async Task EnsureRoleAsync(
        AxiCoreUser user,
        string roleName,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureRoleAsync");
        try
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

            if (role == null)
            {
                role = new AxiCoreRole { Name = roleName };
                _context.Roles.Add(role);
            }

            var alreadyAssigned = await _context.UserRoles
                .AnyAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);

            if (!alreadyAssigned)
            {
                _context.UserRoles.Add(new AxiCoreUserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureRoleAsync -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureRoleAsync");
        }
    }

    /// <summary>
    /// Grants product access when a matching AxiCore access row does not already exist.
    /// Returns no value because the access row is tracked by the shared context.
    /// </summary>
    private void EnsureProductAccess(Guid userId, string productCode)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureProductAccess");
        try
        {
            if (_context.UserProductAccess.Local.Any(x =>
                    x.UserId == userId &&
                    x.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var exists = _context.UserProductAccess.Any(x =>
                x.UserId == userId &&
                x.ProductCode == productCode);

            if (exists)
            {
                return;
            }

            _context.UserProductAccess.Add(new AxiCoreProductAccess
            {
                UserId = userId,
                ProductCode = productCode,
                Status = AxiCoreProductAccessStatuses.Active,
                GrantedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureProductAccess -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Services -> AxiCoreAccountProvisioningService.cs -> EnsureProductAccess");
        }
    }
}
