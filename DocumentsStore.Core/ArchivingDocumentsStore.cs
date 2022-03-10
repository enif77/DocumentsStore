/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
using System.Collections.Generic;


/// <summary>
/// Documents store, that archives documents instead of deleting them.
/// </summary>
public class ArchivingDocumentsStore : IDocumentsStore
{
    private readonly IDocumentsStore _master;
    private readonly IDocumentsStore _archive;

    
    /// <summary>
    /// Returns the name of the master documents store.
    /// </summary>
    public string Name => _master.Name;

    /// <summary>
    /// Returns the number of documents in the master documents store.
    /// </summary>
    public int Count => _master.Count;

    /// <summary>
    /// Returns true, if both the master and the archive documents stores are opened.
    /// </summary>
    public bool IsOpened => _master.IsOpened && _archive.IsOpened;

    /// <summary>
    /// Returns the list of documents in the master documents store.
    /// </summary>
    public IEnumerable<IDocument> Documents => _master.Documents;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="master">The main documents store.</param>
    /// <param name="archive">The archive documents store.</param>
    /// <exception cref="ArgumentException">Thrown, if the name argument is null or empty.</exception>
    public ArchivingDocumentsStore(IDocumentsStore master, IDocumentsStore archive)
    { 
        _master = master ?? throw new ArgumentNullException(nameof(master));
        _archive = archive ?? throw new ArgumentNullException(nameof(archive));
        
        _master.Close();
        _archive.Close();
    }

    /// <summary>
    /// Opens both the master and the archive documents stores.
    /// </summary>
    /// <returns></returns>
    public IResult Open()
    {
        var masterOpenResult = _master.Open();
        if (masterOpenResult.IsSuccess == false)
        {
            return masterOpenResult;
        }

        var archiveOpenResult = _archive.Open();
        if (archiveOpenResult.IsSuccess == false)
        {
            _master.Close();

            return archiveOpenResult;
        }

        return SimpleResult.Ok($"The '{Name}' archiving documents store, based on the '{_master.Name}' and the '{_archive.Name}' documents stores, opened successfully.");
    }

    /// <summary>
    /// Closes both the master and the archive documents stores. 
    /// </summary>
    public void Close()
    {
        _master.Close();
        _archive.Close();
    }

    /// <summary>
    /// Returns true, if the master documents store contains a document with a certain document name.
    /// </summary>
    /// <param name="documentName">A document name.</param>
    /// <returns>True, if the master documents store contains a document with a certain document name.</returns>
    public bool HasDocument(string documentName)
    {
        return _master.HasDocument(documentName);
    }

    /// <summary>
    /// Returns a document from the master documents store.  
    /// </summary>
    /// <param name="documentName">A document name.</param>
    /// <returns>A successful IResult instance with the document or a failed IResult instance with error description.</returns>
    public IResult<IDocument> Load(string documentName)
    {
        return _master.Load(documentName);
    }

    /// <summary>
    /// Saves a document into the master documents store.
    /// Existing document is replaced with the new one. 
    /// </summary>
    /// <param name="document">An IDocument instance.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public IResult Save(IDocument document)
    {
        return _master.Save(document);
    }

    /// <summary>
    /// Renames a document in the master documents store.
    /// </summary>
    /// <param name="documentName">A name of an existing document.</param>
    /// <param name="newDocumentName">A new name for the existing document.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public IResult Rename(string documentName, string newDocumentName)
    {
        return _master.Rename(documentName, newDocumentName);
    }

    /// <summary>
    /// Archives a document into the archive documents store and deletes it from the master documents store.
    /// </summary>
    /// <param name="documentName">A document name.</param>
    /// <returns>An IResult instance signaling, whether this operation succeeded or not.</returns>
    public IResult Delete(string documentName)
    {
        return _master.Archive(documentName, _archive);
    }
}
