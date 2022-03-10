/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
using System.Collections.Generic;


/// <summary>
/// Documents store, that stores documents in memory.
/// </summary>
public class InMemoryDocumentsStore : IDocumentsStore
{
    public string Name { get; }

    public int Count =>
        IsOpened
            ? _documents.Count
            : 0;

    public bool IsOpened { get; private set; }

    public IEnumerable<IDocument> Documents =>
        IsOpened
            ? _documents.Values
            : new List<IDocument>();


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A name of this documents store.</param>
    /// <exception cref="ArgumentException">Thrown, if the name argument is null or empty.</exception>
    public InMemoryDocumentsStore(string name)
    { 
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("A documents store name expected.", nameof(name));

        Name = name;
        IsOpened = false;
        
        _documents = new Dictionary<string, IDocument>();
    }


    public IResult Open()
    {
        IsOpened = true;

        return SimpleResult.Ok($"The '{Name}' documents store opened successfully.");
    }


    public void Close()
    {
        IsOpened = false;
    }


    public bool HasDocument(string documentName)
    {
        return IsOpened && string.IsNullOrEmpty(documentName) == false && _documents.ContainsKey(documentName);
    }


    public IResult<IDocument> Load(string documentName)
    { 
        if (string.IsNullOrEmpty(documentName)) return Result<IDocument>.Error("A document name expected.");

        if (IsOpened == false) return Result<IDocument>.Error($"Documents store '{Name}' is not opened.");

        return _documents.ContainsKey(documentName)
            ? Result<IDocument>.Ok(_documents[documentName])
            : Result<IDocument>.Error($"A document with name '{documentName}' not found.");
    }


    public IResult Save(IDocument document)
    { 
        if (document == null) return SimpleResult.Error("A document expected.");

        if (IsOpened == false) return SimpleResult.Error($"The '{Name}' documents store is not opened.");

        if (_documents.ContainsKey(document.Name))
        {
            _documents.Remove(document.Name);
        }

        _documents.Add(document.Name, document);

        return SimpleResult.Ok($"A document with name '{document.Name}' successfully saved to the '{Name}' documents store.");
    }


    public IResult Rename(string documentName, string newDocumentName)
    { 
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");
        if (string.IsNullOrEmpty(newDocumentName)) return SimpleResult.Error("A new document name expected.");

        if (IsOpened == false) return SimpleResult.Error($"Documents store '{Name}' is not opened.");

        if (_documents.ContainsKey(documentName) == false)
        {
            return SimpleResult.Error($"A document with name '{documentName}' not found.");
        }

        if (_documents.ContainsKey(newDocumentName))
        {
            return SimpleResult.Error($"A document with name '{newDocumentName}' already exists.");
        }

        var document = _documents[documentName];
        _documents.Add(newDocumentName, new Document(newDocumentName, document.Content));
        
        if (_documents.Remove(documentName))
        {
            return SimpleResult.Ok($"A document with name '{documentName}' renamed to '{newDocumentName}' successfully.");
        }

        // Return this documents store to its original state. This can fail.
        _ = _documents.Remove(newDocumentName);
        
        return SimpleResult.Error($"A document with name '{documentName}' cannot be removed from the '{Name}' documents store.");
    }


    public IResult Delete(string documentName)
    {
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");

        if (IsOpened == false) return SimpleResult.Error($"Documents store '{Name}' is not opened.");

        return _documents.Remove(documentName)
            ? SimpleResult.Ok($"A document with name '{documentName}' successfully deleted.")
            : SimpleResult.Error($"A document with name '{documentName}' not found or cannot be deleted.");
    }

    
    private readonly IDictionary<string, IDocument> _documents;
}
