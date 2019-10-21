/* Copyright 2019 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
	   http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sannel.House.Users.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sannel.House.Users.Data.Sqlite;
using Sannel.House.Users.Data.SqlServer;
using Sannel.House.Users.Data.PostgreSQL;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Stores;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using Sannel.House.Data;
using Sannel.House.Web;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace Sannel.House.Users
{
	/// <summary>
	/// 
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// The replacement regex
		/// </summary>
		public static Regex ReplacementRegex = new Regex(
			"\\$\\{(?<key>[a-zA-Z0-9_\\-:]+)\\}",
			RegexOptions.CultureInvariant
			| RegexOptions.Compiled
		);

		/// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		/// <summary>
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
			//	options.CheckConsentNeeded = context => true;
			//	options.MinimumSameSitePolicy = SameSiteMode.None;
			//});

			var connectionString = Configuration["Db:ConnectionString"];
			if(string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("Db:ConnectionString", "Db:ConnectionString is required");
			}

			connectionString = ReplacementRegex.Replace(connectionString, (Match target) =>
			{
				if(target.Groups.ContainsKey("key"))
				{
					var group = target.Groups["key"];
					return Configuration[group.Value];
				}

				return target.Value;
			});

			services.AddDbContext<ApplicationDbContext>(options => {
				switch (Configuration["Db:Provider"])
				{
					case "sqlserver":
					case "SqlServer":
						options.ConfigureSqlServer(connectionString);
						break;
					case "MySql":
					case "mysql":
						//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
						throw new NotSupportedException("We are currently not supporting mysql as a db provider");

					case "PostgreSQL":
					case "postgresql":
						options.ConfigurePostgreSQL(connectionString);
						break;

					case "sqlite":
					default:
						options.ConfigureSqlite(connectionString);
						break;
				}
			});
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddControllers();

			services.AddIdentityServer()
				.AddSigningCredential(
					new X509Certificate2(
						Configuration["IdentityServer:Certificate:Path"],
						Configuration["IdentityServer:Certificate:Password"]))
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = options =>
					{
						switch (Configuration["Db:Provider"])
						{
							case "sqlserver":
							case "SqlServer":
								options.ConfigureSqlServer(connectionString);
								break;
							case "MySql":
							case "mysql":
								//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
								throw new NotSupportedException("We are currently not supporting mysql as a db provider");

							case "PostgreSQL":
							case "postgresql":
								options.ConfigurePostgreSQL(connectionString);
								break;

							case "sqlite":
							default:
								options.ConfigureSqlite(connectionString);
								break;
						}
					};
				})
				.AddOperationalStore(options =>
				{
					options.ConfigureDbContext = options =>
					{
						switch (Configuration["Db:Provider"])
						{
							case "sqlserver":
							case "SqlServer":
								options.ConfigureSqlServer(connectionString);
								break;
							case "MySql":
							case "mysql":
								//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
								throw new NotSupportedException("We are currently not supporting mysql as a db provider");

							case "PostgreSQL":
							case "postgresql":
								options.ConfigurePostgreSQL(connectionString);
								break;

							case "sqlite":
							default:
								options.ConfigureSqlite(connectionString);
								break;
						}
					};

					options.EnableTokenCleanup = true;
					options.TokenCleanupInterval = 3600;
				})
				.AddAspNetIdentity<IdentityUser>();

			services.AddScoped<DataSeeder>();
			services.AddHealthChecks();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider, ILogger<Startup> logger)
		{
			provider.CheckAndInstallTrustedCertificate();
			var p = Configuration["Db:Provider"];
			{
				var db = provider.GetService<ApplicationDbContext>();

				if (/*string.Compare(p, "mysql", true) == 0
					||*/ string.Compare(p, "sqlserver", true) == 0
					|| string.Compare(p, "postgresql", true) == 0)
				{
					db.WaitForServer(logger);
				}

				db.Database.Migrate();
			}
			{
				var db = provider.GetService<ConfigurationDbContext>();
				db.Database.Migrate();
			}
			{
				var db = provider.GetService<PersistedGrantDbContext>();
				db.Database.Migrate();
			}

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
//				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHealthChecks("/health");

			app.UseHttpsRedirection();

			app.UseIdentityServer();

			app.UseRouting();

			app.UseCors();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

			if (Configuration.GetValue<bool>("Db:SeedDb"))
			{
				var s = provider.GetService<DataSeeder>();
				s.SeedData();
			}

		}
	}
}
