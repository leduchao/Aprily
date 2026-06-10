### Migration with Package Manager:

```bash
Add-Migration <MigrationName> -o Database/Migrations
Update-Database
```

### Migration with dotnet CLI:

```bash
dotnet ef migrations add <MigrationName> -o Database/Migrations
dotnet ef database update
```
