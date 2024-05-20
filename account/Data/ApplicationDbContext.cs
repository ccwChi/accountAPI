using account.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace account.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public DbSet<Account> Accounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyUtcDateTimeConverter();
			modelBuilder.Entity<Account>(entity =>
			{
				entity.Property(e => e.Balance)
					.HasColumnType("decimal(18, 2)");
				// Configure one-to-many relationship
				entity.HasMany(a => a.Transactions)
					  .WithOne(t => t.Account)
					  .HasForeignKey(t => t.AccountId);
			});

			modelBuilder.Entity<Transaction>(entity =>
			{
				entity.Property(e => e.Amount)
					.HasColumnType("decimal(18, 2)");
			});

		}
	}
}
