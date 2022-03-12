using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ImageUrlFormatter
{
    public static class ImageUrlFormatter
    {
        public static void UseImageUrlFormatter(this IServiceCollection services)
        {
            services.AddMvc(options => { options.Filters.Add<ImageUrlFormatterFilter>(); });
        }
    }
}