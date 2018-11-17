using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sannel.House.Users.Data.MySql
{
	public class PersistedGrantDesignTimeFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
	{
		public PersistedGrantDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();

			builder.UseMySql("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;", o =>
			o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new PersistedGrantDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());
		}
	}
}
