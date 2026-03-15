using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineJudge.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Problem> Problem { get; set; } = default!;
    public DbSet<TestCase> TestCases { get; set; }
    public DbSet<Submission> Submissions { get; set; }
}
