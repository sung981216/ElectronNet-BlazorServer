using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElectronServerBlazorEf.Data;
using ElectronNET.API;
using ElectronNET.API.Entities;
using ElectronServerBlazorEf.NW;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ElectronServerBlazorEf
{
    public class Startup
    {
        private void CreateMenu () {
            bool isMac = RuntimeInformation.IsOSPlatform (OSPlatform.OSX);
            MenuItem[] menu = null;

            MenuItem[] appMenu = new MenuItem[] {
                new MenuItem { Role = MenuRole.about },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.services },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.hide },
                new MenuItem { Role = MenuRole.hideothers },
                new MenuItem { Role = MenuRole.unhide },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.quit }
            };

            MenuItem[] fileMenu = new MenuItem[] {
                new MenuItem {
                    Label = "Save As...", Type = MenuType.normal, Click = async () => {
                        var mainWindow = Electron.WindowManager.BrowserWindows.First ();
                        var options = new SaveDialogOptions () {
                            Filters = new FileFilter[] {
                            new FileFilter { Name = "CSV Files", Extensions = new string[] { "csv" } }
                            }
                        };
                        string result = await Electron.Dialog.ShowSaveDialogAsync (mainWindow, options);
                        if (!string.IsNullOrEmpty (result)) {
                            result = System.Web.HttpUtility.UrlEncode(result);
                            string url = $"http://localhost:{BridgeSettings.WebPort}/saveas/{result}";
                            mainWindow.LoadURL(url);
                        }
                    }
                },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = isMac ? MenuRole.close : MenuRole.quit }
            };

            MenuItem[] viewMenu = new MenuItem[] {
                new MenuItem { Role = MenuRole.reload },
                new MenuItem { Role = MenuRole.forcereload },
                new MenuItem { Role = MenuRole.toggledevtools },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.resetzoom },
                new MenuItem { Role = MenuRole.zoomin },
                new MenuItem { Role = MenuRole.zoomout },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.togglefullscreen }
            };

            if (isMac) {
                menu = new MenuItem[] {
                new MenuItem { Label = "Electron", Type = MenuType.submenu, Submenu = appMenu },
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu }
                };
            } else {
                menu = new MenuItem[] {
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu }
                };
            }

            Electron.Menu.SetApplicationMenu (menu);
        }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async void ElectronBootstrap() {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Width = 1152,
                Height = 940,
                Show = false
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("Electron.NET with Blazor!");
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<NorthwindService>();
            services.AddScoped<System.Net.Http.HttpClient>();
            services.AddDbContext<NorthwindContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("NW"));
            });

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                // necessary when routing parameter includes a .
                endpoints.MapFallbackToPage("/saveas/{filepath?}", "/_Host");
            });

            if (HybridSupport.IsElectronActive) {
                CreateMenu();
                ElectronBootstrap();
            }

        }
    }
}
