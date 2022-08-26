using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;
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
            string[] roleNames = { "Client", "Member", "Admin" };
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

            // Create (Client) user -- This user is allowed to do CRUD Ops and access the api
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

            // Create (Admin) user -- This user is allowed to do EVERYTHING
            var superuser = new IdentityUser
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
            }
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthorizationHandlerContext>

            services.Configure<JWTSettings>(Configuration.GetSection("JwtSettings"));

            services.AddAuthentication(opts =>
            {
                // Add default auth scheme and JWT auth type with standard setting
                // The code below causes authentication issues
                /*opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;*/
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.IncludeErrorDetails = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudience = Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"])),
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true
                };
            });

            // Add Google Authentication
            services.AddAuthentication().AddGoogle(opts =>
            {
                opts.ClientId = Configuration["GoogleSettings:ClientID"];
                opts.ClientSecret = Configuration["GoogleSettings:ClientSecret"];
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
            };

            // Use custom HTTP error page message
            /*app.UseStatusCodePages(async ctx =>
            {
                if (ctx.HttpContext.Response.StatusCode == 401)
                {
                    //var role = RoleManager.FindByNameAsync("Client");
                    await ctx.HttpContext.Response.WriteAsync("HTTP 401 ERROR: Authorization Failed");
                }
            });*/

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            /*app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());*/

            app.UseAuthentication();
            //app.UseAuthorization();
            app.UseMvc();
            CreateRoles(serviceProvider).Wait();
        }
    }
}
