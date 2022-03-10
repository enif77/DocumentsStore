/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

public interface IResult
{
    bool IsSuccess { get; }
    string Message { get; }
}


public interface IResult<out T> : IResult
{
    T? Data { get; }
}
