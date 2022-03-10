/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core.Tests;

using System;
    
using Xunit;

using DocumentsStore.Core;
    
    
public class Document_Tests
{
    [Fact]
    public void Document_CanCreateEmpty()
    {
        _ = new Document("empty", Array.Empty<byte>());
    }
        
    [Fact]
    public void Document_NullNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new Document(null, Array.Empty<byte>()));
    }
        
    [Fact]
    public void Document_EmptyNameNotAllowed()
    {
        Assert.Throws<ArgumentException>(() => new Document("", Array.Empty<byte>()));
    }
        
    [Fact]
    public void Document_NullContentNotAllowed()
    {
        Assert.Throws<ArgumentNullException>(() => new Document("empty", null));
    }
        
    [Fact]
    public void Document_NewDocumentContainsName()
    {
        var document = new Document("document-name", Array.Empty<byte>());
            
        Assert.Equal("document-name", document.Name);
    }
        
    [Fact]
    public void Document_NewDocumentContainsContent()
    {
        var document = new Document("document", new byte[] { 1, 2, 3 });
            
        Assert.Equal(3, document.Content.Length);
            
        Assert.Equal(1, document.Content[0]);
        Assert.Equal(2, document.Content[1]);
        Assert.Equal(3, document.Content[2]);
    }
}