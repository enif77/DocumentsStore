namespace DocumentsStore.Core;

/// <summary>
/// Documents store helper methods.
/// </summary>
public static class DocumentsStoreExtensions
{
    /// <summary>
    /// Loads a document as a array of bytes.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <returns>Document content or null.</returns>
    public static byte[]? LoadBytes(this IDocumentsStore store, string documentName)
    {
        var result = store.Load(documentName);
        
        return result.IsSuccess
            ? result.Data!.Content  // Data should never be null! 
            : null;
    }

    /// <summary>
    /// Loads a document as a string.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <param name="encoding">An optional character encoding. UTF8 by default.</param>
    /// <returns>A string or null.</returns>
    public static string? LoadString(this IDocumentsStore store, string documentName, System.Text.Encoding? encoding = null)
    {
        var bytes = store.LoadBytes(documentName);
        if (bytes == null)
        {
            return null;
        }

        return (encoding == null)
            ? System.Text.Encoding.UTF8.GetString(bytes)
            : encoding.GetString(bytes);
    }

    /// <summary>
    /// Saves a string as UTF8 bytes into a documents store under a certain name.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <param name="content">A document content.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult Save(this IDocumentsStore store, string documentName, string content)
    {
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");
        if (content == null) return SimpleResult.Error("A document content expected.");

        return store.Save(new Document(documentName, System.Text.Encoding.UTF8.GetBytes(content)));
    }

    /// <summary>
    /// Saves an array of bytes into a documents store under a certain name.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <param name="content">A document content.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult Save(this IDocumentsStore store, string documentName, byte[] content)
    {
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");
        if (content == null) return SimpleResult.Error("A document content expected.");

        return store.Save(new Document(documentName, content));
    }

    /// <summary>
    /// Archives a document into a defined store.
    /// A document is saved to a target documents store and removed from this documents store.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <param name="archive">An archiving document store.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult Archive(this IDocumentsStore store, string documentName, IDocumentsStore archive)
    {
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");
        if (archive == null) return SimpleResult.Error("An archive documents store expected.");

        var loadDocumentResult = store.Load(documentName);
        if (loadDocumentResult.IsSuccess == false)
        {
            return loadDocumentResult;
        }

        var saveDocumentResult = archive.Save(loadDocumentResult.Data!);  // Data should never be null!
        if (saveDocumentResult.IsSuccess == false)
        {
            return saveDocumentResult;
        }

        var deleteDocumentResult = store.Delete(documentName);
        if (deleteDocumentResult.IsSuccess == false)
        {
            // If we cannot delete the document from this documents store,
            // we try to get the archive documents store to its original state 
            // by deleting the document saved to it.
            // If this fails, we ignore that.
            _ = archive.Delete(documentName);

            return deleteDocumentResult;
        }

        return SimpleResult.Ok($"The '{documentName}' document successfully archived to the '{archive.Name}' documents store.");
    }
}
