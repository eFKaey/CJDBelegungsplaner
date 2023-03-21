namespace CJDBelegungsplaner.Domain.Results;

public abstract class Result
{
    public ResultKind? Kind { get; }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Matter { get; }

    public bool IsMatter => Matter is not null;

    internal Result()
    {}

    internal Result(bool success, ResultKind kind)
    {
        IsSuccess = success;
        Kind = kind;
    }

    internal Result(bool success, ResultKind kind, string matter)
    {
        IsSuccess = success;
        Kind = kind;
        Matter = matter;
    }
}