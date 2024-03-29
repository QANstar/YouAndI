﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YouAndI_Entity
{
    public partial class Youandi_DBContext : DbContext
    {
        public Youandi_DBContext()
        {
        }

        public Youandi_DBContext(DbContextOptions<Youandi_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityTag> ActivityTag { get; set; }
        public virtual DbSet<ApplyActivity> ApplyActivity { get; set; }
        public virtual DbSet<ApplyStatus> ApplyStatus { get; set; }
        public virtual DbSet<ChartMessage> ChartMessage { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<RootTag> RootTag { get; set; }
        public virtual DbSet<StarActivity> StarActivity { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Type> Type { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserInformation> UserInformation { get; set; }
        public virtual DbSet<UserTag> UserTag { get; set; }
        public virtual DbSet<View_Activity> View_Activity { get; set; }
        public virtual DbSet<View_ApplyActivity> View_ApplyActivity { get; set; }
        public virtual DbSet<View_ApplyStatus> View_ApplyStatus { get; set; }
        public virtual DbSet<View_ChartMessage> View_ChartMessage { get; set; }
        public virtual DbSet<View_Comment> View_Comment { get; set; }
        public virtual DbSet<View_StarActivity> View_StarActivity { get; set; }
        public virtual DbSet<View_User> View_User { get; set; }
        public virtual DbSet<View_UserTag> View_UserTag { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityTag>(entity =>
            {
                entity.HasOne(d => d.activity)
                    .WithMany(p => p.ActivityTag)
                    .HasForeignKey(d => d.activityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityTag_Activity");
            });

            modelBuilder.Entity<ChartMessage>(entity =>
            {
                entity.HasKey(e => e.message_id)
                    .HasName("PK_chart_message");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.comment_id)
                    .HasName("PK_comment");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.activityId)
                    .HasName("PK_Payment_1");

                entity.Property(e => e.activityId).ValueGeneratedNever();

                entity.HasOne(d => d.activity)
                    .WithOne(p => p.Payment)
                    .HasForeignKey<Payment>(d => d.activityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Activity");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.userid)
                    .HasName("PK_student");

                entity.Property(e => e.userid).ValueGeneratedNever();
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasOne(d => d.root)
                    .WithMany(p => p.Tag)
                    .HasForeignKey(d => d.rootId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Tag_RootTag");
            });

            modelBuilder.Entity<UserInformation>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();
            });

            modelBuilder.Entity<View_Activity>(entity =>
            {
                entity.ToView("View_Activity");
            });

            modelBuilder.Entity<View_ApplyActivity>(entity =>
            {
                entity.ToView("View_ApplyActivity");
            });

            modelBuilder.Entity<View_ApplyStatus>(entity =>
            {
                entity.ToView("View_ApplyStatus");
            });

            modelBuilder.Entity<View_ChartMessage>(entity =>
            {
                entity.ToView("View_ChartMessage");
            });

            modelBuilder.Entity<View_Comment>(entity =>
            {
                entity.ToView("View_Comment");
            });

            modelBuilder.Entity<View_StarActivity>(entity =>
            {
                entity.ToView("View_StarActivity");
            });

            modelBuilder.Entity<View_User>(entity =>
            {
                entity.ToView("View_User");
            });

            modelBuilder.Entity<View_UserTag>(entity =>
            {
                entity.ToView("View_UserTag");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}