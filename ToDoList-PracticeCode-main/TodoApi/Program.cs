using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// הוספת DbContext לשירותים עם החיבור מ-`appsettings.json`
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
        new MySqlServerVersion(new Version(8, 0, 21))));

// הוספת שירות CORS (כדי לאפשר גישה מכל מקור)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// הוספת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הפעלת Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// הפעלת CORS
app.UseCors("AllowAll");

// נתיב ברירת מחדל מפנה ל-Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// // Route לשליפת כל המשימות
app.MapGet("/tasks", async (MyDbContext dbContext) =>
{
    try
    {
        Console.WriteLine("Fetching tasks from database...");
        var tasks = await dbContext.Tasks.ToListAsync();
        Console.WriteLine($"Fetched {tasks.Count} tasks.");
        return Results.Ok(tasks);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error retrieving tasks: {ex.Message}");
        return Results.Problem($"Error retrieving tasks: {ex.Message}");
    }
});


// Route להוספת משימה חדשה
app.MapPost("/tasks", async (MyDbContext dbContext, ToDoTask task) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(task.Name))
        {
            return Results.BadRequest("Task name is required.");
        }

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/tasks/{task.Id}", task);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error adding task: {ex.Message}");
        return Results.Problem("An error occurred while adding the task.");
    }
})
.WithName("AddTask");

// Route לעדכון משימה
app.MapPut("/tasks/{id}", async (MyDbContext dbContext, int id, ToDoTask updatedTask) =>
{
    try
    {
        var existingTask = await dbContext.Tasks.FindAsync(id);
        if (existingTask == null)
        {
            return Results.NotFound("Task not found.");
        }

        if (string.IsNullOrWhiteSpace(updatedTask.Name))
        {
            return Results.BadRequest("Task name is required.");
        }

        existingTask.Name = updatedTask.Name;
        existingTask.IsComplete = updatedTask.IsComplete;

        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error updating task: {ex.Message}");
        return Results.Problem("An error occurred while updating the task.");
    }
})
.WithName("UpdateTask");

// Route למחיקת משימה
app.MapDelete("/tasks/{id}", async (MyDbContext dbContext, int id) =>
{
    try
    {
        var task = await dbContext.Tasks.FindAsync(id);
        if (task == null)
        {
            return Results.NotFound("Task not found.");
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error deleting task: {ex.Message}");
        return Results.Problem("An error occurred while deleting the task.");
    }
})
.WithName("DeleteTask");

// הפעלת האפליקציה
app.Run();

// מחלקת ה-DbContext
public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<ToDoTask> Tasks { get; set; }
}

// מודל המשימה
[Table("tabl")]
public class ToDoTask
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}




