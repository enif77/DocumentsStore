/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


/// <summary>
/// Documents store, that stores documents on disk as files.
/// </summary>
public class OnDiskDocumentsStore : IDocumentsStore
{
    public string Name { get; }

    public int Count =>
        IsOpened
            ? GetDocumentsCount()
            : 0;

    /// <summary>
    /// The location of this file store.
    /// </summary>
    public string Location { get; }

    public bool IsOpened { get; private set; }

    /// <summary>
    /// Returns all documents from this documents store.
    /// Does not load all documents, it just returns an enumerator.
    /// Can throw System.IO related exceptions. 
    /// </summary>
    public IEnumerable<IDocument> Documents
    {
        get
        {
            string path;
            if (IsOpened && Directory.Exists(path = GetDocumentsStoreDirectoryFullPath()))
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    yield return new Document(Path.GetFileName(file), File.ReadAllBytes(file));
                }
            }
            
            foreach (var document in new List<IDocument>())
            {
                yield return document;
            }
        }
    }
    
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">A name of this documents store.</param>
    /// <param name="location">A path to a directory this documents store is located.</param>
    /// <exception cref="ArgumentException">Thrown, if the name or the location arguments are null or empty.</exception>
    public OnDiskDocumentsStore(string name, string location)
    { 
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("A documents store name expected.", nameof(name));
        if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("A documents store location expected.", nameof(location));

        Name = name;
        Location = location;
        IsOpened = false;

        CheckName();
        CheckLocation();
    }

    /// <summary>
    /// Initializes and opens this documents store.
    /// For a new/non existing documents store creates it's directory.
    /// For an existing documents store does nothing.
    /// The Name and the Location of the file store has to be set.
    /// </summary>
    public IResult Open()
    {
        if (IsOpened) return SimpleResult.Ok($"The '{Name}' documents store is already opened.");

        var storeFullPath = GetDocumentsStoreDirectoryFullPath();
        if (Directory.Exists(storeFullPath))
        {
            IsOpened = true;

            return SimpleResult.Ok($"The documents store directory '{storeFullPath}' found and will be used.");
        }

        try
        {
            var directory = Directory.CreateDirectory(storeFullPath ?? throw new InvalidOperationException());

            IsOpened = true;
        
            return SimpleResult.Ok($"A new '{storeFullPath}' working directory for the '{Name}' documents store created successfully at {directory.CreationTime}.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"The '{Name}' documents store cannot be opened with an exception '{ex.Message}'.");
        }
    }


    public void Close()
    {
        IsOpened = false;
    }


    public bool HasDocument(string documentName)
    {
        return
            IsOpened &&
            IsValidDocumentName(documentName).IsSuccess &&
            File.Exists(GetDocumentFileFullPath(documentName));
    }


    public IResult<IDocument> Load(string documentName)
    { 
        var isValidDocumentNameResult = IsValidDocumentName(documentName);
        if (isValidDocumentNameResult.IsSuccess == false)
        {
            return Result<IDocument>.Error(isValidDocumentNameResult.Message);
        }

        if (IsOpened == false) return Result<IDocument>.Error($"Documents store '{Name}' is not opened.");

        var documentFullPath = GetDocumentFileFullPath(documentName);

        try
        {
            return File.Exists(documentFullPath)
                ? Result<IDocument>.Ok(
                    new Document(
                        Path.GetFileName(documentFullPath),
                        File.ReadAllBytes(documentFullPath)),
                    $"A document with name '{documentName}' loaded from the '{documentFullPath}' file.")
                : Result<IDocument>.Error($"A document with name '{documentName}' not found.");
        }
        catch (Exception ex)
        {
            return Result<IDocument>.Error($"A document with the '{documentName}' name cannot be loaded with an exception '{ex.Message}'.");
        }
    }


    public IResult Save(IDocument document)
    { 
        if (document == null) return SimpleResult.Error("A document expected.");

        if (IsOpened == false) return SimpleResult.Error($"The '{Name}' documents store is not opened.");

        var documentFullPath = GetDocumentFileFullPath(document.Name);

        try
        {
            if (File.Exists(documentFullPath))
            {
                File.Delete(documentFullPath);
            }

            File.WriteAllBytes(documentFullPath ?? throw new InvalidOperationException("The documentFullPath was null."), document.Content);

            return SimpleResult.Ok($"A document with the '{document.Name}' name successfully saved into the '{documentFullPath}' file in the '{Name}' documents store.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"A document with the '{document.Name}' name cannot be saved with an exception '{ex.Message}'.");
        }
    }


    public IResult Rename(string documentName, string newDocumentName)
    { 
        var isValidDocumentNameResult = IsValidDocumentName(documentName);
        if (isValidDocumentNameResult.IsSuccess == false)
        {
            return isValidDocumentNameResult;
        }
        
        var isValidNewDocumentNameResult = IsValidDocumentName(newDocumentName);
        if (isValidNewDocumentNameResult.IsSuccess == false)
        {
            return isValidNewDocumentNameResult;
        }

        if (IsOpened == false) return SimpleResult.Error($"The '{Name}' documents store is not opened.");

        var documentFullPath = GetDocumentFileFullPath(documentName);
        
        if (File.Exists(documentFullPath) == false)
        {
            return SimpleResult.Error($"A document with the '{documentName}' name not found or cannot be accessed.");
        }

        var newDocumentFullPath = GetDocumentFileFullPath(newDocumentName);
        
        if (File.Exists(newDocumentFullPath))
        {
            return SimpleResult.Error($"A document with the '{newDocumentName}' name already exists.");
        }

        try
        {
            File.Move(documentFullPath, newDocumentFullPath ?? throw new InvalidOperationException("The newDocumentFullPath was null."));
        
            return SimpleResult.Ok($"A document with name '{documentName}' renamed to '{newDocumentName}' successfully.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"A document with the '{documentName}' name cannot be renamed with an exception '{ex.Message}'.");
        }
    }


    public IResult Delete(string documentName)
    {
        var isValidDocumentNameResult = IsValidDocumentName(documentName);
        if (isValidDocumentNameResult.IsSuccess == false)
        {
            return isValidDocumentNameResult;
        }

        if (IsOpened == false) return SimpleResult.Error($"The '{Name}' documents store is not opened.");

        var documentFullPath = GetDocumentFileFullPath(documentName);
        
        if (File.Exists(documentFullPath) == false)
        {
            return SimpleResult.Error($"A document with the '{documentName}' name not found or cannot be accessed.");
        }

        try
        {
            File.Delete(documentFullPath);
            
            return SimpleResult.Ok($"A document with the '{documentName}' name successfully deleted.");
        }
        catch (Exception ex)
        {
            return SimpleResult.Error($"The document with the '{documentName}' name cannot be deleted with an exception '{ex.Message}'.");
        }
    }

    #region private

    private IResult IsValidDocumentName(string documentName)
    {
        if (string.IsNullOrWhiteSpace(documentName))
        {
            return SimpleResult.Error($"A document name expected.");
        }

        var invalidFileChars = Path.GetInvalidFileNameChars();
        foreach (var c in documentName)
        {
            if (c is '*' or '?' || invalidFileChars.Contains(c))
            {
                return SimpleResult.Error($"Invalid char '{c}' in the '{documentName}' document name found.");
            }
        }

        return SimpleResult.Ok(documentName);
    }
    
    
    private string GetDocumentFileFullPath(string name)
    {
        return Path.Combine(GetDocumentsStoreDirectoryFullPath(), name);
    }
    
    
    private string GetDocumentsStoreDirectoryFullPath()
    {
        return Path.Combine(Location, Name);
    }
    
    
    private int GetDocumentsCount()
    {
        var documentsStorePath = GetDocumentsStoreDirectoryFullPath();
        
        return Directory.Exists(documentsStorePath)
            ? Directory.EnumerateFiles(documentsStorePath).Count() 
            : 0;
    }


    private void CheckName()
    {
        CheckDirectoryPath(Name, "documents store name");
    }

    
    private void CheckLocation()
    {
        CheckDirectoryPath(Location, "documents store location");
    }
    
    
    private void CheckDirectoryPath(string path, string checkedValueName)
    {
        var invalidPathChars = Path.GetInvalidPathChars();
        foreach (var c in path)
        {
            if (c is '*' or '?' || invalidPathChars.Contains(c))
            {
                throw new ArgumentException($"Invalid char '{c}' in {checkedValueName} '{path}' found.");
            }
        }
    }

    #endregion
}
