using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Core.interfaces;
using BlogDemo.Infrastructure.Database;
using BlogDemo.Infrastructure.Extensions;
using BlogDemo.Infrastructure.Resources;
using BlogDemo.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Controllers
{
    [Route("api/posts")]
    public class PostController:Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public PostController(
            IPostRepository postRepository, 
            IUnitOfWork unitOfWork, 
            ILoggerFactory loggerFactory, 
            IConfiguration configuration, 
            IMapper mapper,
            IUrlHelper urlHelper,
            ITypeHelperService typeHelperService,
            IPropertyMappingContainer propertyMappingContainer)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger("BlogDemo.Api.Controllers.PostController");
            _configuration = configuration;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _typeHelperService = typeHelperService;
            _propertyMappingContainer = propertyMappingContainer;
        }

        [HttpGet(Name = "GetPosts")]
        public async Task<IActionResult> Get(PostParameters postParameters, [FromHeader(Name = "Accept")] string mediaType)
        {
            //验证排序属性
            if (!_propertyMappingContainer.ValidateMappingExistsFor<PostResource, Post>(postParameters.OrderBy))
            {
                return BadRequest("Cann't finds for sorting.");
            }
            //验证塑形字段
            if (!_typeHelperService.TypeHasProperties<PostResource>(postParameters.Fields))
            {
                return BadRequest("Fields not exist");
            }

            var postList = await _postRepository.GetPostsAsync(postParameters);
            var postResources = _mapper.Map<IEnumerable<Post>, IEnumerable<PostResource>>(postList);

            if (mediaType == "application/vnd.cgzl.hateoas+json")
            {
                //塑形
                var shapedPostResources = postResources.ToDynamicIEnumerable(postParameters.Fields);

                var shapedWithLinks = shapedPostResources.Select(
                    p =>
                    {
                        var dict = p as IDictionary<string, object>;
                        var postLinks = CreateLinksForPost((int)dict["Id"], postParameters.Fields);
                        dict.Add("links", postLinks);
                        return dict;
                    });

                var links = CreateLinksForPosts(postParameters, postList.HasPrevious, postList.HasNext);

                var result = new
                {
                    value = shapedWithLinks,
                    links
                };


                //生成上一页下一页的链接
                //var previousPageLink = postList.HasPrevious ?
                //    CreatePostUri(postParameters, PaginationResourceUriType.PreviousPage) : null;
                //var nextPageLink = postList.HasNext ?
                //    CreatePostUri(postParameters, PaginationResourceUriType.NextPage) : null;

                var meta = new
                {
                    PageIndex = postList.PageIndex,
                    PageSize = postList.PageSize,
                    TotalItemsCount = postList.TotalItemsCount,
                    PageCount = postList.PageCount,
                    //属性名不写，默认和值名字相同
                    //previousPageLink,
                    //nextPageLink
                };
                //将分页的 元数据 从自定义的Headers里面返回去
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    //格式化对象的过程中，对象属性转换为“驼峰式”，即将 meta对象转换为json字符串的时候，首字母转为小写
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
                return Ok(result);
            }
            else
            {
                var previousPageLink = postList.HasPrevious ?
                    CreatePostUri(postParameters, PaginationResourceUriType.PreviousPage) : null;
                var nextPageLink = postList.HasNext ?
                    CreatePostUri(postParameters, PaginationResourceUriType.NextPage) : null;

                var meta = new
                {
                    postList.TotalItemsCount,
                    postList.PageSize,
                    postList.PageIndex,
                    postList.PageCount,
                    previousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));

                return Ok(postResources.ToDynamicIEnumerable(postParameters.Fields));
            }            
        }
        
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> Get(int id, string fields = null)
        {
            if (!_typeHelperService.TypeHasProperties<PostResource>(fields))
            {
                return BadRequest("Fields not exist.");
            }

            var post = await _postRepository.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var postResource = _mapper.Map<Post, PostResource>(post);
            //塑形
            var shapedPostResource = postResource.ToDynamic(fields);

            var links = CreateLinksForPost(id, fields);

            var result = shapedPostResource as IDictionary<string, object>;

            result.Add("links", links);

            return Ok(shapedPostResource);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Post newPost = new Post()
            {
                Author = "admin",
                Body = "ja;lkfj;alfk;la5464654654654",
                Title = "Title A",
                LastModified = DateTime.Now
            };

            _postRepository.AddPost(newPost);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        private string CreatePostUri(PostParameters parameters, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {                    
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", previousParameters);
                    
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", nextParameters);
                    
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                        //title = parameters.Title
                    };
                    return _urlHelper.Link("GetPosts", currentParameters);                    
            }
        }

        private IEnumerable<LinkResource> CreateLinksForPost(int id, string fields = null)
        {
            var links = new List<LinkResource>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetPost", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetPost", new { id, fields }), "self", "GET"));
            }

            links.Add(
                new LinkResource(
                    _urlHelper.Link("DeletePost", new { id }), "delete_post", "DELETE"));

            return links;
        }

        private IEnumerable<LinkResource> CreateLinksForPosts(PostParameters postResourceParameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResource>
            {
                new LinkResource(
                    CreatePostUri(postResourceParameters, PaginationResourceUriType.CurrentPage),
                    "self", "GET")
            };

            if (hasPrevious)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.PreviousPage),
                        "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.NextPage),
                        "next_page", "GET"));
            }

            return links;
        }
    }
}
