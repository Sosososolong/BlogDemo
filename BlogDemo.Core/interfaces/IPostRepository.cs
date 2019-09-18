using BlogDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Core.interfaces
{
    //Repository模式 是为了使 Controller和DbContext不要绑定耦合到一起, 在Repository中操作DbContext, Controller使用Repository
    public interface IPostRepository
    {
        Task<PaginatedList<Post>> GetPostsAsync(PostParameters postParameters);
        Task<Post> GetPostByIdAsync(int id);
        void AddPost(Post post);
    }
}
