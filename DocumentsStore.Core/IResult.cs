/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

/// <summary>
/// Basic result interface.
/// </summary>
public interface IResult
{
    /// <summary>
    /// True, if an operation succeeded.
    /// </summary>
    bool IsSuccess { get; }
        
    /// <summary>
    /// An optional string representation of the operational result. 
    /// </summary>
    string Message { get; }
}


/// <summary>
/// An operation result with data.
/// </summary>
/// <typeparam name="T">A type of the optional data related to this operation result.</typeparam>
public interface IResult<out T> : IResult
{
    T? Data { get; }
}
