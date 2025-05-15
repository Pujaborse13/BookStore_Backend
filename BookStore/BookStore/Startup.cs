using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using ManagerLayer.Service;
using ManagerLayer.Interface;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RepositoryLayer.Helper;
using Microsoft.AspNetCore.Http;




namespace BookStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
           
            services.AddDbContext<BookStoreDBContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DBConn"]));

            // Register DI
            services.AddTransient<IUserRepo, UserRepo>();
            services.AddTransient<IUserManager, UserManager>();

            services.AddTransient<IAdminRepo, AdminRepo>();
            services.AddTransient<IAdminManager, AdminManager>();

            services.AddTransient<JwtTokenHelper>();

            services.AddTransient<IBookRepo, BookRepo>();
            services.AddTransient<IBookManager, BookManager>();

            services.AddTransient<ICartRepo, CartRepo>();
            services.AddTransient<ICartManager, CartManager>();

            services.AddTransient<IWishListRepo, WishListRepo>();
            services.AddTransient<IWishListManager, WishListManager>();


            //Swagger for API Documentation
            services.AddSwaggerGen(
                option =>
                {
                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" });
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter the valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"

                    });

                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                      {
                          new OpenApiSecurityScheme
                          {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }

                          },

                        new string[]{ }

                       }
                    });
                });



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var json = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                success = false,
                                message = "You are not logged in or Authorization token is missing. Please login ! "
                            });

                            return context.Response.WriteAsync(json);
                        }
                    };

                });

        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();


            app.UseSwagger();
            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
