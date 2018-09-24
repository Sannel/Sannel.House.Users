using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sannel.House.Users.Data.SqlServer
{
	public class PersistedGrantDesignTimeFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
	{
		public PersistedGrantDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();

			builder.UseSqlServer("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=m;", o => o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new PersistedGrantDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());
		}
	}
}
