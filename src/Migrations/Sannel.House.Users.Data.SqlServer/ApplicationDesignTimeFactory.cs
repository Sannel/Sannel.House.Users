using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.SqlServer
{
	public class ApplicationDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

			builder.UseSqlServer("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=m;", o => o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new ApplicationDbContext(builder.Options);
		}
	}
}
