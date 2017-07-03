// <copyright file="Startup.cs" company="natsnudasoft">
// Copyright (c) Adrian John Dunstan. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Natsnudasoft.RgbLedSequencerWebInterface
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Hangfire;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Natsnudasoft.NatsnudaLibrary;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Data;
    using Natsnudasoft.RgbLedSequencerWebInterface.Services;
    using static System.FormattableString;

    /// <summary>
    /// The Startup class for ASP.NET Core.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The hosting environment that is running the ASP.NET Core application.
        /// </param>
        public Startup(IHostingEnvironment env)
        {
            ParameterValidation.IsNotNull(env, nameof(env));

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(Invariant($"appsettings.{env.EnvironmentName}.json"), optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration of the ASP.NET Core applciation.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Gets the application container of the ASP.NET Core applciation.
        /// </summary>
        public IContainer ApplicationContainer { get; private set; }

        /// <summary>
        /// Configures the services for the ASP.NET Core application.
        /// </summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the
        /// container.
        /// </remarks>
        /// <param name="services">The services collection.</param>
        /// <returns>A custom service provider for the ASP.NET Core application to use.</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ParameterValidation.IsNotNull(services, nameof(services));

            var defaultConnectionString
                = this.Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(o
                => o.UseSqlServer(defaultConnectionString));

            services.AddMvc();
            services.AddMemoryCache();
            services.AddOptions();

            services.AddHangfire(c => c.UseSqlServerStorage(defaultConnectionString));

            var rgbLedSequencerConfigSection = this.Configuration.GetSection("RgbLedSequencer");
            var rgbLedSequencerConfig = services.AddRgbLedSequencerConfiguration(
                rgbLedSequencerConfigSection);

            services.AddRgbLedSequencer(rgbLedSequencerConfig);

            var builder = new ContainerBuilder();
            builder.RegisterType<RgbLedSequencerService>().As<IRgbLedSequencerService>();
            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        /// <summary>
        /// Configures the ASP.NET Core application.
        /// </summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request
        /// pipeline.
        /// </remarks>
        /// <param name="app">The ASP.NET Core application.</param>
        /// <param name="env">The hosting environment that is running the ASP.NET Core application.
        /// </param>
        /// <param name="loggerFactory">The injected logger factory for the ASP.NET Core
        /// application.</param>
        /// <param name="appLifetime">The application lifetime service for the ASP.NET Core
        /// application.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Maintainability",
            "CA1506",
            Justification = "Entry point allowed high coupling.")]
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime)
        {
            ParameterValidation.IsNotNull(app, nameof(app));
            ParameterValidation.IsNotNull(env, nameof(env));
            ParameterValidation.IsNotNull(loggerFactory, nameof(loggerFactory));
            ParameterValidation.IsNotNull(appLifetime, nameof(appLifetime));

            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            RecurringJob.AddOrUpdate<IRgbLedSequencerService>(
                "send-next-sequence",
                r => r.SendNextSequenceAsync(),
                Cron.MinuteInterval(5));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(this.CleanUp);
        }

        private void CleanUp()
        {
            if (this.ApplicationContainer.TryResolve(out ISerialPortAdapter serialPortAdapter))
            {
                if (serialPortAdapter.IsOpen)
                {
                    serialPortAdapter.Close();
                }
            }

            this.ApplicationContainer.Dispose();
        }
    }
}
