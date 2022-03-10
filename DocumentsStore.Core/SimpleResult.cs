/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
    
    
/// <summary>
/// The default IResult implementation.
/// </summary>
public static class SimpleResult
{
    /// <summary>
    /// Creates a successful operation result. 
    /// </summary>
    /// <param name="message">An optional message. "Ok" by default.</param>
    /// <returns>An IResult&lt;object&gt; instance.</returns>
    public static IResult Ok(string? message = default)
    {
        return Result<object>.Ok(message);
    }

    /// <summary>
    /// Creates a successful operation result. 
    /// </summary>
    /// <param name="data">Data related to the option result.</param>
    /// <param name="message">An optional message. "Ok" by default.</param>
    /// <returns>An IResult&lt;T&gt; instance.</returns>
    public static IResult Ok(object? data, string? message = default)
    {
        return Result<object>.Ok(data, message);
    }
    
    /// <summary>
    /// Creates a failed operation result. 
    /// </summary>
    /// <param name="message">An optional message. "Error" by default.</param>
    /// <returns>An IResult&lt;object&gt; instance.</returns>
    public static IResult Error(string? message = default)
    {
        return Result<object>.Error(message);
    }

    /// <summary>
    /// Creates a failed operation result. 
    /// </summary>
    /// <param name="data">Data related to the option result.</param>
    /// <param name="message">An optional message. "Error" by default.</param>
    /// <returns>An IResult&lt;object&gt; instance.</returns>
    public static IResult Error(object? data, string? message = default)
    {
        return Result<object>.Error(data, message);
    }
    
    /// <summary>
    /// Creates a failed operation result based on an exception. 
    /// </summary>
    /// <param name="ex">An exception related to the option result.</param>
    /// <param name="message">An optional message. Exception.Message by default.</param>
    /// <returns>An IResult&lt;Exception&gt; instance.</returns>
    public static IResult Error(Exception ex, string? message = default)
    {
        return Result<object>.Error(ex, message);
    }

    /// <summary>
    /// Returns an IReturn instance based on a boolean state.
    /// </summary>
    /// <param name="state">A state.</param>
    /// <param name="message">An optional message.</param>
    /// <returns>An IResult instance.</returns>
    public static IResult FromBoolean(bool state, string? message = default)
    {
        return state
            ? Ok(message)
            : Error(message);
    }
}
