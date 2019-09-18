using BlogDemo.Core.Entities;
using BlogDemo.Core.interfaces;
using BlogDemo.Infrastructure.Database;
using BlogDemo.Infrastructure.Extensions;
using BlogDemo.Infrastructure.Resources;
using BlogDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Infrastructure.Repository
{
    public class PostRepository:IPostRepository
    {
        private readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public PostRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public void AddPost(Post post)
        {
            _myContext.Posts.Add(post);
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _myContext.Posts.FindAsync(id);
        }

        public async Task<PaginatedList<Post>> GetPostsAsync(PostParameters postParameters)
        {
            var query = _myContext.Posts.AsQueryable();
            // 客户端有可能根据标题 Title 查询    
            if (!string.IsNullOrEmpty(postParameters.Title))
            {
                string title = postParameters.Title.ToLowerInvariant();
                query = query.Where(p => p.Title.Contains(title));
            }
            //query = query.OrderBy(p => p.Id);
            query = query.ApplySort(postParameters.OrderBy, _propertyMappingContainer.Resolve<PostResource, Post>());

            int count = await query.CountAsync();

            var data = await query
                .Skip((postParameters.PageIndex - 1) * postParameters.PageSize)
                .Take(postParameters.PageSize).ToListAsync();

            return new PaginatedList<Post>(postParameters.PageIndex, postParameters.PageSize, count, data);
        }

        
    }
}
