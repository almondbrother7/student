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

### Quick Start (Most Simple)
```bash
- 1. Start the API in memory mode:
dotnet run --project Students/Students.csproj
or
dotnet run --project Students/Students.csproj --launch-profile "Students (InMemory)"
or
dotnet run --project Students/Students.csproj --launch-profile "Students (JSON)"
- 2. Open Swagger UI:
https://localhost:7062/
- 3. Run the debug endpoint to see a 409 error:
https://localhost:7062/debug/ExceptionEmailAlreadyInUse
```

## Running the API

You can run the API using launch profiles **or** by setting environment variables directly.

### Option A — Launch profiles (simple)

**In-memory (ephemeral):**
```bash
dotnet run --project Students/Students.csproj --launch-profile "Students (InMemory)"
```

**JSON-backed (persistent):**
```bash
dotnet run --project Students/Students.csproj --launch-profile "Students (JSON)"
```

### Option B — No launch profiles (env vars)

**macOS/Linux (bash/zsh):**
```bash
# In-memory
STUDENTS_REPO=memory ASPNETCORE_URLS="http://localhost:5069;https://localhost:7062" dotnet run --project Students/Students.csproj

# JSON-backed
STUDENTS_REPO=json STUDENTS_PATH=./data/students.json ASPNETCORE_URLS="http://localhost:5069;https://localhost:7062" dotnet run --project Students/Students.csproj
```

**Windows PowerShell:**
```powershell
# In-memory
$env:STUDENTS_REPO="memory"; $env:ASPNETCORE_URLS="http://localhost:5069;https://localhost:7062"; dotnet run --project Students/Students.csproj

# JSON-backed
$env:STUDENTS_REPO="json"; $env:STUDENTS_PATH=".\\data\\students.json"; $env:ASPNETCORE_URLS="http://localhost:5069;https://localhost:7062"; dotnet run --project Students/Students.csproj
```

**Windows CMD:**
```cmd
REM In-memory
set STUDENTS_REPO=memory && set ASPNETCORE_URLS=http://localhost:5069;https://localhost:7062 && dotnet run --project Students/Students.csproj

REM JSON-backed
set STUDENTS_REPO=json && set STUDENTS_PATH=.\data\students.json && set ASPNETCORE_URLS=http://localhost:5069;https://localhost:7062 && dotnet run --project Students/Students.csproj
```

> **Note:** `dotnet run` uses `launchSettings.json` by default. If your first profile sets `STUDENTS_REPO=json`, you’ll start in JSON mode unless you use `--launch-profile` or `--no-launch-profile`.

## Testing the API
```bash
# 1. Using Swagger UI
https://localhost:7062/swagger/index.html

# 2. Using the shell script - a quick smoke-test script that exercises the basic CRUD endpoints
Scripts/test-students.sh

# 3. Triggering a sample 409 error - debug endpoint to test your DuplicateEmailException → ProblemDetails mapping
# This should return: HTTP 409 Conflict: application/problem+json body including title, detail, status, traceId, email, and existingId
https://localhost:7062/debug/ExceptionEmailAlreadyInUse
```

## First-time HTTPS setup (dev cert)

If your browser or VS Code shows HTTPS trust errors locally, initialize the ASP.NET Core **development certificate** (one-time per machine):

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Then restart VS Code and the app. On macOS you may be prompted by Keychain—approve to trust the cert.

**Verify**:
```bash
dotnet dev-certs https --check
```

If you still see HTTPS issues, try the HTTP URL (`http://localhost:5069`) or clear any cached HSTS entries in your browser.

### Clear HSTS cache (Chrome/Edge)

**Chrome**  
- Go to `chrome://net-internals/#hsts`. If unavailable in your version, clear site data instead.  
- Under **Delete domain security policies**, enter `localhost`, click **Delete**.  
- Or via settings: **Settings → Privacy & security → Site settings → View permissions and data stored across sites** → search `localhost` → **Clear data**.

**Edge**  
- Similar steps. You can try `edge://net-internals/#hsts` on many versions, or clear site data for `localhost` via **Settings → Privacy & security**.

