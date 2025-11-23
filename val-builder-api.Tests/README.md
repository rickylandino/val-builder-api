# val-builder-api.Tests

Unit tests for the val-builder-api RESTful API.

## Overview

This test project contains comprehensive unit tests for the Companies API, including:
- **Service Layer Tests** - Testing business logic with in-memory database
- **Controller Tests** - Testing API endpoints with mocked services

## Technologies & Libraries

- **xUnit** - Testing framework
- **Moq** - Mocking framework for creating test doubles
- **FluentAssertions** - Fluent API for writing readable assertions
- **EntityFrameworkCore.InMemory** - In-memory database for integration testing

## Project Structure

```
val-builder-api.Tests/
├── Controllers/
│   └── CompaniesControllerTests.cs    # Tests for CompaniesController
├── Services/
│   └── CompanyServiceTests.cs         # Tests for CompanyService
└── README.md
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Detailed Output
```bash
dotnet test --verbosity normal
```

### Run Tests with Code Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~CompaniesControllerTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~GetAllCompanies_WhenCompaniesExist_ReturnsOkWithCompanies"
```

## Test Coverage

### CompanyServiceTests
Tests the `CompanyService` class with the following scenarios:

✅ **GetAllCompaniesAsync**
- Returns empty list when no companies exist
- Returns all companies when they exist
- Returns companies in correct order
- Does not track entities (AsNoTracking verification)

✅ **GetCompanyByIdAsync**
- Returns company when it exists
- Returns null when company doesn't exist
- Returns null when database is empty
- Returns correct company from multiple records
- Does not track entities (AsNoTracking verification)

**Total Service Tests: 9**

### CompaniesControllerTests
Tests the `CompaniesController` class with the following scenarios:

✅ **GET /api/companies**
- Returns 200 OK with companies when they exist
- Returns 200 OK with empty list when no companies
- Calls service exactly once
- Has correct API attributes and routing

✅ **GET /api/companies/{id}**
- Returns 200 OK with company when it exists
- Returns 404 Not Found when company doesn't exist
- Returns 404 with error message
- Calls service with correct ID
- Handles edge cases (zero ID, negative ID)

**Total Controller Tests: 11**

## Test Patterns Used

### 1. Arrange-Act-Assert (AAA)
All tests follow the AAA pattern for clarity:
```csharp
// Arrange - Setup test data and mocks
var company = new Company { ... };

// Act - Execute the method being tested
var result = await _service.GetCompanyByIdAsync(1);

// Assert - Verify the results
result.Should().NotBeNull();
```

### 2. In-Memory Database (Service Tests)
Service tests use EF Core's in-memory database for realistic data access testing:
```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### 3. Mocking (Controller Tests)
Controller tests use Moq to isolate the controller from dependencies:
```csharp
var mockService = new Mock<ICompanyService>();
mockService.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(companies);
```

### 4. Fluent Assertions
Tests use FluentAssertions for readable and expressive assertions:
```csharp
result.Should().NotBeNull();
result.Should().HaveCount(3);
result.Should().BeEquivalentTo(expected);
```

## Writing New Tests

### Example: Adding a New Service Test
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var testData = new Company { ... };
    await _context.Companies.AddAsync(testData);
    await _context.SaveChangesAsync();

    // Act
    var result = await _companyService.YourMethod();

    // Assert
    result.Should().NotBeNull();
    // Add more assertions
}
```

### Example: Adding a New Controller Test
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    _mockCompanyService
        .Setup(s => s.YourMethod())
        .ReturnsAsync(expectedResult);

    // Act
    var result = await _controller.YourMethod();

    // Assert
    result.Result.Should().BeOfType<OkObjectResult>();
    // Add more assertions
}
```

## Best Practices

1. ✅ **One Assert Per Test** - Focus each test on a single behavior
2. ✅ **Descriptive Test Names** - Use `MethodName_Scenario_ExpectedBehavior` pattern
3. ✅ **Isolated Tests** - Each test should be independent
4. ✅ **Fast Tests** - Use in-memory databases and mocks for speed
5. ✅ **Readable Tests** - Use FluentAssertions for clear expectations
6. ✅ **Clean Up** - Dispose of resources properly (IDisposable pattern)

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# Example GitHub Actions
- name: Run Tests
  run: dotnet test --no-build --verbosity normal
```

## Code Coverage Goals

- **Minimum Coverage**: 80%
- **Target Coverage**: 90%+
- **Critical Paths**: 100%

## Troubleshooting

### Tests Fail with "Database Already Exists"
Solution: Each test uses a unique GUID for the database name to prevent conflicts.

### Moq Setup Not Working
Ensure you're using `.Object` when passing mocks to controllers:
```csharp
new CompaniesController(_mockService.Object)
```

### FluentAssertions Not Found
Make sure the package is installed:
```bash
dotnet add package FluentAssertions
```

## Future Test Additions

As the API grows, consider adding:
- [ ] Integration tests with a real test database
- [ ] Performance tests
- [ ] POST/PUT/DELETE endpoint tests
- [ ] Validation tests
- [ ] Authentication/Authorization tests
- [ ] Error handling tests

## Contributing

When adding new features:
1. Write tests first (TDD approach)
2. Ensure all tests pass
3. Maintain or improve code coverage
4. Follow existing test patterns and naming conventions

---

**Total Tests: 20**  
**Test Frameworks: xUnit, Moq, FluentAssertions**  
**Last Updated: 2024**
