using Microsoft.EntityFrameworkCore.Design;
using IdentityServer4.EntityFramework.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Sannel.House.Users.Data.MySql
{
	public class ConfigurationDesignTimeFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
	{
		public ConfigurationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();

			builder.UseMySql("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;", o =>
			o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new ConfigurationDbContext(builder.Options, new IdentityServer4.EntityFramework.Options.ConfigurationStoreOptions());
		}
	}
}
