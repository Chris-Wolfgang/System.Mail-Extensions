# Contributing to Wolfgang.Extensions.Mail

Thank you for your interest in contributing to **Wolfgang.Extensions.Mail**! We welcome contributions to help improve this project.

## How Can You Contribute?

You can contribute in several ways:
- Reporting bugs
- Suggesting enhancements
- Submitting pull requests for new features or bug fixes
- Improving documentation
- Writing or improving tests

**Please note:** Before coding anything please check with me first by entering an issue and getting approval for it. PRs are more likely to get merged if I have agreed to the changes.

---

## Getting Started

1. **Fork the repository** and clone it locally.
2. **Create a new branch** for your feature or bug fix:
   ```sh
   git checkout -b your-feature-name
   ```
3. **Make your changes** and commit them with clear messages:
   ```sh
   git commit -m "Describe your changes"
   ```
4. **Push your branch** to your fork:
   ```sh
   git push origin your-feature-name
   ```
5. **Open a pull request** describing your changes.

6. **PR Checks:**
   Once you create a pull request (PR), several Continuous Integration (CI) steps will run automatically. These may include:
   - Building the project
   - Running automated tests
   - Checking code style and linting
   - Running static analysis

   **It is important to make sure that all CI steps pass before your PR can be merged.**
   - If any CI step fails, please review the error messages and update your PR as needed.
   - Maintainers will review your PR once all checks have passed.

---

## Code Quality Standards

This project aims to maintain **high code quality standards** through automated checks in the build and CI pipeline.

### Static Analysis and CI Enforcement

Code quality enforcement in this repository is based on the tools and checks that are actually configured in the project and workflow files.

Depending on the current repository configuration, pull requests may be validated with checks such as:

1. **.NET SDK analyzers and compiler warnings**
   - Correctness, reliability, and maintainability checks provided by the SDK
   - Warnings surfaced during restore/build

2. **Automated tests**
   - Validation that changes do not break expected behavior
   - Coverage and test execution requirements when configured

3. **Formatting, style, and linting rules**
   - Enforcement driven by repository configuration such as `.editorconfig`
   - Helps keep code consistent and reviewable

4. **CI and security scanning**
   - Additional checks may run through the configured GitHub Actions workflows
   - Contributors should treat the repository configuration and CI results as the source of truth for what is enforced

If a tool, analyzer, or banned API list is not configured in the repository, contributors should not assume it is enforced automatically.
Always check the current project files and workflow definitions when in doubt.

---

## Build and Test Instructions

### Prerequisites
- .NET 8.0 SDK or later
- PowerShell Core (optional, for formatting scripts)

### Build the Project

```bash
# Restore NuGet packages
dotnet restore

# Build in Release configuration
dotnet build --configuration Release
```

### Run Tests

```bash
# Run all unit tests
dotnet test --configuration Release

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Code Formatting

This project uses `.editorconfig` for consistent code style:

```bash
# Format all code
dotnet format

# Check formatting without changes (CI mode)
dotnet format --verify-no-changes

# PowerShell formatting script
pwsh ./scripts/format.ps1
```

---

## .editorconfig Rules

Key style rules enforced:

- **Indentation:** 4 spaces (C#), 2 spaces (XML/JSON)
- **Line endings:** LF (Unix-style)
- **Charset:** UTF-8
- **Trim trailing whitespace:** Yes
- **Final newline:** Yes
- **Braces:** New line style (Allman)
- **Naming:** PascalCase for public members, camelCase for parameters/locals
- **File-scoped namespaces:** Required in C# 10+
- **`var` preferences:** Use for built-in types and when type is obvious
- **Null checks:** Prefer pattern matching (`is null`, `is not null`)

View the complete configuration in [.editorconfig](.editorconfig).

---

## Guidelines

- Follow the coding style used in the project.
- Write clear, concise commit messages.
- Add relevant tests for new features or bug fixes.
- Document any public APIs with XML documentation comments.
- Ensure all analyzer warnings are addressed.
- Use async/await patterns - no blocking calls allowed.
- Include `CancellationToken` parameters in async methods where appropriate.

---

## Pull Requests

- Ensure your pull request passes all tests and analyzer checks.
- Respond to review feedback in a timely manner.
- Reference related issues in your pull request description.
- Keep changes focused and atomic - one feature/fix per PR.
- Update documentation if you change public APIs.

---

## Code of Conduct

Please be respectful and considerate in all interactions. See [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) for our community guidelines.

---

Thank you for contributing!
