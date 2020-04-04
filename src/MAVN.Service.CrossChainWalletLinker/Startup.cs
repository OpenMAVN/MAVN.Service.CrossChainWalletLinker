using JetBrains.Annotations;
using Lykke.Sdk;
using MAVN.Service.CrossChainWalletLinker.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using AutoMapper;

namespace MAVN.Service.CrossChainWalletLinker
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "CrossChainWalletLinker API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "CrossChainWalletLinkerLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.CrossChainWalletLinkerService.Db.LogsConnString;
                };

                options.Extend = (sc, settings) =>
                {
                    sc.AddAutoMapper(typeof(AutoMapperProfile));
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
