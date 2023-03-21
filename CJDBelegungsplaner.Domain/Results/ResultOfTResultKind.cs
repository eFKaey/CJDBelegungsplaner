namespace CJDBelegungsplaner.Domain.Results;

public class Result<TResultKind> : Result
    where TResultKind : ResultKind
{

    internal Result()
    { }

    internal Result(bool success, TResultKind kind)
        : base(success, kind)
    {}

    internal Result(bool success, TResultKind kind, string matter)
        : base(success, kind, matter)
    {}

    protected internal virtual Result<TResultKind> Success(int kind)
    {
        return new Result<TResultKind>(true, GetResultKind(kind));
    }

    protected internal virtual Result<TResultKind> Failure(int kind)
    {
        return new Result<TResultKind>(false, GetResultKind(kind));
    }

    protected internal virtual Result<TResultKind> Failure(int kind, string matter)
    {
        return new Result<TResultKind>(false, GetResultKind(kind), matter);
    }

    protected internal virtual Result<TResultKind> PassOn<TResultKindParameter>(Result<TResultKindParameter> result)
        where TResultKindParameter : ResultKind
    {
        if (result is null)
        {
            throw new ArgumentNullException("result");
        }

        return new Result<TResultKind>(result.IsSuccess, GetResultKind(result.Kind), result.Matter);
    }

    protected TResultKind GetResultKind(int parameter)
    {
        return (TResultKind)Activator.CreateInstance(typeof(TResultKind), new object[] { parameter })!;
    }
}