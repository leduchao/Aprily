### Scaffold databse with db-first approach:

```bash
dotnet ef dbcontext scaffold \
    "Host=localhost;Port=5432;Database=aprily-db;Username=postgres;Password=postgres" Npgsql.EntityFrameworkCore.PostgreSQL \
    -o Entities \
    --context AppDbContext2 \
    --context-dir Database \
    --no-onconfiguring \
    --force
```
