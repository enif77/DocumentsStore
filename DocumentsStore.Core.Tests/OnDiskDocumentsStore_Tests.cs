/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Xunit;

using DocumentsStore.Core;


public class OnDiskDocumentsStore_Tests
{
    #region ctor tests
    
    [Fact]
    public void OnDiskDocumentsStore_NullNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore(null, TestDocumentsStoreLocation));
    }
    
    [Fact]
    public void OnDiskDocumentsStore_EmptyNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore("", TestDocumentsStoreLocation));
    }
    
    [Theory]
    [MemberData(nameof(InvalidNamesList))]
    public void OnDiskDocumentsStore_InvalidNameNotAllowed(string name)
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore(name, TestDocumentsStoreLocation));
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NullLocationNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore(TestDocumentsStoreName, null));
    }
    
    [Fact]
    public void OnDiskDocumentsStore_EmptyLocationNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore(TestDocumentsStoreName, ""));
    }
    
    [Theory]
    [MemberData(nameof(InvalidPathsList))]
    public void OnDiskDocumentsStore_InvalidLocationNotAllowed(string location)
    {
        Assert.Throws<ArgumentException>(() => new OnDiskDocumentsStore(TestDocumentsStoreName, location));
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreContainsName()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.Equal(TestDocumentsStoreName, store.Name);
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreContainsLocation()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.Equal(TestDocumentsStoreLocation, store.Location);
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreIsNotOpened()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreReturnsDocuments()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.NotNull(store.Documents);
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreDocumentsCountIsZero()
    {
        var store = CreateEmptyDocumentsStore();
       
        Assert.Equal(0, store.Count);
    }
    
    [Fact]
    public void OnDiskDocumentsStore_NewDocumentsStoreIsEmpty()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.Empty(store.Documents);
    }
    
    #endregion

    
    #region Open - Close tests
    
    [Fact]
    public void OpenClose_OpenOpensDocumentsStore()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);
    
        var openResult = store.Open();
        
        Assert.True(openResult.IsSuccess);
        Assert.True(store.IsOpened);
    }
    
    [Fact]
    public void OpenClose_OpenCreatesDocumentsStoreDirectories()
    {
        Cleanup();
        
        Assert.False(Directory.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName)));
        
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);
        Assert.False(Directory.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName)));
    
        var openResult = store.Open();
        
        Assert.True(openResult.IsSuccess);
        Assert.True(Directory.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName)));
    }
    
    [Fact]
    public void OpenClose_CloseClosesDocumentsStore()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
    
        _ = store.Open();
        
        Assert.True(store.IsOpened);
    
        store.Close();
        
        Assert.False(store.IsOpened);
    }
    
    [Fact]
    public void OpenClose_OpenedDocumentsStoreReturnsNonEmptyDocuments()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.Documents.Any());
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void OpenClose_OpenedNonEmptyDocumentsStoreDocumentsCountIsNotZero()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
    
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        Assert.Equal(1, store.Count);
    }
    
    [Fact]
    public void OpenClose_ClosedNonEmptyDocumentsStoreDocumentsCountIsZero()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
    
        Assert.Equal(1, store.Count);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        
        store.Close();
        
        Assert.Equal(0, store.Count);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void OpenClose_OpenedNonEmptyDocumentsStoreIsNotEmpty()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.NotEmpty(store.Documents);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void OpenClose_ClosedStoreReturnsEmptyDocuments()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.NotEmpty(store.Documents);
        
        store.Close();
        
        Assert.Empty(store.Documents);
    }
    
    [Fact]
    public void OpenClose_ReopenedStoreReturnsNonEmptyDocuments()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.NotEmpty(store.Documents);
        
        store.Close();
        
        Assert.Empty(store.Documents);
    
        _ = store.Open();
        
        Assert.NotEmpty(store.Documents);
    }
    
    #endregion

    
    #region HasDocument tests
    
    [Fact]
    public void HasDocument_ExistingDocumentIsFoundInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.HasDocument(TestDocumentName));
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void HasDocument_ExistingDocumentIsNotFoundInClosedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.HasDocument(TestDocumentName));
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        
        store.Close();
        
        Assert.False(store.IsOpened);
        Assert.False(store.HasDocument(TestDocumentName));
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void HasDocument_DocumentWithNullNameIsNotFoundInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument(null));
    }
    
    [Fact]
    public void HasDocument_DocumentWithEmptyNameIsNotFoundInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument(""));
    }
    
    [Fact]
    public void HasDocument_NotExistingDocumentIsNotFoundInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument("bla"));
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, "bla")));
    }
    
    #endregion
    
    
    #region Load tests
    
    [Fact]
    public void Load_ExistingDocumentIsLoadedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var loadResult = store.Load(TestDocumentName);
        
        Assert.True(loadResult.IsSuccess);
        Assert.Equal(TestDocumentName, loadResult.Data.Name);
    }
    
    [Fact]
    public void Load_DocumentWithNullNameIsNotLoadedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var loadResult = store.Load(null);
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains("document name expected", loadResult.Message);
        Assert.Null(loadResult.Data);
    }
    
    [Fact]
    public void Load_DocumentWithEmptyNameIsNotLoadedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var loadResult = store.Load("");
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains("document name expected", loadResult.Message);
        Assert.Null(loadResult.Data);
    }
    
    [Theory]
    [MemberData(nameof(InvalidNamesList))]
    public void Load_DocumentWithInvalidNameNotAllowed(string name)
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var loadResult = store.Load(name);
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains("Invalid char", loadResult.Message);
        Assert.Null(loadResult.Data);
    }
    
    [Fact]
    public void Load_ExistingDocumentIsNotLoadedFromClosedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        
        store.Close();
    
        var loadResult = store.Load(TestDocumentName);
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains($"store '{store.Name}' is not opened", loadResult.Message);
    }
    
    [Fact]
    public void Load_NotExistingDocumentIsNotLoadedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var documentName = "bla";
        var loadResult = store.Load(documentName);
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains($"document with name '{documentName}' not found", loadResult.Message);
        Assert.Null(loadResult.Data);
    }
    
    #endregion
    
    
    #region Save tests
    
    [Fact]
    public void Save_NullDocumentIsNotSavedToDocumentsStore()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
        
        var saveResult = store.Save(null);
        
        Assert.False(saveResult.IsSuccess);
        Assert.Contains("document expected", saveResult.Message);
    }
    
    [Fact]
    public void Save_DocumentIsNotSavedToClosedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);

        var document = CreateTestDocument();
        var saveResult = store.Save(document);
        
        Assert.False(saveResult.IsSuccess);
        Assert.Contains($"'{store.Name}' documents store is not opened", saveResult.Message);
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, document.Name)));
    }
    
    [Fact]
    public void Save_DocumentIsSavedToOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
    
        _ = store.Open(); 
        
        Assert.True(store.IsOpened);
        Assert.Empty(store.Documents);
        
        var document = CreateTestDocument();
        
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, document.Name)));
        
        var saveResult = store.Save(document);
        
        Assert.True(saveResult.IsSuccess);
        Assert.Contains($"document with the '{document.Name}' name successfully saved", saveResult.Message);
        Assert.Single(store.Documents);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, document.Name)));
    }
    
    [Fact]
    public void Save_SavedDocumentCanBeLoaded()
    {
        Cleanup();
        
        var store = CreateEmptyDocumentsStore();
    
        _ = store.Open(); 
        
        var document = CreateTestDocument();
        var saveResult = store.Save(document);
        
        Assert.True(saveResult.IsSuccess);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, document.Name)));
    
        var loadResult = store.Load(document.Name);
        
        Assert.True(loadResult.IsSuccess);
        Assert.Equal(document.Name, loadResult.Data.Name);
    }
    
    [Fact]
    public void Save_ExistingDocumentIsReplaced()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
    
        _ = store.Open();
    
        var existingDocumentLoadResult = store.Load(TestDocumentName);
        
        Assert.True(existingDocumentLoadResult.IsSuccess);
        Assert.Equal(1, existingDocumentLoadResult.Data.Content[0]);
        
        var newDocument = new Document(TestDocumentName, new byte[] { 4 });
        var saveResult = store.Save(newDocument);
        
        Assert.True(saveResult.IsSuccess);
    
        var loadResult = store.Load(newDocument.Name);
        
        Assert.True(loadResult.IsSuccess);
        Assert.Equal(4, loadResult.Data.Content[0]);
    }
    
    #endregion


    #region Rename tests

    [Fact]
    public void Rename_DocumentWithNullNameIsNotRenamedInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename(null, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithEmptyNameIsNotRenamedInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename("", "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Theory]
    [MemberData(nameof(InvalidNamesList))]
    public void Rename_DocumentWithInvalidNameIsNotRenamedInOpenedDocumentsStore(string name)
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename(name, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("Invalid char", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithNullNewNameIsNotRenamedInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename(TestDocumentName, null);
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithEmptyNewNameIsNotRenamedInOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename(TestDocumentName, "");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Theory]
    [MemberData(nameof(InvalidNamesList))]
    public void Rename_DocumentWithInvalidNewNameIsNotRenamedInOpenedDocumentsStore(string newName)
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
    
        var renameResult = store.Rename(TestDocumentName, newName);
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("Invalid char", renameResult.Message);
    }
    
    [Fact]
    public void Rename_ExistingDocumentIsNotRenamedInClosedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, "new-name")));
        
        store.Close();
    
        var renameResult = store.Rename(TestDocumentName, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"'{store.Name}' documents store is not opened", renameResult.Message);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, "new-name")));
    }
    
    [Fact]
    public void Rename_NotExistingDocumentCannotBeRenamedInDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        
        var documentName = "not-existing-document-name";
        
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, documentName)));
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, "new-name")));
        
        var renameResult = store.Rename(documentName, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"document with the '{documentName}' name not found", renameResult.Message);
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, "new-name")));
    }
    
    [Fact]
    public void Rename_ExistingDocumentNotRenamedToExistingDocumentInDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
    
        var otherDocumentName = "other-document-name";
    
        var saveResult = store.Save(new Document(otherDocumentName, Array.Empty<byte>()));
        
        Assert.True(saveResult.IsSuccess);
        
        var renameResult = store.Rename(TestDocumentName, otherDocumentName);
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"document with the '{otherDocumentName}' name already exists", renameResult.Message);
        Assert.True(store.HasDocument(TestDocumentName));
        Assert.True(store.HasDocument(otherDocumentName));
    }
    
    [Fact]
    public void Rename_ExistingDocumentIsRenamedInDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
    
        var newDocumentName = "new-document-name.bin";
    
        Assert.True(store.HasDocument(TestDocumentName));
        Assert.False(store.HasDocument(newDocumentName));
        
        var renameResult = store.Rename(TestDocumentName, newDocumentName);
        
        Assert.True(renameResult.IsSuccess);
        Assert.Contains($"document with name '{TestDocumentName}' renamed to '{newDocumentName}' successfully", renameResult.Message);
        Assert.False(store.HasDocument(TestDocumentName));
        Assert.True(store.HasDocument(newDocumentName));
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, newDocumentName)));
    }
    
    #endregion
    
    
    #region Delete tests

    [Fact]
    public void Delete_ExistingDocumentIsDeletedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    
        var deleteResult = store.Delete(TestDocumentName);
        
        Assert.True(deleteResult.IsSuccess);
        Assert.Contains($"document with the '{TestDocumentName}' name successfully deleted", deleteResult.Message);
        Assert.Empty(store.Documents);
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void Delete_DocumentWithNullNameIsNotDeletedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
    
        var deleteResult = store.Delete(null);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains("document name expected", deleteResult.Message);
        Assert.Single(store.Documents);
    }
    
    [Fact]
    public void Delete_DocumentWithEmptyNameIsNotDeletedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
    
        var deleteResult = store.Delete("");
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains("document name expected", deleteResult.Message);
        Assert.Single(store.Documents);
    }
    
    [Theory]
    [MemberData(nameof(InvalidNamesList))]
    public void Delete_DocumentWithInvalidNameIsNotDeletedFromOpenedDocumentsStore(string name)
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
    
        var deleteResult = store.Delete(name);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains("Invalid char", deleteResult.Message);
        Assert.Single(store.Documents);
    }
    
    [Fact]
    public void Delete_ExistingDocumentIsNotDeletedFromClosedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
        
        store.Close();
    
        var deleteResult = store.Delete(TestDocumentName);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains($"'{store.Name}' documents store is not opened", deleteResult.Message);
    
        _ = store.Open();
        
        Assert.Single(store.Documents);
        Assert.True(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, TestDocumentName)));
    }
    
    [Fact]
    public void Delete_NotExistingDocumentIsNotDeletedFromOpenedDocumentsStore()
    {
        Cleanup();
        
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        
        var documentName = "bla";
        
        Assert.False(File.Exists(Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName, documentName)));
        
        var deleteResult = store.Delete(documentName);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains($"document with the '{documentName}' name not found", deleteResult.Message);
        Assert.Single(store.Documents);
    }
    
    #endregion
    
    
    #region setups
    
    private const string TestDocumentsStoreLocation = @"C:\Temp";
    private const string TestDocumentsStoreName = "test-documents-store";
    private const string TestDocumentName = "test-document.bin";


    private IDocument CreateTestDocument() => new Document(TestDocumentName, new byte[] {1, 2, 3});
    
    
    private OnDiskDocumentsStore CreateEmptyDocumentsStore() => new OnDiskDocumentsStore(TestDocumentsStoreName, TestDocumentsStoreLocation);
    
    
    private OnDiskDocumentsStore CreateDocumentsStoreWithDocument()
    {
        var store = CreateEmptyDocumentsStore();

        store.Open();
        store.Save(CreateTestDocument());

        return store;
    }


    private void Cleanup()
    {
        var storePath = Path.Combine(TestDocumentsStoreLocation, TestDocumentsStoreName);
        if (Directory.Exists(storePath))
        {
            Directory.Delete(storePath, true);
        }
    }
    
    
    // Linux invalid chars:
    //   '\0', '/' -> name
    //   '\0' -> path
    
    // Windows invalid chars:
    //   '\0', ...  -> name
    //   '\0', ... -> path
    
    public static IEnumerable<object[]> InvalidNamesList =>
        new List<object[]>
        {
            // new object[] { (string)null },
            // new object[] { "" },
            new object[] { "na*e" },
            new object[] { "na?e" },
            new object[] { "na\0e" },
            //new object[] { "na/e" }
        };
    
    public static IEnumerable<object[]> InvalidPathsList =>
        new List<object[]>
        {
            // new object[] { (string)null },
            // new object[] { "" },
            new object[] { "na*e" },
            new object[] { "na?e" },
            new object[] { "na\0e" },
            //new object[] { "na/e" }
        };

    #endregion
}
