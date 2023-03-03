using System;
using Microsoft.EntityFrameworkCore;
namespace ApiHRM
{
    public class EmployeeDb : DbContext
    {
        public EmployeeDb(DbContextOptions<EmployeeDb> options) : base(options) { }
        public DbSet<Employee> Employees => Set<Employee>();
    } 
}
