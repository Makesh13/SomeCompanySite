using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SomeCompanySite.Domain;
using SomeCompanySite.Domain.Repositories.Abstract;
using SomeCompanySite.Domain.Repositories.EntityFramework;
using SomeCompanySite.Service;

namespace SomeCompanySite
{
    public class Startup
    {
        //Сюда забайндим конфигурацию из json с помощью вспомогательного класса Config;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            //Подключаем конфиг из appsettings.json(Project это имя объекта в файле)
            //Тоесть по сути, так как поляв экземпляре класса Config будут статик
            //Мы просто пишем в них поля из объекта json project
            Configuration.Bind("Project", new Config());

            //Подключаем кучу того что понаписали(в основном все для базы данных) в качестве сервисов
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            //Подключаем контекст базы
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Config.ConnectionString));

            services.AddIdentity<IdentityUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


            //настраиваем autentication cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;//Значит что кука будет недоступна на стороне клиента
                options.LoginPath = "/account/login";//Путь авторизации
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            //Добавляем  поддержку контроллеров и представлений(MVC)
            services.AddControllersWithViews(x =>
                {
                    //Это нам нужно для того чтобы работала проверка на админа в классе AdminAreaAuthorization, он лежит в папке services загляни туда
                    //Если в маршруте будет запрос, к Admin, то вот как раз будет логика выполняться из класса, а второй параметр это полиси, сверху настраивается
                    x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
                })
                //выставляем совместимость с асп нет 3.0
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Подключаем поддержку статических файлов в приложении (css, js и т.д)
            app.UseStaticFiles();
            app.UseRouting();

            //Конвейр для авторизации
            //Должен быть подключен после роутинга, но до ендпоинтов
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //Регистрируем маршруты
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
