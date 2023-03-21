namespace CJDBelegungsplaner.Domain.Results;

public class Result<TResultKind, TContent> : Result<TResultKind>
    where TResultKind : ResultKind
{
    public TContent? Content { get; }

    internal Result()
    { }

    internal Result(bool success, TResultKind kind)
        : base(success, kind) { }

    internal Result(bool success, TResultKind kind, string matter)
        : base(success, kind, matter) { }

    internal Result(bool success, TResultKind kind, TContent content)
        : base(success, kind)
    {
        Content = content;
    }

    internal Result(bool success, TResultKind kind, string matter, TContent content)
        : base(success, kind, matter)
    {
        Content = content;
    }

    protected internal override Result<TResultKind, TContent> Success(int kind)
    {
        return new Result<TResultKind, TContent>(true, GetResultKind(kind));
    }

    protected internal Result<TResultKind, TContent> Success(int kind, TContent content)
    {
        return new Result<TResultKind, TContent>(true, GetResultKind(kind), content);
    }

    protected internal override Result<TResultKind, TContent> Failure(int kind)
    {
        return new Result<TResultKind, TContent>(false, GetResultKind(kind));
    }

    protected internal override Result<TResultKind, TContent> Failure(int kind, string matter)
    {
        return new Result<TResultKind, TContent>(false, GetResultKind(kind), matter);
    }

    protected internal Result<TResultKind, TContent> Failure(int kind, TContent content)
    {
        return new Result<TResultKind, TContent>(false, GetResultKind(kind), content);
    }

    protected internal Result<TResultKind, TContent> Failure(int kind, string matter, TContent content)
    {
        return new Result<TResultKind, TContent>(false, GetResultKind(kind), matter, content);
    }

    protected internal override Result<TResultKind, TContent> PassOn<TResultKindParameter>(Result<TResultKindParameter> result)
    {
        if (result is null)
        {
            throw new ArgumentNullException("result");
        }

        return new Result<TResultKind, TContent>(result.IsSuccess, GetResultKind(result.Kind), result.Matter);
    }

    protected internal Result<TResultKind, TContent> PassOn<TResultKindParameter, TContentParameter>(Result<TResultKindParameter, TContentParameter> result)
        where TResultKindParameter : ResultKind
        where TContentParameter : TContent
    {
        if (result is null)
        {
            throw new ArgumentNullException("result");
        }

        return new Result<TResultKind, TContent>(result.IsSuccess, GetResultKind(result.Kind), result.Matter, result.Content);
    }
}