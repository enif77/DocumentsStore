/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System.Collections.Generic;


/// <summary>
/// Defines a documents store.
/// </summary>
public interface IDocumentsStore
{
    /// <summary>
    /// Gets a name of this documents store.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the number of documents in this documents store.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns true, if this documents store is opened for operations.
    /// </summary>
    bool IsOpened { get; }

    /// <summary>
    /// Gets all documents in this documents store.
    /// </summary>
    IEnumerable<IDocument> Documents { get; }


    /// <summary>
    /// Sets up this documents store for further operations.
    /// Can be called multiple times and must be called before any other operation.
    /// </summary>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    IResult Open();

    /// <summary>
    /// Cleans up the internal state of this documents store and closes it for any future use.
    /// This document store can be reopened by calling the Open() method.
    /// </summary>
    void Close();

    /// <summary>
    /// Checks, if this documents store has a document with a certain name.
    /// </summary>
    /// <param name="documentName">A name of a document. Null or empty name is never found.</param>
    /// <returns>True, if this documents store has a document with a certain name.</returns>
    bool HasDocument(string documentName);

    /// <summary>
    /// Loads and returns a document from this documents store, if it exists.
    /// </summary>
    /// <param name="documentName">A name of a document.</param>
    /// <returns>A successful IResult instance with the document or a failed IResult instance with error description.</returns>
    IResult<IDocument> Load(string documentName);
    
    /// <summary>
    /// Saves a document into this store.
    /// Existing document is replaced with the new one. 
    /// </summary>
    /// <param name="document">An IDocument instance.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    IResult Save(IDocument document);

    /// <summary>
    /// Renames a document in this documents store.
    /// Internally it creates a new document with the new name and the content of the old document.
    /// The old document is then removed from this documents store.
    /// </summary>
    /// <param name="documentName">A name of an existing document.</param>
    /// <param name="newDocumentName">A new name for the existing document.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    IResult Rename(string documentName, string newDocumentName);

    /// <summary>
    /// Deletes a document from this documents store.
    /// </summary>
    /// <param name="documentName">A name of a document.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    IResult Delete(string documentName);
}
