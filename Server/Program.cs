
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MiniCourseDbContext>(options => options.UseSqlite("Data Source=./Data/MiniCourseDB.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a very simple Post model.");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapGet("/posts/all", async (MiniCourseDbContext dbContext) =>
{
    List<Post> allPosts = await dbContext.Posts.ToListAsync();
    return allPosts;
});

app.MapGet("/posts/by-id/{postId}", async (int postId, MiniCourseDbContext dbContext) =>
{
    Post? post = await dbContext.Posts.FindAsync(postId);

    if (post != null)
    {
        return Results.Ok(post);
    }
    else
    {
        return Results.NotFound();
    }
});

app.MapPost("/posts/create", async (Post postToCreate, MiniCourseDbContext dbContext) =>
{
    dbContext.Posts.Add(postToCreate);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/posts/by-id/{postToCreate.PostId}", postToCreate);
});

app.MapPut("/posts/update/{postId}", async (int postId, Post updatedPost, MiniCourseDbContext dbContext) =>
{
    var postToUpdate = await dbContext.Posts.FindAsync(postId);

    if (postToUpdate == null)
    {
        return Results.NotFound();
    }

    postToUpdate.Title = updatedPost.Title;
    postToUpdate.Content = updatedPost.Content;

    await dbContext.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/posts/delete/{postId}", async (int postId, MiniCourseDbContext dbContext) =>
{
    Post? postToDelete = await dbContext.Posts.FindAsync(postId);

    if (postToDelete != null)
    {
        dbContext.Posts.Remove(postToDelete);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }
});

app.Run();