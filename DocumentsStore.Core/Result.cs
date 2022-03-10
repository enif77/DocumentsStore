/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public T? Data { get; }


    private Result(bool isSuccess, T? data, string message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }


    public static IResult<T> Ok(string? message = null)
    {
        return new Result<T>(true, default, message ?? "Ok");
    }


    public static IResult<T> Ok(T? data, string? message = null)
    {
        return new Result<T>(true, data ?? default, message ?? "Ok");
    }


    public static IResult<T> Error(string? message = null)
    {
        return new Result<T>(false, default, message ?? "Ok");
    }


    public static IResult<T> Error(T? data, string? message = null)
    {
        return new Result<T>(false, data ?? default, message ?? "Ok");
    }
}