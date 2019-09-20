using FluentValidation;
namespace BlogDemo.Infrastructure.Resources
{
    public class PostAddOrUpdateResourceValidator<T> : AbstractValidator<T> where T : PostAddOrUpdateResource
    {
        public PostAddOrUpdateResourceValidator()
        {
            RuleFor(postResource => postResource.Title)
                .NotNull()
                .WithName("标题")
                .WithMessage("required|{PropertyName}是必填的") //因为前段需要错误类型，所以这里在错误信息里面加入了错误类型，然后自定义方法 返回验证失败的信息
                .MaximumLength(50)
                .WithMessage("maxlength|{PropertyName}的最大长度是{MaxLength}");

            RuleFor(postResource => postResource.Body)
                .NotNull()
                .WithName("正文")
                .WithMessage("required|{PropertyName}是必填的")
                .MinimumLength(100)
                .WithMessage("minlength|{PropertyName}的最小长度是{MinLength}");
        }
    }
}
