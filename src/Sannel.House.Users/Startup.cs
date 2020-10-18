/* Copyright 2019-2020 Sannel Software, L.L.C.
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sannel.House.Users.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sannel.House.Users.Data.Sqlite;
using Sannel.House.Users.Data.SqlServer;
using Sannel.House.Users.Data.PostgreSQL;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Sannel.House.Base.Data;
using Sannel.House.Base.Web;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace Sannel.House.Users
{
	/// <summary>
	/// 
	/// </summary>
	public class Startup
	{
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

			var connectionString = Configuration.GetWithReplacement("Db:ConnectionString");
			if(string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("Db:ConnectionString", "Db:ConnectionString is required");
			}

			services.AddDbContext<ApplicationDbContext>(options => {
				switch (Configuration["Db:Provider"]?.ToLowerInvariant())
				{
					case "sqlserver":
						options.ConfigureSqlServer(connectionString);
						break;
					case "mysql":
						//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
						throw new NotSupportedException("We are currently not supporting mysql as a db provider");

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

			var identityBuilder = services.AddIdentityServer(i =>
			{
				// documents for adding more options: http://docs.identityserver.io/en/latest/reference/options.html
				if(!string.IsNullOrWhiteSpace(Configuration["IdentityServer:IssuerUri"]))
				{
					i.IssuerUri = Configuration["IdentityServer:IssuerUri"];
				}

				if(!string.IsNullOrWhiteSpace(Configuration["IdentityServer:AccessTokenJwtType"]))
				{
					i.AccessTokenJwtType = Configuration["IdentityServer:AccessTokenJwtType"];
				}
			})
				.AddConfigurationStore(options =>
				{
					options.ConfigureDbContext = options =>
					{
						switch (Configuration["Db:Provider"]?.ToLowerInvariant())
						{
							case "sqlserver":
								options.ConfigureSqlServer(connectionString);
								break;
							case "mysql":
								//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
								throw new NotSupportedException("We are currently not supporting mysql as a db provider");

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
						switch (Configuration["Db:Provider"]?.ToLowerInvariant())
						{
							case "sqlserver":
								options.ConfigureSqlServer(connectionString);
								break;
							case "mysql":
								//options.ConfigureMySql(Configuration["Db:ConnectionString"]);
								throw new NotSupportedException("We are currently not supporting mysql as a db provider");

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

			switch (Configuration["IdentityServer:SigningCredentialType"]?.ToLowerInvariant())
			{
				case "ecdsa":
					var bytes = Convert.FromBase64String(Configuration["IdentityServer:ECDsa:Key"]);

					var ecdsa = ECDsa.Create();
					ecdsa.ImportECPrivateKey(bytes, out _);

					var securityKey = new ECDsaSecurityKey(ecdsa) {KeyId = Configuration["IdentityServer:ECDsa:KeyId"]};

					identityBuilder.AddSigningCredential(securityKey, IdentityServerConstants.ECDsaSigningAlgorithm.ES256);
					break;
				case "x509":
				default:
					identityBuilder.AddSigningCredential(
						new X509Certificate2(
							Configuration["IdentityServer:X509:Path"],
							Configuration["IdentityServer:X509:Password"]));
					break;
			}

			services.AddScoped<DataSeeder>();
			services.AddHealthChecks()
				.AddDbHealthCheck<ApplicationDbContext>("ApplicationDb", async (c) =>
				{
					await c.Users.AnyAsync();
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider, ILogger<Startup> logger)
		{
			provider.CheckAndInstallTrustedCertificate();
			var p = Configuration["Db:Provider"];
			{
				var db = provider.GetService<ApplicationDbContext>();

				if(db is null)
				{
					throw new Exception("Unable to get ApplicationDbContext from service provider");
				}

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

				if(db is null)
				{
					throw new Exception("Unable to get ConfigurationDbContext from service provider");
				}

				db.Database.Migrate();
			}
			{
				var db = provider.GetService<PersistedGrantDbContext>();

				if(db is null)
				{
					throw new Exception("Unable to get PersistedGrantDbContext from service provider");
				}

				db.Database.Migrate();
			}

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				//app.UseHsts();
			}

			//app.UseHttpsRedirection();

			app.UseIdentityServer();

			app.UseRouting();

			app.UseCors();

			var pOrigin = Configuration["IdentityServer:PublicOrigin"];
			if (Uri.TryCreate(pOrigin, UriKind.Absolute, out var publicOrigin))
			{
				// add public origin back
				app.Use((context, next) =>
				{
					context.Request.Scheme = publicOrigin.Scheme;
					if (publicOrigin.Port != 80 && publicOrigin.Port != 443)
					{
						context.Request.Host = new HostString(publicOrigin.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped));
					}
					else
					{
						context.Request.Host = new HostString(publicOrigin.Host);
					}
					return next();
				});
			}
			else if(logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("Public Origin is not set or is incorrect {PublicOrigin}", Configuration["IdentityServer:PublicOrigin"]);
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHouseRobotsTxt();
				endpoints.MapHouseHealthChecks("/health");
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

			if (Configuration.GetValue<bool>("Db:SeedDb"))
			{
				var s = provider.GetService<DataSeeder>();

				if(s is null)
				{
					throw new Exception("Unable to get DataSeeder from service provider");
				}

				s.SeedData();
			}

		}
	}
}
