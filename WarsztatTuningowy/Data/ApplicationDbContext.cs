using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<OrderPart> OrderParts { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Workstation> Workstations { get; set; }
        public DbSet<WorkstationAssignment> WorkstationAssignments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderPart>()
                .HasKey(op => new { op.OrderId, op.PartId });

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DefaultMechanic)
                .WithMany(e => e.AssignedOrders)
                .HasForeignKey(o => o.DefaultMechanicId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ServiceTask>()
                .HasOne(st => st.AssignedEmployee)
                .WithMany(e => e.ServiceTasks)
                .HasForeignKey(st => st.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .Property(o => o.TuningGoal)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Workstation>()
                .Property(w => w.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Workstation>().HasData(
                new Workstation { Id = 1, Name = "Podnośnik 1", Type = WorkstationType.Lift, IsOccupied = false },
                new Workstation { Id = 2, Name = "Hamownia", Type = WorkstationType.Dyno, IsOccupied = false },
                new Workstation { Id = 3, Name = "Diagnostyka", Type = WorkstationType.Diagnostic, IsOccupied = false },
                new Workstation { Id = 4, Name = "Detailing", Type = WorkstationType.Detailing, IsOccupied = false },
                new Workstation { Id = 5, Name = "Spawalnia", Type = WorkstationType.Welding, IsOccupied = false }
            );

            modelBuilder.Entity<PartRequest>()
                .Property(pr => pr.Status)
                .HasConversion<string>();
        }
    }
}
