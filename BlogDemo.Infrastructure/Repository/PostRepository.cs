using BlogDemo.Core.Entities;
using BlogDemo.Core.interfaces;
using BlogDemo.Infrastructure.Database;
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

        public PostRepository(MyContext myContext)
        {
            _myContext = myContext;
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
            var query = _myContext.Posts.OrderBy(p => p.Id);

            int count = await query.CountAsync();

            var data = await query
                .Skip((postParameters.PageIndex - 1) * postParameters.PageSize)
                .Take(postParameters.PageSize).ToListAsync();

            return new PaginatedList<Post>(postParameters.PageIndex, postParameters.PageSize, count, data);
        }

        
    }
}
