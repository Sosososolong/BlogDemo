using BlogDemo.Core.Entities;
using BlogDemo.Infrastructure.Database.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.Database
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 在新建的 PostConfiguration.cs 中配置Post表字段的一些属性，例如长度范围，约束。。 然后添加迁移即可应用到数据库中
            // 注意sqlite不支持修改字段
            modelBuilder.ApplyConfiguration(new PostConfiguration());
        }

        public DbSet<Post> Posts { get; set; }
    }
}
