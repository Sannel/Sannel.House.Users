using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.MySql
{
	public class ApplicationDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

			builder.UseMySql("Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;", o =>
				o.MigrationsAssembly(GetType().Assembly.GetName().FullName));

			return new ApplicationDbContext(builder.Options);
		}
	}
}
