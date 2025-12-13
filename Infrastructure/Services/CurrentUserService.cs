using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Http;

namespace AdmissionMinaret.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Task<Guid> GetUserAsync()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
        }

        return Task.FromResult(userId);
    }

    public string? GetUserEmail()
    {
        return httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public IEnumerable<string> GetUserRoles()
    {
        return httpContextAccessor.HttpContext?.User
            ?.FindAll(ClaimTypes.Role)
            ?.Select(c => c.Value)
            ?? [];
    }

    public bool IsInRole(string role)
    {
        return httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}