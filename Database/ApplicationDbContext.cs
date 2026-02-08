using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<User> Users { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<Event> Events { get; set; }
  public DbSet<Organization> Organizations { get; set; }
  public DbSet<EventGuest> EventGuests { get; set; }
  public DbSet<Task> Tasks { get; set; }
  public DbSet<Tender> Tenders { get; set; }
  public DbSet<TenderResponse> TenderResponses { get; set; }

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
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Roles Table
    modelBuilder.Entity<Role>(entity =>
    {
      entity.HasKey(r => r.Id);
      entity.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.HasMany(r => r.Organizations)
            .WithOne(o => o.Role)
            .HasForeignKey(o => o.IdRole)
            .OnDelete(DeleteBehavior.Restrict);
    });


    // Event Table
    modelBuilder.Entity<Event>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
      entity.HasMany(e => e.Tasks)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.HasMany(e => e.EventGuests)
            .WithOne(eg => eg.Event)
            .HasForeignKey(eg => eg.EventId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.HasMany(e => e.Organizations)
            .WithOne(o => o.Event)
            .HasForeignKey(o => o.EventId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.HasMany(e => e.Tenders)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);
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
      entity.HasKey(eg => eg.Id);
      entity.HasOne(eg => eg.Event)
            .WithMany(e => e.EventGuests)
            .HasForeignKey(eg => eg.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    // Task Table
    modelBuilder.Entity<Task>(entity =>
    {
      entity.HasKey(t => t.Id);
      entity.HasOne(t => t.Event)
            .WithMany(e => e.Tasks)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
      entity.HasOne(t => t.Employee)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Tender Table
    modelBuilder.Entity<Tender>(entity =>
    {
      entity.HasKey(t => t.Id);
      entity.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
      entity.HasOne(t => t.Event)
            .WithMany(e => e.Tenders)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.HasMany(t => t.Responses)
            .WithOne(r => r.Tender)
            .HasForeignKey(r => r.TenderId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    // TenderResponse Table
    modelBuilder.Entity<TenderResponse>(entity =>
    {
      entity.HasKey(tr => tr.Id);
      entity.HasIndex(tr => new { tr.TenderId, tr.EmployeeId })
            .IsUnique();
      entity.HasOne(tr => tr.Employee)
            .WithMany(u => u.TenderResponses)
            .HasForeignKey(tr => tr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
      entity.Property(tr => tr.Status)
             .HasConversion<string>()
             .HasMaxLength(50)
             .IsRequired();
    });
  }
}
