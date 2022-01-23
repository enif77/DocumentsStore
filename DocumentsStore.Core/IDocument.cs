namespace DocumentsStore.Core;

/// <summary>
/// Defines a document.
/// </summary>
public interface IDocument
{
    /// <summary>
    /// A name of this document.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// A content of this document.
    /// </summary>
    byte[] Content { get; }
}
