using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rest_Api_A3.Clients;
using Rest_Api_A3.Helpers;
using Rest_Api_A3.Models.Database;
using Rest_Api_A3.Repositories;
using Rest_Api_A3.Repositories.Interfaces;
using Rest_Api_A3.Services.Implementations;
using Rest_Api_A3.Services.Interfaces;
using Supabase;

namespace Rest_Api_A3
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
            SqlMapper.AddTypeHandler(new UriTypeHandler()); // Register custom handler to map URI columns to System.Uri in Dapper

            services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins", builder =>
                {
                    builder.AllowAnyMethod()
                           .AllowAnyHeader()
                           .SetIsOriginAllowed(_ => true);
                });
            });

            services.Configure<ConnectionString>(Configuration.GetSection("ConnectionStrings"));

            services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            })
            .AddSignInManager()
            .AddUserStore<DapperUserStore>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            // addsup, Supabase 
            services.Configure<SupabaseOptions>(Configuration.GetSection("Supabase"));
            services.AddSingleton(sp =>
            {
                var cfg = sp.GetRequiredService<IOptions<SupabaseOptions>>().Value;
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = false
                };

                var client = new Client(cfg.Url, cfg.ApiKey, options);
                client.InitializeAsync().GetAwaiter().GetResult(); 
                return client;
            });
            services.AddSingleton<IStorageService, SupabaseStorageService>();

            // MVC and AutoMapper
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IActorRepository, ActorRepository>();
            services.AddScoped<IProducerRepository, ProducerRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IActorService, ActorService>();
            services.AddScoped<IProducerService, ProducerService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IMovieService, MovieService>();

            // Default CORS policy
            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


