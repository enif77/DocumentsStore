/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core.Tests;

using System;
using System.Linq;

using Xunit;

using DocumentsStore.Core;


public class InMemoryDocumentsStore_Tests
{
    #region ctor tests
    
    [Fact]
    public void InMemoryDocumentsStore_NullNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new InMemoryDocumentsStore(null));
    }
    
    [Fact]
    public void InMemoryDocumentsStore_EmptyNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new InMemoryDocumentsStore(""));
    }
    
    [Fact]
    public void InMemoryDocumentsStore_NewDocumentsStoreContainsName()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.Equal(TestDocumentsStoreName, store.Name);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_NewDocumentsStoreIsNotOpened()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_NewDocumentsStoreReturnsDocuments()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.NotNull(store.Documents);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_NewDocumentsStoreDocumentsCountIsZero()
    {
        var store = CreateEmptyDocumentsStore();
       
        Assert.Equal(0, store.Count);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_NewDocumentsStoreIsEmpty()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.Empty(store.Documents);
    }
    
    #endregion
    
    
    #region Open - Close tests
    
    [Fact]
    public void Open_OpenOpensDocumentsStore()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);

        var openResult = store.Open();
        
        Assert.True(openResult.IsSuccess);
        Assert.True(store.IsOpened);
    }
    
    [Fact]
    public void Close_CloseClosesDocumentsStore()
    {
        var store = CreateEmptyDocumentsStore();

        _ = store.Open();
        
        Assert.True(store.IsOpened);

        store.Close();
        
        Assert.False(store.IsOpened);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_OpenedDocumentsStoreReturnsNonEmptyDocuments()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.Documents.Any());
    }
    
    [Fact]
    public void InMemoryDocumentsStore_OpenedNonEmptyDocumentsStoreDocumentsCountIsNotZero()
    {
        var store = CreateDocumentsStoreWithDocument();

        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        Assert.Equal(1, store.Count);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_ClosedNonEmptyDocumentsStoreDocumentsCountIsZero()
    {
        var store = CreateDocumentsStoreWithDocument();

        Assert.Equal(1, store.Count);
        
        store.Close();
        
        Assert.Equal(0, store.Count);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_OpenedNonEmptyDocumentsStoreIsNotEmpty()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.NotEmpty(store.Documents);
    }
    
    [Fact]
    public void InMemoryDocumentsStore_ClosedStoreReturnsEmptyDocuments()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.Documents.Any());
        
        store.Close();
        
        Assert.False(store.Documents.Any());
    }
    
    [Fact]
    public void InMemoryDocumentsStore_ReopenedStoreReturnsNonEmptyDocuments()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.Documents.Any());
        
        store.Close();
        
        Assert.False(store.Documents.Any());

        _ = store.Open();
        
        Assert.True(store.Documents.Any());
    }
    
    #endregion

    
    #region HasDocument tests
    
    [Fact]
    public void HasDocument_ExistingDocumentIsFoundInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.HasDocument(TestDocumentName));
    }
    
    [Fact]
    public void HasDocument_ExistingDocumentIsNotFoundInClosedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.True(store.HasDocument(TestDocumentName));
        
        store.Close();
        
        Assert.False(store.IsOpened);
        Assert.False(store.HasDocument(TestDocumentName));
    }
    
    [Fact]
    public void HasDocument_DocumentWithNullNameIsNotFoundInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument(null));
    }
    
    [Fact]
    public void HasDocument_DocumentWithEmptyNameIsNotFoundInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument(""));
    }
    
    [Fact]
    public void HasDocument_NotExistingDocumentIsNotFoundInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.False(store.HasDocument("bla"));
    }
    
    #endregion
    
    
    #region Load tests
    
    [Fact]
    public void Load_ExistingDocumentIsLoadedFromOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var loadResult = store.Load(TestDocumentName);
        
        Assert.True(loadResult.IsSuccess);
        Assert.Equal(TestDocumentName, loadResult.Data.Name);
    }
    
    [Fact]
    public void Load_DocumentWithNullNameIsNotLoadedFromOpenedDocumentsStore()
    {
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
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var loadResult = store.Load("");
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains("document name expected", loadResult.Message);
        Assert.Null(loadResult.Data);
    }
    
    [Fact]
    public void Load_ExistingDocumentIsNotLoadedFromClosedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        
        store.Close();

        var loadResult = store.Load(TestDocumentName);
        
        Assert.False(loadResult.IsSuccess);
        Assert.Contains($"store '{store.Name}' is not opened", loadResult.Message);
    }
    
    [Fact]
    public void Load_NotExistingDocumentIsNotLoadedFromOpenedDocumentsStore()
    {
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
        var store = CreateEmptyDocumentsStore();
        
        var saveResult = store.Save(null);
        
        Assert.False(saveResult.IsSuccess);
        Assert.Contains("document expected", saveResult.Message);
    }
    
    [Fact]
    public void Save_DocumentIsNotSavedToClosedDocumentsStore()
    {
        var store = CreateEmptyDocumentsStore();
        
        Assert.False(store.IsOpened);
   
        var saveResult = store.Save(CreateTestDocument());
        
        Assert.False(saveResult.IsSuccess);
        Assert.Contains($"The '{store.Name}' documents store is not opened", saveResult.Message);
    }
    
    [Fact]
    public void Save_DocumentIsSavedToOpenedDocumentsStore()
    {
        var store = CreateEmptyDocumentsStore();

        _ = store.Open(); 
        
        Assert.True(store.IsOpened);
        Assert.False(store.Documents.Any());

        var document = CreateTestDocument();
        var saveResult = store.Save(document);
        
        Assert.True(saveResult.IsSuccess);
        Assert.Contains($"document with name '{document.Name}' successfully saved", saveResult.Message);
        Assert.Single(store.Documents);
    }
    
    [Fact]
    public void Save_SavedDocumentCanBeLoaded()
    {
        var store = CreateEmptyDocumentsStore();

        _ = store.Open(); 
        
        var document = CreateTestDocument();
        var saveResult = store.Save(document);
        
        Assert.True(saveResult.IsSuccess);

        var loadResult = store.Load(document.Name);
        
        Assert.True(loadResult.IsSuccess);
        Assert.Equal(document.Name, loadResult.Data.Name);
    }
    
    [Fact]
    public void Save_ExistingDocumentIsReplaced()
    {
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
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var renameResult = store.Rename(null, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithEmptyNameIsNotRenamedInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var renameResult = store.Rename("", "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithNullNewNameIsNotRenamedInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var renameResult = store.Rename(TestDocumentName, null);
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("new document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_DocumentWithEmptyNewNameIsNotRenamedInOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);

        var renameResult = store.Rename(TestDocumentName, "");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains("new document name expected", renameResult.Message);
    }
    
    [Fact]
    public void Rename_ExistingDocumentIsNotRenamedInClosedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        
        store.Close();

        var renameResult = store.Rename(TestDocumentName, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"store '{store.Name}' is not opened", renameResult.Message);

        store.Open();
        
        Assert.True(store.HasDocument(TestDocumentName));
    }

    [Fact]
    public void Rename_NotExistingDocumentCannotBeRenamedInDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var documentName = "not-existing-document-name";
        var renameResult = store.Rename(documentName, "new-name");
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"document with name '{documentName}' not found", renameResult.Message);
    }
    
    [Fact]
    public void Rename_ExistingDocumentNotRenamedToExistingDocumentInDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var otherDocumentName = "other-document-name";

        var saveResult = store.Save(new Document(otherDocumentName, Array.Empty<byte>()));
        
        Assert.True(saveResult.IsSuccess);
        
        var renameResult = store.Rename(TestDocumentName, otherDocumentName);
        
        Assert.False(renameResult.IsSuccess);
        Assert.Contains($"document with name '{otherDocumentName}' already exists", renameResult.Message);
        Assert.True(store.HasDocument(TestDocumentName));
        Assert.True(store.HasDocument(otherDocumentName));
    }
    
    [Fact]
    public void Rename_ExistingDocumentIsRenamedInDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var newDocumentName = "new-document-name";

        Assert.True(store.HasDocument(TestDocumentName));
        Assert.False(store.HasDocument(newDocumentName));
        
        var renameResult = store.Rename(TestDocumentName, newDocumentName);
        
        Assert.True(renameResult.IsSuccess);
        Assert.Contains($"document with name '{TestDocumentName}' renamed to '{newDocumentName}' successfully", renameResult.Message);
        Assert.False(store.HasDocument(TestDocumentName));
        Assert.True(store.HasDocument(newDocumentName));
    }
    
    #endregion
    
    
    #region Delete tests

    [Fact]
    public void Delete_ExistingDocumentIsDeletedFromOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var deleteResult = store.Delete(TestDocumentName);
        
        Assert.True(deleteResult.IsSuccess);
        Assert.Contains($"document with name '{TestDocumentName}' successfully deleted", deleteResult.Message);
        Assert.Empty(store.Documents);
    }
    
    [Fact]
    public void Delete_DocumentWithNullNameIsNotDeletedFromOpenedDocumentsStore()
    {
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
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var deleteResult = store.Delete("");
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains("document name expected", deleteResult.Message);
        Assert.Single(store.Documents);
    }

    [Fact]
    public void Delete_ExistingDocumentIsNotDeletedFromClosedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);
        
        store.Close();

        var deleteResult = store.Delete(TestDocumentName);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains($"store '{store.Name}' is not opened", deleteResult.Message);

        _ = store.Open();
        
        Assert.Single(store.Documents);
    }
    
    [Fact]
    public void Delete_NotExistingDocumentIsNotDeletedFromOpenedDocumentsStore()
    {
        var store = CreateDocumentsStoreWithDocument();
        
        Assert.True(store.IsOpened);
        Assert.Single(store.Documents);

        var documentName = "bla";
        var deleteResult = store.Delete(documentName);
        
        Assert.False(deleteResult.IsSuccess);
        Assert.Contains($"document with name '{documentName}' not found", deleteResult.Message);
        Assert.Single(store.Documents);
    }
    
    #endregion
    
    
    #region setups

    private const string TestDocumentsStoreName = "test-documents-store";
    private const string TestDocumentName = "test-document";


    private IDocument CreateTestDocument() => new Document(TestDocumentName, new byte[] {1, 2, 3});
    
    
    private InMemoryDocumentsStore CreateEmptyDocumentsStore() => new InMemoryDocumentsStore(TestDocumentsStoreName);
    
    
    private InMemoryDocumentsStore CreateDocumentsStoreWithDocument()
    {
        var store = CreateEmptyDocumentsStore();

        store.Open();
        store.Save(CreateTestDocument());

        return store;
    }

    #endregion
    
}
