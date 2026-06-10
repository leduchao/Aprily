using System.ComponentModel.DataAnnotations;

namespace Aprily.Backend.Common.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32)]
    public required string SecretKey { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Range(1, 120)]
    public int ExpirationInMinutes { get; init; }
}