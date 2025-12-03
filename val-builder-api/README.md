# Val Builder API

## Features

- **ValHeader Management:**
  - Create, update, retrieve, and filter valuation headers.
- **ValDetail Auto-Generation:**
  - When a new ValHeader is created, ValDetails are automatically generated from ValTemplateItems where `DefaultOnVal == true`.
  - Only defaulted template items are copied.
  - All relevant attributes (Bold, Bullet, Indent, etc.) are copied from the template.
- **ValTemplateItem Management:**
  - CRUD operations and bulk display order updates.
- **ValAnnotation Support:**
  - CRUD operations
- **Company/Plan Integration:**
  - Filter ValHeaders by company or plan.
- **PDF Generation:**
  - Generate PDF documents for valuations using [PuppeteerSharp](https://github.com/hardkoded/puppeteer-sharp) along with [iText](https://itextpdf.com/).
  - PuppeteerSharp enables HTML-to-PDF conversion using a headless Chromium browser.
  - iText is used for direct PDF merging. AGPLv3 license
- **Robust Testing:**
  - Comprehensive unit tests using xUnit and FluentAssertions.
- **Entity Framework Core:**
  - Uses EF Core for ORM and data access, with in-memory database support for testing.
- **Dependency Injection:**
  - All services are injected for testability and maintainability.
- **SonarQube Integration:**
  - Static code analysis and quality checks using SonarQube/SonarCloud. See CI/CD workflow for details.
- **Code Coverage Reporting:**
  - Coverage reports generated using [coverlet](https://github.com/coverlet-coverage/coverlet), with scripts for Windows, Linux, and PowerShell.

## API Endpoints

- `/api/ValHeader`
  - `GET`: List all ValHeaders, filter by companyId or planId.
  - `GET /{id}`: Get a specific ValHeader.
  - `POST`: Create a new ValHeader (auto-generates ValDetails from default template items).
  - `PUT /{id}`: Update an existing ValHeader.
- `/api/ValDetails`
  - Standard CRUD for ValDetails.
- `/api/ValTemplateItems`
  - CRUD and bulk display order update for template items.
- `/api/ValAnnotations`
  - CRUD for annotations.
- `/api/CompanyPlan`, `/api/ValSections`, etc.
  - Additional endpoints for related entities.

## SonarQube Integration

- SonarQube/SonarCloud is integrated via GitHub Actions for static code analysis, code quality, and security checks.
- See `.github/workflows/build.yaml` for Sonar integration steps.
- Results are available in your Sonar dashboard.

## Code Coverage Reporting

- Coverage is measured using [coverlet](https://github.com/coverlet-coverage/coverlet).
- Scripts provided:
  - `generate-coverage.bat` (Windows)
  - `generate-coverage.sh` (Linux/macOS)
  - `generate-coverage.ps1` (PowerShell)
- Coverage exclusions and guides are in `CODE-COVERAGE-GUIDE.md` and `COVERAGE-EXCLUSIONS.md`.
- Reports are generated in `coverage.opencover.xml` and can be uploaded to SonarQube/SonarCloud.

## Running & Testing

1. **Build:**
   ```sh
   dotnet build
   ```
2. **Run:**
   ```sh
   dotnet run
   ```
3. **Test:**
   ```sh
   dotnet test
   ```
4. **Code Coverage:**
   Run the appropriate script for your OS:
   ```sh
   ./generate-coverage.sh
   # or
   generate-coverage.bat
   # or
   ./generate-coverage.ps1
   ```

## Contributing

- Fork and clone the repo.
- Create feature branches.
- Submit pull requests with clear descriptions.