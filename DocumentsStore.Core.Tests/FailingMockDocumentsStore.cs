/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core.Tests;

using System;
using System.Collections.Generic;

using DocumentsStore.Core;


public class FailingMockDocumentsStore : IDocumentsStore
{
    public string Name { get; }
    public int Count => 0;
    public bool IsOpened { get; }

    public IEnumerable<IDocument> Documents => new List<IDocument>();


    public FailingMockDocumentsStore(string name, bool isOpened)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("A documents store name expected.", nameof(name));
        
        Name = name;
        IsOpened = isOpened;
    }


    public IResult Open()
    {
        return SimpleResult.Error($"The '{Name}' documents store cannot be opened.");
    }

    
    public void Close()
    {
    }


    public bool HasDocument(string documentName) => false;

    
    public IResult<IDocument> Load(string documentName)
    {
        return Result<IDocument>.Error(string.IsNullOrEmpty(documentName)
            ? "A document name expected."
            : $"A document with name '{documentName}' not found.");
    }

    
    public IResult Save(IDocument document)
    {
        return SimpleResult.Error(document == null
            ? "A document expected."
            : $"The '{document.Name}' document cannot be saved into the '{Name}' documents store.");
    }

    
    public IResult Rename(string documentName, string newDocumentName)
    {
        if (string.IsNullOrEmpty(documentName)) return SimpleResult.Error("A document name expected.");
        if (string.IsNullOrEmpty(newDocumentName)) return SimpleResult.Error("A new document name expected.");
        
        return SimpleResult.Error($"The '{documentName}' document cannot be renamed in the '{Name}' documents store.");
    }

    
    public IResult Delete(string documentName)
    {
        return SimpleResult.Error(string.IsNullOrEmpty(documentName)
            ? "A document name expected."
            : $"The '{documentName}' document cannot be deleted from the '{Name}' documents store.");
    }
}
