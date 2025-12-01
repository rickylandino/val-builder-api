using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValPdfAttachmentServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ValPdfAttachmentService _service;

    public ValPdfAttachmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new ValPdfAttachmentService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenAttachmentsExist_ReturnsAllAttachments()
    {
        // Arrange
        var attachments = new List<ValPdfAttachment>
        {
            new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 },
            new ValPdfAttachment { PDFId = 2, ValID = 1, DisplayOrder = 2 },
            new ValPdfAttachment { PDFId = 3, ValID = 2, DisplayOrder = 1 }
        };
        await _context.ValPdfAttachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(attachments);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoAttachmentsExist_ReturnsEmptyList()
    {
        // Arrange - empty database

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_OrdersByValIdThenDisplayOrder()
    {
        // Arrange
        var attachments = new List<ValPdfAttachment>
        {
            new ValPdfAttachment { PDFId = 1, ValID = 2, DisplayOrder = 2 },
            new ValPdfAttachment { PDFId = 2, ValID = 1, DisplayOrder = 2 },
            new ValPdfAttachment { PDFId = 3, ValID = 1, DisplayOrder = 1 },
            new ValPdfAttachment { PDFId = 4, ValID = 2, DisplayOrder = 1 }
        };
        await _context.ValPdfAttachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(4);
        result[0].PDFId.Should().Be(3); // ValID=1, DisplayOrder=1
        result[1].PDFId.Should().Be(2); // ValID=1, DisplayOrder=2
        result[2].PDFId.Should().Be(4); // ValID=2, DisplayOrder=1
        result[3].PDFId.Should().Be(1); // ValID=2, DisplayOrder=2
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsAttachment()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1, PDFName = "Test.pdf" };
        await _context.ValPdfAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.PDFId.Should().Be(1);
        result.PDFName.Should().Be("Test.pdf");
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange - empty database

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DoesNotTrackEntities()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        await _context.ValPdfAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        _context.Entry(result!).State.Should().Be(EntityState.Detached);
    }

    #endregion

    #region GetByValIdAsync Tests

    [Fact]
    public async Task GetByValIdAsync_WhenAttachmentsExistForVal_ReturnsAttachments()
    {
        // Arrange
        var valId = 1;
        var attachments = new List<ValPdfAttachment>
        {
            new ValPdfAttachment { PDFId = 1, ValID = valId, DisplayOrder = 2 },
            new ValPdfAttachment { PDFId = 2, ValID = valId, DisplayOrder = 1 },
            new ValPdfAttachment { PDFId = 3, ValID = 2, DisplayOrder = 1 } // Different ValID
        };
        await _context.ValPdfAttachments.AddRangeAsync(attachments);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _service.GetByValIdAsync(valId)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(a => a.ValID == valId).Should().BeTrue();
        result[0].PDFId.Should().Be(2); // DisplayOrder=1
        result[1].PDFId.Should().Be(1); // DisplayOrder=2
    }

    [Fact]
    public async Task GetByValIdAsync_WhenNoAttachmentsExistForVal_ReturnsEmptyList()
    {
        // Arrange
        var valId = 999;

        // Act
        var result = await _service.GetByValIdAsync(valId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByValIdAsync_DoesNotTrackEntities()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        await _context.ValPdfAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = (await _service.GetByValIdAsync(1)).ToList();

        // Assert
        result.Should().NotBeEmpty();
        foreach (var item in result)
        {
            _context.Entry(item).State.Should().Be(EntityState.Detached);
        }
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidAttachment_ReturnsCreatedAttachment()
    {
        // Arrange
        var newAttachment = new ValPdfAttachment { ValID = 1, DisplayOrder = 1, PDFName = "New.pdf" };

        // Act
        var result = await _service.AddAsync(newAttachment);

        // Assert
        result.Should().NotBeNull();
        result.PDFId.Should().BeGreaterThan(0);
        result.ValID.Should().Be(1);
        result.PDFName.Should().Be("New.pdf");

        // Verify it was saved to database
        var saved = await _context.ValPdfAttachments.FindAsync(result.PDFId);
        saved.Should().NotBeNull();
        saved!.PDFName.Should().Be("New.pdf");
    }

    [Fact]
    public async Task AddAsync_WithPDFContents_StoresContentCorrectly()
    {
        // Arrange
        var pdfContent = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF header
        var newAttachment = new ValPdfAttachment 
        { 
            ValID = 1, 
            DisplayOrder = 1, 
            PDFContents = pdfContent 
        };

        // Act
        var result = await _service.AddAsync(newAttachment);

        // Assert
        result.Should().NotBeNull();
        result.PDFContents.Should().NotBeNull();
        result.PDFContents.Should().BeEquivalentTo(pdfContent);

        // Verify from database
        var saved = await _context.ValPdfAttachments.FindAsync(result.PDFId);
        saved!.PDFContents.Should().BeEquivalentTo(pdfContent);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenAttachmentExists_ReturnsUpdatedAttachment()
    {
        // Arrange
        var existing = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1, PDFName = "Old.pdf" };
        await _context.ValPdfAttachments.AddAsync(existing);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var updateAttachment = new ValPdfAttachment 
        { 
            PDFId = 1, 
            ValID = 2, 
            DisplayOrder = 3, 
            PDFName = "Updated.pdf" 
        };

        // Act
        var result = await _service.UpdateAsync(1, updateAttachment);

        // Assert
        result.Should().NotBeNull();
        result!.PDFId.Should().Be(1);
        result.ValID.Should().Be(2);
        result.DisplayOrder.Should().Be(3);
        result.PDFName.Should().Be("Updated.pdf");

        // Verify in database
        var saved = await _context.ValPdfAttachments.FindAsync(1);
        saved!.PDFName.Should().Be("Updated.pdf");
    }

    [Fact]
    public async Task UpdateAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var updateAttachment = new ValPdfAttachment { PDFId = 999, ValID = 1, DisplayOrder = 2 };

        // Act
        var result = await _service.UpdateAsync(999, updateAttachment);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPDFContents()
    {
        // Arrange
        var existing = new ValPdfAttachment 
        { 
            PDFId = 1, 
            ValID = 1, 
            DisplayOrder = 1, 
            PDFContents = new byte[] { 1, 2, 3 } 
        };
        await _context.ValPdfAttachments.AddAsync(existing);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var newContents = new byte[] { 4, 5, 6, 7 };
        var updateAttachment = new ValPdfAttachment 
        { 
            PDFId = 1, 
            ValID = 1, 
            DisplayOrder = 1, 
            PDFContents = newContents 
        };

        // Act
        var result = await _service.UpdateAsync(1, updateAttachment);

        // Assert
        result!.PDFContents.Should().BeEquivalentTo(newContents);

        // Verify in database
        var saved = await _context.ValPdfAttachments.FindAsync(1);
        saved!.PDFContents.Should().BeEquivalentTo(newContents);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_ReturnsTrue()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        await _context.ValPdfAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();

        // Verify it was deleted
        var deleted = await _context.ValPdfAttachments.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentDoesNotExist_ReturnsFalse()
    {
        // Arrange - empty database

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesFromDatabase()
    {
        // Arrange
        var attachment = new ValPdfAttachment { PDFId = 1, ValID = 1, DisplayOrder = 1 };
        await _context.ValPdfAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();

        // Verify it exists
        (await _context.ValPdfAttachments.FindAsync(1)).Should().NotBeNull();

        // Act
        await _service.DeleteAsync(1);

        // Assert
        var count = await _context.ValPdfAttachments.CountAsync();
        count.Should().Be(0);
    }

    #endregion
}
