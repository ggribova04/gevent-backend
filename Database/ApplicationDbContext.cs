using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventStatus> EventStatuses { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<EventGuest> EventGuests { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskStatus> TaskStatuses { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users Table
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.UserName).IsUnique();
            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.IdRole)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Roles Table
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.IdRole);
            entity.HasMany(r => r.Users)
                  .WithOne(u => u.Role)
                  .HasForeignKey(u => u.IdRole)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(r => r.Organizations)
                  .WithOne(o => o.Role)
                  .HasForeignKey(o => o.IdRole)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Permissions Table
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(p => p.IdPermission);
        });

        // RolePermissions Table
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.IdRole, rp.IdPermission });
            entity.HasOne(rp => rp.Role)
                  .WithMany(r => r.RolePermissions)
                  .HasForeignKey(rp => rp.IdRole)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(rp => rp.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(rp => rp.IdPermission)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Event Table
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Status)
                  .WithMany(es => es.Events)
                  .HasForeignKey(e => e.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Tasks)
                  .WithOne(t => t.Event)
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.EventGuests)
                  .WithOne(eg => eg.Event)
                  .HasForeignKey(eg => eg.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Services)
                  .WithOne(s => s.Event)
                  .HasForeignKey(s => s.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Organizations)
                  .WithOne(o => o.Event)
                  .HasForeignKey(o => o.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // EventStatus Table
        modelBuilder.Entity<EventStatus>(entity =>
        {
            entity.HasKey(es => es.IdStatus); // Первичный ключ
        });

        // Organization Table
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(o => o.Id); // Первичный ключ
            entity.HasOne(o => o.User) // Внешний ключ на Users
                  .WithMany(u => u.Organizations)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(o => o.Event) // Внешний ключ на Events
                  .WithMany(e => e.Organizations)
                  .HasForeignKey(o => o.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(o => o.Role) // Внешний ключ на Roles
                  .WithMany(r => r.Organizations)
                  .HasForeignKey(o => o.IdRole)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // EventGuests Table
        modelBuilder.Entity<EventGuest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Event)
                  .WithMany(e => e.EventGuests)
                  .HasForeignKey(e => e.EventId);
        });

        // Task Table
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasOne(t => t.Event)
                  .WithMany(e => e.Tasks)
                  .HasForeignKey(t => t.EventId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Status)
                  .WithMany(ts => ts.Tasks)
                  .HasForeignKey(t => t.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(t => t.Employee)
                  .WithMany(u => u.Tasks)
                  .HasForeignKey(t => t.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskStatus Table
        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(ts => ts.IdStatus); // Первичный ключ
        });

        // Service Table
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(s => s.Id); // Первичный ключ
            entity.HasOne(s => s.Supplier) // Внешний ключ на Users
                  .WithMany(u => u.Services)
                  .HasForeignKey(s => s.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(s => s.Event) // Внешний ключ на Events
                  .WithMany(e => e.Services)
                  .HasForeignKey(s => s.EventId)
                  .OnDelete(DeleteBehavior.Restrict); // Удаление запрещено
            entity.HasOne(s => s.Status) // Внешний ключ на ServiceStatus
                  .WithMany(ts => ts.Services)
                  .HasForeignKey(s => s.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
