﻿using AppCore.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace AppCore.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, string connectionString)
        {
            services.Configure<ApiBehaviorOptions>(lbda =>
            {
                lbda.SuppressModelStateInvalidFilter = true;
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            services.AddControllers().AddJsonOptions(lbda
                => lbda.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<AppCoreDbContext>(options => options
                .UseSqlServer(connectionString));

            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder =>
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("Production",
                    builder =>
                        builder
                            .WithMethods("GET")
                            .WithOrigins("http://apifornecedoresprodutos.api")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                            .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(Program));
        }

        public static void UseApiConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("Development");
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseCors("Development"); // Usar apenas nas demos => Configuração Ideal: Production
                app.UseHsts();//só aceita https (na primeira conexao com https)
            }

            app.UseHttpsRedirection();//redireciona para https

            app.UseAuthorization();

            app.MapControllers();
        }
    }
}