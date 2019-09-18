using AutoMapper;
using BlogDemo.Api.Extensions;
using BlogDemo.Core.interfaces;
using BlogDemo.Infrastructure.Database;
using BlogDemo.Infrastructure.Repository;
using BlogDemo.Infrastructure.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlogDemo.Api
{
    public class StartupDevelopment
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                options =>
                {
                    options.ReturnHttpNotAcceptable = true; //启用406状态码, 当使用Postman发送一个请求,并且将请求头里面的Accept值修改为 application/xml, 之请求xml格式的资源, 此时我们的api如果不支持xml格式就会返回406状态码, 下行代码启用xml

                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); //启用xml格式资源
                });
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

            //将创建的 MappingProfile 放到依赖注入容器中            
            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<IValidator<PostResource>, PostResourceValidator>();

            //使用IUrlHelper，生成前一夜后一页uri
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(
                factory =>
                {
                    var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
                    return new UrlHelper(actionContext);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            app.UseMyExceptionHandler(loggerFactory);//启用自定义的异常处理
            //顺序很重要，https重定向要在 UseMVC 之前
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
