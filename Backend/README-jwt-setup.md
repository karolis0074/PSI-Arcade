# Read This Before Running the Backend

**JWT local setup is required — skipping this causes 500 errors on every endpoint.**

## Why this matters

The backend signs and validates JWT tokens using `Jwt:Key` and `Jwt:Issuer` from configuration (see `Program.cs` and `Utils/CryptService.cs`). These values are **not** committed to `appsettings.json` — each developer sets them locally with `dotnet user-secrets`.

If they are not set, `Jwt:Key` is `null`, and `Encoding.UTF8.GetBytes(...)` throws — every endpoint, including `/games`, returns **500**.

**Important:** `Jwt:Key` must be at least 32 characters long. A shorter key fails with `IDX10720` when generating a token.

## Setup (Windows / macOS / Linux)

1. Open a terminal.
2. Navigate to the backend project directory:
   ```bash
   cd PSI-Arcade/Backend
   ```
3. Initialize user secrets for the project (safe to run even if already initialized):
   ```bash
   dotnet user-secrets init
   ```
4. Set the JWT key (32+ characters):
   ```bash
   dotnet user-secrets set "Jwt:Key" "your-local-development-secret-key-32chars+"
   ```
5. Set the JWT issuer:
   ```bash
   dotnet user-secrets set "Jwt:Issuer" "SportMatch.API"
   ```
6. Verify the values were saved:
   ```bash
   dotnet user-secrets list
   ```

---

Note: the project already has a `UserSecretsId` set in `SportMatch.API.csproj`, so `dotnet user-secrets init` will just report it's already initialized — it's included above as a no-op safety step in case that ever changes.
