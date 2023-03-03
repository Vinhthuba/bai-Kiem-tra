using Microsoft.EntityFrameworkCore;
using ApiHRM;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmployeeDb>(opt => opt.UseInMemoryDatabase("EmployeeList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/employeeitems", async (EmployeeDb db) =>
    await db.Employees.ToListAsync()
);

app.MapGet("/employeeitems/complete", async (EmployeeDb db) =>
    await db.Employees.Where(t => t.IsComplete).ToListAsync()
);


app.MapGet("/employeeitems/search/{name}", async (string name, EmployeeDb db) =>
{
    var employees = await db.Employees
        .Where(e => e.Name!.Contains(name))
        .ToListAsync();

    if (employees is null || employees.Count == 0) return Results.NotFound();

    return Results.Ok(employees);
});

app.MapGet("/employeeitems/{id}", async (int id, EmployeeDb db) =>
    await db.Employees.FindAsync(id)
        is Employee employee
            ? Results.Ok(employee)
            : Results.NotFound()
);

app.MapPost("/employeeitems", async (Employee employee, EmployeeDb db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();

    return Results.Created($"/employeeitems/{employee.Id}", employee);
});

app.MapPut("/employeeitems/{id}", async (int id, Employee inputEmployee, EmployeeDb db) =>
{
    var employee = await db.Employees.FindAsync(id);

    if (employee is null) return Results.NotFound();

    employee.Name = inputEmployee.Name;
    employee.IsComplete = inputEmployee.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/employeeitems/{id}", async (int id, EmployeeDb db) =>
{
    if (await db.Employees.FindAsync(id) is Employee employee)
    {
        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return Results.Ok(employee);
    }

    return Results.NotFound();
});

app.Run();