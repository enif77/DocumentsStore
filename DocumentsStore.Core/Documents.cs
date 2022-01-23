namespace DocumentsStore.Core;

/// <summary>
/// A document.
/// </summary>
public class Document : IDocument
{
    #region IDocument

    public string Name { get; }
    public byte[] Content { get; }

    #endregion


    #region ctor

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A name of this document.</param>
    /// <param name="content">The content of this document.</param>
    /// <exception cref="ArgumentException">If the name is null or empty.</exception>
    /// <exception cref="ArgumentNullException">If the content is null.</exception>
    public Document(string name, byte[] content)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("A document name expected.", nameof(name));

        Name = name;
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }


    public static IDocument Empty => new Document("empty", Array.Empty<byte>());

    #endregion
}
