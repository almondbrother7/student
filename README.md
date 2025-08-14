# Students API

A simple ASP.NET Core Web API for managing students.  
Supports CRUD operations, model validation, with both in-memory implementation and JSON-backed persistence.

## Features
- **.NET 8 Minimal API** with clean separation of concerns
- **IStudentRepository** interface with both in-memory and JSON-backed implementations
- **Validation** via Data Annotations
- **Integration tests** using `WebApplicationFactory<Program>`
- **Unit tests** for repository and model validation
- **Smoke test script** for quick local verification

## Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) [Visual Studio Code](https://code.visualstudio.com/) or Visual Studio 2022
- Git

## Getting Started

### Clone the repository
```bash
git clone https://github.com/almondbrother7/student.git
cd student
```

## Running the API
```bash
# Default run:
dotnet run

# Run it on pinned dev ports:
dotnet run --urls "http://localhost:5069;https://localhost:7062"

# Run with JSON-backed repository:
STUDENTS_REPO=json STUDENTS_PATH="App_Data/students.json" \
dotnet run --urls "http://localhost:5069;https://localhost:7062"

# Run with in-memory repository (default if STUDENTS_REPO not set):
dotnet run --urls "http://localhost:5069;https://localhost:7062"
