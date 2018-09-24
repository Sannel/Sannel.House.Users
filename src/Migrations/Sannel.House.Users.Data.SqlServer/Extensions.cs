using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.SqlServer
{
	public static class Extensions
	{
		public static DbContextOptionsBuilder ConfigureSqlServer(this DbContextOptionsBuilder option, string connectionString)
		=> option.UseSqlServer(connectionString, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
	}
}
