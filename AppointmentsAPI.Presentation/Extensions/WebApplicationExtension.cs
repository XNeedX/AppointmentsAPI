public static class WebApplicationExtension
{
    public static void UseMiddlewarePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
                {
                    { "prompt", "login" }
                });
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
    }
}