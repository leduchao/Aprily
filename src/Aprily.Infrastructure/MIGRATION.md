## Migration with Package Manager:
Please set the default project to `Aprily.Infrastructure` in the Package Manager Console before running the following commands:
```bash
Add-Migration <MigrationName> -o Database/Migrations
Update-Database
```

## Migration with dotnet CLI:
Please navigate to the `Aprily.Infrastructure` project in the terminal and run the following commands:
```bash
dotnet ef migrations add <MigrationName> -o Database/Migrations --startup-project ../Aprily.Api
dotnet ef database update --startup-project ../Aprily.Api
```
