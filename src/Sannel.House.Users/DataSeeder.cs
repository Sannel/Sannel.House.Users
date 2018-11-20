/* Copyright 2018 Sannel Software, L.L.C.
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
	   http://www.apache.org/licenses/LICENSE-2.0
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sannel.House.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sannel.House.Users
{
	public class DataSeeder
	{
		private IConfiguration configuration;
		private IServiceProvider provider;
		private ILogger logger;

		public DataSeeder(IConfiguration configuration,
			IServiceProvider provider,
			ILogger<DataSeeder> logger)
		{
			this.configuration = configuration;
			this.provider = provider;
			this.logger = logger;
		}

		public async void SeedData()
		{
			using (var scope = provider.CreateScope())
			{
				var configContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
				var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();

				var apiSection = configuration.GetSection("Db:DbSeed:ApiResources");
				var apiList = apiSection.Get<List<ApiResource>>();
				foreach (var api in apiList)
				{
					if (configContext.ApiResources.Count(i => i.Name == api.Name) == 0)
					{
						logger.LogDebug("Adding api resource {0}", api.Name);
						await configContext.ApiResources.AddAsync(api);
					}
				}

				await configContext.SaveChangesAsync();

				var clientsSection = configuration.GetSection("Db:DbSeed:Clients");
				await addClients(configContext, clientsSection);

				await configContext.SaveChangesAsync();

				var rolesSection = configuration.GetSection("Db:DbSeed:Roles");
				await addRoles(roleManager, rolesSection);

				var usersSection = configuration.GetSection("Db:DbSeed:Users");
				await addUsers(userManager, usersSection);
			}
		}

		private async Task addClients(ConfigurationDbContext configContext, IConfigurationSection section)
		{
			var clientList = section.Get<List<Client>>();

			foreach(var client in clientList)
			{
				if(configContext.Clients.Count(i => i.ClientId == client.ClientId) == 0)
				{
					if(client.ClientSecrets != null)
					{
						foreach(var cs in client.ClientSecrets)
						{
							if (cs.Type == "EncodeSharedSecret")
							{
								cs.Value = IdentityServer4.Models.HashExtensions.Sha256(cs.Value);
								cs.Type = "SharedSecret";
							}
						}
					}
					logger.LogDebug("Adding client {0}", client.ClientName);
					await configContext.Clients.AddAsync(client);
				}
			}
		}

		private async Task addRoles(RoleManager<IdentityRole> roleManager, IConfigurationSection section)
		{
			var roles = section.Get<List<string>>();
			foreach(var r in roles)
			{
				if(await roleManager.FindByNameAsync(r) == null)
				{
					logger.LogDebug("Adding role {0}", r);
					await roleManager.CreateAsync(new IdentityRole(r));
				}
			}
		}

		private async Task addUsers(UserManager<IdentityUser> userManager, IConfigurationSection section)
		{
			foreach(var child in section.GetChildren())
			{
				var config = child.Get<UserConfiguration>();

				if(await userManager.FindByEmailAsync(child.Key) == null)
				{
					logger.LogDebug("Adding user {0}", child.Key);
					var user = new IdentityUser()
					{
						Email = child.Key,
						UserName = child.Key
					};

					var uresult = await userManager.CreateAsync(user, config.Password);

					if (uresult.Succeeded)
					{
						if (config.Roles != null)
						{
							var result = await userManager.AddToRolesAsync(user, config.Roles);
							if (result.Succeeded)
							{
								logger.LogDebug("Successfully added user {0} to Roles {1}", user.Email, string.Join(',', config.Roles));
							}
							else
							{
								logger.LogError("Error adding user {0} to Roles {1}, Errors: {2}",
									user.Email,
									string.Join(',', config.Roles),
									string.Join(',', result.Errors.Select(i => i.Description)));
							}
						}
					}
					else
					{
						logger.LogError("Error adding user {0}, Errors: {1}", user.Email,
							string.Join(',', uresult.Errors.Select(i => i.Description)));
					}
				}
			}
		}
	}
}
