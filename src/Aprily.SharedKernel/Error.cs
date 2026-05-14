namespace Aprily.SharedKernel;

public class Error(string code, string message)
{
    public string Code { get; set; } = code;
    public string Message { get; set; } = message;

    public static Error None => new(string.Empty, string.Empty);
}
