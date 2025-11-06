// ========================================
// קובץ: Program.cs
// מיקום: TodoApi/Program.cs
// תיאור: קובץ הראשי של ה-API Server
// ========================================

using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת DbContext ל-Services
builder.Services.AddDbContext<ToDoDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql")
    )
);

// הוספת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הוספת CORS (אם תרצי להתחבר מ-Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// הפעלת Swagger (רק בסביבת פיתוח)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// שימוש ב-CORS
app.UseCors("AllowAll");

// ===== ROUTES (API Endpoints) =====

// 1. GET - שליפת כל המשימות
app.MapGet("/items", async (ToDoDBContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

// 2. GET - שליפת משימה לפי ID
app.MapGet("/items/{id}", async (int id, ToDoDBContext db) =>
{
    var item = await db.Items.FindAsync(id);
    
    if (item == null)
        return Results.NotFound($"Item with ID {id} not found");
    
    return Results.Ok(item);
});

// 3. POST - הוספת משימה חדשה
app.MapPost("/items", async (Item newItem, ToDoDBContext db) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    
    return Results.Created($"/items/{newItem.Id}", newItem);
});

// 4. PUT - עדכון משימה
app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDBContext db) =>
{
    var item = await db.Items.FindAsync(id);
    
    if (item == null)
        return Results.NotFound($"Item with ID {id} not found");
    
    // עדכון השדות
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    
    await db.SaveChangesAsync();
    
    return Results.Ok(item);
});

// 5. DELETE - מחיקת משימה
app.MapDelete("/items/{id}", async (int id, ToDoDBContext db) =>
{
    var item = await db.Items.FindAsync(id);
    
    if (item == null)
        return Results.NotFound($"Item with ID {id} not found");
    
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    
    return Results.Ok($"Item with ID {id} was deleted");
});

// נתיב ברירת מחדל
app.MapGet("/", () => "Todo API is running! Use /items to access the API.");

app.Run();