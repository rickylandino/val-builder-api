using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Dto;
using val_builder_api.Models;
using val_builder_api.Services.Impl;
using Xunit;

namespace val_builder_api.Tests.Services;

public class ValDetailServiceTests
{
    private static ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetValDetailsAsync_ReturnsAllDetailsForValId()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var detail1 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 1,
            GroupContent = "Detail 1",
            DisplayOrder = 1
        };

        var detail2 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 2,
            GroupContent = "Detail 2",
            DisplayOrder = 2
        };

        var detail3 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 2,
            GroupId = 1,
            GroupContent = "Detail 3",
            DisplayOrder = 1
        };

        context.Valdetails.AddRange(detail1, detail2, detail3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetValDetailsAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, d => Assert.Equal(1, d.ValId));
    }

    [Fact]
    public async Task GetValDetailsAsync_FiltersByGroupId()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var detail1 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 1,
            GroupContent = "Detail 1",
            DisplayOrder = 1
        };

        var detail2 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 2,
            GroupContent = "Detail 2",
            DisplayOrder = 2
        };

        context.Valdetails.AddRange(detail1, detail2);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetValDetailsAsync(1, groupId: 1);

        // Assert
        var details = result.ToList();
        Assert.Single(details);
        Assert.Equal(1, details[0].GroupId);
    }

    [Fact]
    public async Task GetValDetailsAsync_OrdersByDisplayOrder()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var detail1 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 1,
            GroupContent = "Detail 3",
            DisplayOrder = 3
        };

        var detail2 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 1,
            GroupContent = "Detail 1",
            DisplayOrder = 1
        };

        var detail3 = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            ValId = 1,
            GroupId = 1,
            GroupContent = "Detail 2",
            DisplayOrder = 2
        };

        context.Valdetails.AddRange(detail1, detail2, detail3);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetValDetailsAsync(1);

        // Assert
        var details = result.ToList();
        Assert.Equal(3, details.Count);
        Assert.Equal(1, details[0].DisplayOrder);
        Assert.Equal(2, details[1].DisplayOrder);
        Assert.Equal(3, details[2].DisplayOrder);
    }

    [Fact]
    public async Task GetValDetailByIdAsync_ReturnsDetail_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var detail = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "Test Detail"
        };

        context.Valdetails.Add(detail);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetValDetailByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.ValDetailsId);
        Assert.Equal("Test Detail", result.GroupContent);
    }

    [Fact]
    public async Task GetValDetailByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        // Act
        var result = await service.GetValDetailByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateValDetailAsync_CreatesDetail()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var detail = new Valdetail
        {
            ValId = 1,
            GroupId = 1,
            GroupContent = "New Detail",
            DisplayOrder = 1
        };

        // Act
        var result = await service.CreateValDetailAsync(detail);

        // Assert
        Assert.NotEqual(Guid.Empty, result.ValDetailsId);
        Assert.Equal("New Detail", result.GroupContent);

        var saved = await context.Valdetails.FindAsync(result.ValDetailsId);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task CreateValDetailAsync_UsesProvidedGuid()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var detail = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "New Detail"
        };

        // Act
        var result = await service.CreateValDetailAsync(detail);

        // Assert
        Assert.Equal(id, result.ValDetailsId);
    }

    [Fact]
    public async Task UpdateValDetailAsync_UpdatesExistingDetail()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var detail = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "Original",
            DisplayOrder = 1
        };

        context.Valdetails.Add(detail);
        await context.SaveChangesAsync();

        var updateDetail = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "Updated",
            DisplayOrder = 2,
            Bullet = true,
            Bold = true
        };

        // Act
        var result = await service.UpdateValDetailAsync(id, updateDetail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.GroupContent);
        Assert.Equal(2, result.DisplayOrder);
        Assert.True(result.Bullet);
        Assert.True(result.Bold);
    }

    [Fact]
    public async Task UpdateValDetailAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var updateDetail = new Valdetail
        {
            ValDetailsId = Guid.NewGuid(),
            GroupContent = "Updated"
        };

        // Act
        var result = await service.UpdateValDetailAsync(Guid.NewGuid(), updateDetail);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteValDetailAsync_DeletesDetail_WhenExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var detail = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "To Delete"
        };

        context.Valdetails.Add(detail);
        await context.SaveChangesAsync();

        // Act
        var result = await service.DeleteValDetailAsync(id);

        // Assert
        Assert.True(result);
        var deleted = await context.Valdetails.FindAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteValDetailAsync_ReturnsFalse_WhenNotExists()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        // Act
        var result = await service.DeleteValDetailAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SaveValDetailChangesAsync_CreatesDetails()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var dto = new ValDetailChangeDto
        {
            ValId = 1,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "create",
                    Detail = new Valdetail
                    {
                        ValId = 1,
                        GroupContent = "New Detail 1"
                    }
                },
                new ValDetailChange
                {
                    Action = "create",
                    Detail = new Valdetail
                    {
                        ValId = 1,
                        GroupContent = "New Detail 2"
                    }
                }
            }
        };

        // Act
        var result = await service.SaveValDetailChangesAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.ItemsCreated);
        Assert.Equal(0, result.ItemsUpdated);
        Assert.Equal(0, result.ItemsDeleted);
        Assert.Equal(2, context.Valdetails.Count());
    }

    [Fact]
    public async Task SaveValDetailChangesAsync_UpdatesDetails()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var existing = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "Original"
        };

        context.Valdetails.Add(existing);
        await context.SaveChangesAsync();

        var dto = new ValDetailChangeDto
        {
            ValId = 1,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "update",
                    Detail = new Valdetail
                    {
                        ValDetailsId = id,
                        ValId = 1,
                        GroupContent = "Updated"
                    }
                }
            }
        };

        // Act
        var result = await service.SaveValDetailChangesAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.ItemsCreated);
        Assert.Equal(1, result.ItemsUpdated);
        Assert.Equal(0, result.ItemsDeleted);

        var updated = await context.Valdetails.FindAsync(id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.GroupContent);
    }

    [Fact]
    public async Task SaveValDetailChangesAsync_DeletesDetails()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var id = Guid.NewGuid();
        var existing = new Valdetail
        {
            ValDetailsId = id,
            ValId = 1,
            GroupContent = "To Delete"
        };

        context.Valdetails.Add(existing);
        await context.SaveChangesAsync();

        var dto = new ValDetailChangeDto
        {
            ValId = 1,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "delete",
                    Detail = new Valdetail
                    {
                        ValDetailsId = id
                    }
                }
            }
        };

        // Act
        var result = await service.SaveValDetailChangesAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.ItemsCreated);
        Assert.Equal(0, result.ItemsUpdated);
        Assert.Equal(1, result.ItemsDeleted);
        Assert.Empty(context.Valdetails);
    }

    [Fact]
    public async Task SaveValDetailChangesAsync_HandlesMultipleOperations()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var updateId = Guid.NewGuid();
        var deleteId = Guid.NewGuid();

        context.Valdetails.AddRange(
            new Valdetail { ValDetailsId = updateId, ValId = 1, GroupContent = "To Update" },
            new Valdetail { ValDetailsId = deleteId, ValId = 1, GroupContent = "To Delete" }
        );
        await context.SaveChangesAsync();

        var dto = new ValDetailChangeDto
        {
            ValId = 1,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "create",
                    Detail = new Valdetail { ValId = 1, GroupContent = "New" }
                },
                new ValDetailChange
                {
                    Action = "update",
                    Detail = new Valdetail { ValDetailsId = updateId, ValId = 1, GroupContent = "Updated" }
                },
                new ValDetailChange
                {
                    Action = "delete",
                    Detail = new Valdetail { ValDetailsId = deleteId }
                }
            }
        };

        // Act
        var result = await service.SaveValDetailChangesAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.ItemsCreated);
        Assert.Equal(1, result.ItemsUpdated);
        Assert.Equal(1, result.ItemsDeleted);
        Assert.Equal(2, context.Valdetails.Count());
    }

    [Fact]
    public async Task SaveValDetailChangesAsync_RollsBackOnError()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var service = new ValDetailService(context);

        var dto = new ValDetailChangeDto
        {
            ValId = 1,
            Changes = new List<ValDetailChange>
            {
                new ValDetailChange
                {
                    Action = "create",
                    Detail = new Valdetail { ValId = 1, GroupContent = "New" }
                },
                new ValDetailChange
                {
                    Action = "update",
                    Detail = new Valdetail { ValDetailsId = Guid.NewGuid(), ValId = 1, GroupContent = "Update Non-Existent" }
                }
            }
        };

        // Act
        var result = await service.SaveValDetailChangesAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.NotEmpty(result.Errors);
        Assert.Empty(context.Valdetails); // Should have rolled back
    }
}
