using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sannel.House.Users.Data.MySql
{
	public static class Extensions
	{
		public static DbContextOptionsBuilder ConfigureMySql(this DbContextOptionsBuilder option, string connectionString)
		=> option.UseMySql(connectionString, i => i.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
	}
}
