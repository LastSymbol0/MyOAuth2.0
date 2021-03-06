using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using AuthServer.Mediator;
using AuthServer.Mediator.Commands;
using AuthServer.Mediator.Queries;
using AuthServer.Domain.AggregatesModel.SessionAggregate;

using AuthServer.Infrastructure.Repository;
//using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<TokenManager>();

            services.AddAutoMapper(typeof(Startup));

            string dbConnection = Configuration.GetConnectionString("SessionsDB");

            services.AddDbContext<SessionsContext>(opt => opt.UseMySql(dbConnection));
            services.AddScoped<ISessionRepository,SessionRepository>();

            services.AddMediator()
                .AddCommand<StartSessionCommand, StartSessionCommandHandler, SessionCommandResponce>()
                .AddCommand<AuthenticateClientCommand, AuthenticateClientCommandHandler, SessionCommandResponce>()
                .AddCommand<RefreshClientTokenCommand, RefreshClientTokenCommandHandler, SessionCommandResponce>()
                .AddQuery<GetTokenPairQuery, GetTokenPairQueryHandler, GetTokenPairQueryDTO>()
                .AddQuery<GetClientAuthCodeQuery, GetClientAuthCodeQueryHandler, string>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TokenManager.TokenIssuer,

                        ValidateAudience = true,
                        ValidAudience = TokenManager.TokenAudience,

                        ValidateLifetime = true,

                        IssuerSigningKey = TokenManager.GetSymetricKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddAuthorization();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
