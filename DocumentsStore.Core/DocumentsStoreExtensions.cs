/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
using System.IO;
  

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
    
    /// <summary>
    /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.
    /// A path to the temporary file is in the Data property of the returned IResult instance.</returns>
    public static IResult<string> GetTempFileName(this IDocumentsStore store)
    {
        try
        {
            var path = Path.GetTempFileName();
        
            return Result<string>.Ok(path, $"The temporary file '{path}' created.");
        }
        catch (Exception ex)
        {
            return Result<string>.Error($"A temporary file cannot be created. {ex.Message}");
        }
        
    }
    
    /// <summary>
    /// Deletes a temporary file.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="tempFilePath">A temporary file path.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult DeleteTempFile(this IDocumentsStore store, string tempFilePath)
    {
        if (string.IsNullOrWhiteSpace(tempFilePath)) return SimpleResult.Error("A temp file path expected.");

        if (File.Exists(tempFilePath) == false)
        {
            return SimpleResult.Ok($"The '{tempFilePath}' file not found.");    
        }
        
        try
        {
            File.Delete(tempFilePath);

            return SimpleResult.Ok($"The '{tempFilePath}' file deleted.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"The '{tempFilePath}' file cannot be deleted. {ex.Message}");
        }
    }

    /// <summary>
    /// Imports a document into a documents store.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="path">A path to a file.</param>
    /// <param name="documentName">A document name under which a file will be imported.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult ImportDocument(this IDocumentsStore store, string path, string documentName)
    {
        if (string.IsNullOrWhiteSpace(path)) return SimpleResult.Error("A path to a document expected.");
        if (string.IsNullOrWhiteSpace(documentName)) return SimpleResult.Error("A document name expected.");

        if (File.Exists(path) == false)
        {
            return SimpleResult.Error($"The '{path}' document not found.");
        }

        try
        {
            return store.Save(documentName, File.ReadAllBytes(path));
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"The '{path}' document import failed. {ex.Message}");
        }
    }

    /// <summary>
    /// Exports a document to a file.
    /// </summary>
    /// <param name="store">A documents store.</param>
    /// <param name="documentName">A document name.</param>
    /// <param name="path">A path to a file.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public static IResult ExportDocument(this IDocumentsStore store, string documentName, string path)
    {
        if (string.IsNullOrWhiteSpace(documentName)) return SimpleResult.Error("A document name expected.");
        if (string.IsNullOrWhiteSpace(path)) return SimpleResult.Error("A path to a document expected.");

        var documentLoadResult = store.Load(documentName);
        if (documentLoadResult.IsSuccess == false)
        {
            return documentLoadResult;
        }

        try
        {
            File.WriteAllBytes(path, documentLoadResult.Data!.Content);

            return SimpleResult.Ok($"The '{documentName}' document successfully exported to the '{path}' file.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"The '{documentName}' document export failed. {ex.Message}");
        }
    }
}
