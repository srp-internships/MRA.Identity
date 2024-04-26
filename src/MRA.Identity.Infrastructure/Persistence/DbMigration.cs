using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MRA.Identity.Infrastructure.Persistence;
public class DbMigration(ApplicationDbContext context, IConfiguration configuration)
{
    public async Task InitialiseAsync()
    {
        if (configuration["Environment"] == "Staging")
            await context.Database.EnsureDeletedAsync();

        if (context.Database.IsSqlServer())
            await context.Database.MigrateAsync();
    }
}