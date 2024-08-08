
using Microsoft.EntityFrameworkCore;
using Hangfire;

namespace ScheduleJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICurrencyRepo, CurrencyRepo>();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //app.Environment.IsEnvironment("UAT")
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            MapsterConfiguration.Configuration();

            app.UseHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
