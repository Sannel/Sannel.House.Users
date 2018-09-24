using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.Sqlite
{
	public static class Extensions
	{
		public static DbContextOptionsBuilder ConfigureSqlite(this DbContextOptionsBuilder option, string connectionString)
		=> option.UseSqlite(connectionString, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
	}
}
