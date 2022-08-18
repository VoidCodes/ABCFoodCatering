using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ABCFoodCatering.Models;
using Microsoft.AspNetCore.Identity;

namespace ABCFoodCatering
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Method to create roles
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            // Add Custom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Client", "Member" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                // Create role and seed them to database
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create (Member) user
            var memberuser = new IdentityUser
            {
                UserName = Configuration.GetSection("UserSettings")["MemberEmail"],
                Email = Configuration.GetSection("UserSettings")["MemberEmail"]
            };

            string MemberPass = Configuration.GetSection("UserSettings")["MemberPassword"];
            var _member = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["MemberEmail"]);
            if (_member == null)
            {
                var createMemberUser = await UserManager.CreateAsync(memberuser, MemberPass);
                if (createMemberUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(memberuser, "Member");
                }
            }

            // Create (Client) user
            var clientuser = new IdentityUser
            {
                UserName = Configuration.GetSection("UserSettings")["ClientEmail"],
                Email = Configuration.GetSection("UserSettings")["ClientEmail"]
            };
            
            string ClientPass = Configuration.GetSection("UserSettings")["ClientPassword"];
            var _client = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["ClientEmail"]);
            if (_client == null)
            {
                var createClientUser = await UserManager.CreateAsync(clientuser, ClientPass);
                if (createClientUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(clientuser, "Client");
                }
            }

            // Create superuser (Admin) to have full access
            // NOT NEEDED AS ADMIN IS ABLE TO DO / VIEW EVERYTHING INCLUDING THE DETAILS IN THE DATABASE.
            /*var superuser = new IdentityUser
            {
                UserName = Configuration.GetSection("UserSettings")["AdminEmail"],
                Email = Configuration.GetSection("UserSettings")["AdminEmail"]
            };

            string AdminPass = Configuration.GetSection("UserSettings")["AdminPassword"];
            var _admin = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["AdminEmail"]);
            if (_admin == null)
            {
                var createAdminUser = await UserManager.CreateAsync(superuser, AdminPass);
                if (createAdminUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(superuser, "Admin");
                }
            }*/
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(opts =>
            {
                // Add default auth scheme and JWT auth type with standard setting
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.IncludeErrorDetails = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["Auth:Jwt:Issuer"],
                    ValidAudience = Configuration["Auth:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Äuth:Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true
                };
            });

            services.AddMvc(cfg =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                cfg.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
            CreateRoles(serviceProvider).Wait();
        }
    }
}
