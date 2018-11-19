using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BangazonWorkforce.Models;

public class WorkforceContext : DbContext
{
    public WorkforceContext(DbContextOptions<WorkforceContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employee { get; set; }

    public DbSet<Department> Department { get; set; }

    public DbSet<TrainingProgram> Training { get; set; }

    public DbSet<EmployeeTraining> EmployeeTraining { get; set; }
}