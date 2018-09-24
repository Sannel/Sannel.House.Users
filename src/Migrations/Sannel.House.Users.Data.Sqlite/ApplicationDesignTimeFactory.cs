using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.Sqlite
{
	public class ApplicationDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

			builder.UseSqlite("Data Source=data/data.sqlite", o => o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new ApplicationDbContext(builder.Options);
		}
	}
}
