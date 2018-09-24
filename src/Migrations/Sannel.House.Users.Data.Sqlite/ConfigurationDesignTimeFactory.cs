using Microsoft.EntityFrameworkCore.Design;
using IdentityServer4.EntityFramework.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Users.Data.Sqlite
{
	public class ConfigurationDesignTimeFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
	{
		public ConfigurationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();

			builder.UseSqlite("Data Source=data/data.sqlite", o =>
			o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new ConfigurationDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions());
		}
	}
}
