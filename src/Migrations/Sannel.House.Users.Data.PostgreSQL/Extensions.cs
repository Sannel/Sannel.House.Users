using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql.EntityFrameworkCore;

namespace Sannel.House.Users.Data.PostgreSQL
{
	public static class Extensions
	{
		public static DbContextOptionsBuilder ConfigurePostgreSQL(this DbContextOptionsBuilder option, string connectionString)
		=> option.UseNpgsql(connectionString, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
	}
}
