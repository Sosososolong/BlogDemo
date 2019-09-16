using BlogDemo.Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Infrastructure.Database
{
    public class MyContextSeed
    {
        public static async Task SeedAsync(MyContext context, ILoggerFactory loggerFactory, int retry = 0)
        {
            int retryForAvailability = retry;
            try
            {
                //TODO: Only run this if using a real database
                //context.Database.Migrate()
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new List<Post>
                        {
                            new Post
                            {
                                Title = "Post Title 1",
                                Body = "Post Body 1",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 2",
                                Body = "Post Body 2",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 3",
                                Body = "Post Body 3",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 4",
                                Body = "Post Body 4",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 5",
                                Body = "Post Body 5",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 6",
                                Body = "Post Body 6",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 7",
                                Body = "Post Body 7",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            },
                            new Post
                            {
                                Title = "Post Title 8",
                                Body = "Post Body 8",
                                Author = "Dave",
                                LastModified = DateTime.Now
                            }
                        });
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var logger = loggerFactory.CreateLogger<MyContextSeed>();
                    logger.LogError(ex.Message);
                    await SeedAsync(context, loggerFactory, retryForAvailability);
                }                
            }
        }
    }
}
