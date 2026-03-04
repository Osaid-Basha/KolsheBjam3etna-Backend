using KolsheBjam3etna.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace KolsheBjam3etna.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {


        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<ServiceRequestAttachment> ServiceRequestAttachments => Set<ServiceRequestAttachment>();
        public DbSet<ProductAd> ProductAds => Set<ProductAd>();
        public DbSet<ProductAdImage> ProductAdImages => Set<ProductAdImage>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<SwapAd> SwapAds => Set<SwapAd>();

        public DbSet<SwapAdImage> SwapAdImages => Set<SwapAdImage>();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<Conversation>()
       .HasOne(c => c.User1)
       .WithMany()
       .HasForeignKey(c => c.User1Id)
       .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
    }
