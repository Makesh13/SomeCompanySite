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
        //���� ��������� ������������ �� json � ������� ���������������� ������ Config;
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            //���������� ������ �� appsettings.json(Project ��� ��� ������� � �����)
            //������ �� ����, ��� ��� ����� ���������� ������ Config ����� ������
            //�� ������ ����� � ��� ���� �� ������� json project
            Configuration.Bind("Project", new Config());

            //���������� ���� ���� ��� ����������(� �������� ��� ��� ���� ������) � �������� ��������
            services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();
            services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
            services.AddTransient<DataManager>();

            //���������� �������� ����
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


            //����������� autentication cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myCompanyAuth";
                options.Cookie.HttpOnly = true;//������ ��� ���� ����� ���������� �� ������� �������
                options.LoginPath = "/account/login";//���� �����������
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
            });
            services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
            });

            //���������  ��������� ������������ � �������������(MVC)
            services.AddControllersWithViews(x =>
                {
                    //��� ��� ����� ��� ���� ����� �������� �������� �� ������ � ������ AdminAreaAuthorization, �� ����� � ����� services ������� ����
                    //���� � �������� ����� ������, � Admin, �� ��� ��� ��� ����� ������ ����������� �� ������, � ������ �������� ��� ������, ������ �������������
                    x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
                })
                //���������� ������������� � ��� ��� 3.0
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddSessionStateTempDataProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //���������� ��������� ����������� ������ � ���������� (css, js � �.�)
            app.UseStaticFiles();
            app.UseRouting();

            //������� ��� �����������
            //������ ���� ��������� ����� ��������, �� �� ����������
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            //������������ ��������
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("admin", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
