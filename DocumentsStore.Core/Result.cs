/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
    
    
/// <summary>
/// The default IResult implementation.
/// </summary>
/// <typeparam name="T">A type of the data related to an operation result.</typeparam>
public class Result<T> : IResult<T>
{
    private const string OkMessage = "Ok";
    private const string ErrorMessage = "Error";
    
    public bool IsSuccess { get; }
    public string Message { get; }
    public T? Data { get; }

   
    private Result(bool isSuccess, T? data = default, string message = "")
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }
    

    /// <summary>
    /// Creates a successful operation result. 
    /// </summary>
    /// <param name="message">An optional message. "Ok" by default.</param>
    /// <returns>An IResult&lt;T&gt; instance.</returns>
    public static IResult<T> Ok(string? message = default)
    {
        return new Result<T>(true, default, message ?? OkMessage);
    }

    /// <summary>
    /// Creates a successful operation result. 
    /// </summary>
    /// <param name="data">Data related to the option result.</param>
    /// <param name="message">An optional message. "Ok" by default.</param>
    /// <returns>An IResult&lt;T&gt; instance.</returns>
    public static IResult<T> Ok(T? data, string? message = default)
    {
        return new Result<T>(true, data, message ?? OkMessage);
    }
    
    /// <summary>
    /// Creates a failed operation result. 
    /// </summary>
    /// <param name="message">An optional message. "Error" by default.</param>
    /// <returns>An IResult&lt;T&gt; instance.</returns>
    public static IResult<T> Error(string? message = default)
    {
        return new Result<T>(false, default, message ?? ErrorMessage);
    }

    /// <summary>
    /// Creates a failed operation result. 
    /// </summary>
    /// <param name="data">Data related to the option result.</param>
    /// <param name="message">An optional message. "Error" by default.</param>
    /// <returns>An IResult&lt;T&gt; instance.</returns>
    public static IResult<T> Error(T? data, string? message = default)
    {
        return new Result<T>(false, data, message ?? ErrorMessage);
    }
    
    /// <summary>
    /// Creates a failed operation result based on an exception. 
    /// </summary>
    /// <param name="ex">An exception related to the option result.</param>
    /// <param name="message">An optional message. Exception.Message by default.</param>
    /// <returns>An IResult&lt;Exception&gt; instance.</returns>
    public static IResult<Exception> Error(Exception ex, string? message = default)
    {
        return new Result<Exception>(false, ex, message ?? ex.Message);
    }
}
