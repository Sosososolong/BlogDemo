using BlogDemo.Core.interfaces;
using BlogDemo.Infrastructure.Database;
using BlogDemo.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlite("Data Source=BlogDemo.db");
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 5001;
            });

            //Scope: 每次Http请求会创建一个实例,线程内唯一对象
            //Transient: 每次其他的类请求(不是指HTTP Request)都会创建一个新实例,它比较适合轻量级的无状态的(Stateless)的service.
            //Singleton: 在第一次创建的时候就会创建一个实例, 以后也只有这一个实例; 或者在ConfigureServices这段代码的时候创建唯一一个实例
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            //顺序很重要，https重定向要在 UseMVC 之前
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
