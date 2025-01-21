using RazorLight;

namespace NotificationService.Application.Services
{
    /// <summary>
    /// Service for rendering Razor templates into HTML using RazorLight, which enables runtime compilation of Razor views.
    /// </summary>
    public class HtmlRenderService
    {
        private readonly RazorLightEngine _razorEngine;

        public HtmlRenderService()
        {
            // Using .UseFileSystemProject(AppContext.BaseDirectory) to load templates with build action set to Content,
            // located in the assembly's root path.
            // Use .UseEmbeddedResourcesProject for Embedded Resource templates instead.
            _razorEngine = new RazorLightEngineBuilder()
               .UseFileSystemProject(AppContext.BaseDirectory)
               .UseMemoryCachingProvider()
               .Build();
        }

        /// <summary>
        /// Renders a Razor template into HTML from a path relative to the running assembly's directory, where the template is located.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to pass to the Razor template.</typeparam>
        /// <param name="templateRelativePath">The relative path to the Razor template file.</param>
        /// <param name="model">The model to be passed to the template for rendering.</param>
        /// <returns>A string containing the rendered HTML.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the specified template file is not found.</exception>
        public async Task<string> RenderHtmlFromTemplatePathAsync<TModel>(string templateRelativePath, TModel model)
        {
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, templateRelativePath)))
            {
                throw new FileNotFoundException($"Template not found at path: {templateRelativePath}");
            }

            string htmlTemplate = await _razorEngine.CompileRenderAsync(templateRelativePath, model);
            return htmlTemplate;
        }

        /// <summary>
        /// Renders a Razor template into HTML from a string containing the template content. Use this method to support templates from 
        /// various sources such as files, databases, or dynamically provided content. It is particularly useful in dynamic scenarios, 
        /// including multi-tenant applications where tenant-specific templates may be rendered.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to pass to the Razor template.</typeparam>
        /// <param name="templateKey">An identifier for the template, used for caching and distinguishing between templates.</param>
        /// <param name="templateContent">The Razor template content.</param>
        /// <param name="model">The model to be passed to the template for rendering.</param>
        /// <returns>A string containing the rendered HTML.</returns>
        /// <exception cref="ArgumentException">Thrown if the template content is null or empty.</exception>
        public async Task<string> RenderHtmlFromTemplateContentAsync<TModel>(string templateKey, string templateContent, TModel model)
        {
            if (string.IsNullOrWhiteSpace(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            string htmlTemplate = await _razorEngine.CompileRenderStringAsync(templateKey, templateContent, model);
            return htmlTemplate;
        }
    }
}
