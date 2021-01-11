using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using _2048_Rbu.Classes;

#nullable disable

namespace _2048_Rbu.Database
{
    public partial class DbRbuContext : DbContext
    {
        public DbRbuContext()
        {
        }

        public DbRbuContext(DbContextOptions<DbRbuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Batcher> Batchers { get; set; }
        public virtual DbSet<BatcherMaterial> BatcherMaterials { get; set; }
        public virtual DbSet<BatcherOpcParameter> BatcherOpcParameters { get; set; }
        public virtual DbSet<CommonOpcParameter> CommonOpcParameters { get; set; }
        public virtual DbSet<Container> Containers { get; set; }
        public virtual DbSet<ContainerType> ContainerTypes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DosingSource> DosingSources { get; set; }
        public virtual DbSet<DosingSourceMaterial> DosingSourceMaterials { get; set; }
        public virtual DbSet<DosingSourceOpcParameter> DosingSourceOpcParameters { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<MaterialType> MaterialTypes { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeGroup> RecipeGroups { get; set; }
        public virtual DbSet<RecipeMaterial> RecipeMaterials { get; set; }
        public virtual DbSet<RecipeMaterialType> RecipeMaterialTypes { get; set; }
        public virtual DbSet<RecipeType> RecipeTypes { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskQueueItem> TaskQueueItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                Dictionary<string, string> serviceDictionary = Service.GetInstance().GetOpcDict();

                if (serviceDictionary.ContainsKey("DbConnectionString"))
                {
                    optionsBuilder.UseNpgsql(serviceDictionary["DbConnectionString"]);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Russian_Russia.1251");

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batch");

                entity.HasIndex(e => e.ReportId, "IX_Batch_ReportId");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Batches)
                    .HasForeignKey(d => d.ReportId);
            });

            modelBuilder.Entity<BatcherMaterial>(entity =>
            {
                entity.ToTable("BatcherMaterial");

                entity.HasIndex(e => e.BatchId, "IX_BatcherMaterial_BatchId");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.BatcherMaterials)
                    .HasForeignKey(d => d.BatchId);
            });

            modelBuilder.Entity<BatcherOpcParameter>(entity =>
            {
                entity.HasIndex(e => e.BatcherId, "IX_BatcherOpcParameters_BatcherId");

                entity.HasOne(d => d.Batcher)
                    .WithMany(p => p.BatcherOpcParameters)
                    .HasForeignKey(d => d.BatcherId);
            });

            modelBuilder.Entity<Container>(entity =>
            {
                entity.HasIndex(e => e.ContainerTypeId, "IX_Containers_ContainerTypeId");

                entity.HasIndex(e => e.CurrentMaterialId, "IX_Containers_CurrentMaterialId");

                entity.HasOne(d => d.ContainerType)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.ContainerTypeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.CurrentMaterial)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.CurrentMaterialId);
            });

            modelBuilder.Entity<ContainerType>(entity =>
            {
                entity.HasIndex(e => e.MaterialTypeId, "IX_ContainerTypes_MaterialTypeId");

                entity.HasOne(d => d.MaterialType)
                    .WithMany(p => p.ContainerTypes)
                    .HasForeignKey(d => d.MaterialTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<DosingSource>(entity =>
            {
                entity.HasIndex(e => e.BatcherId, "IX_DosingSources_BatcherId");

                entity.HasIndex(e => e.ContainerId, "IX_DosingSources_ContainerId");

                entity.HasOne(d => d.Batcher)
                    .WithMany(p => p.DosingSources)
                    .HasForeignKey(d => d.BatcherId);

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.DosingSources)
                    .HasForeignKey(d => d.ContainerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<DosingSourceMaterial>(entity =>
            {
                entity.ToTable("DosingSourceMaterial");

                entity.HasIndex(e => e.BatcherMaterialId, "IX_DosingSourceMaterial_BatcherMaterialId");

                entity.HasOne(d => d.BatcherMaterial)
                    .WithMany(p => p.DosingSourceMaterials)
                    .HasForeignKey(d => d.BatcherMaterialId);
            });

            modelBuilder.Entity<DosingSourceOpcParameter>(entity =>
            {
                entity.HasIndex(e => e.DosingSourceId, "IX_DosingSourceOpcParameters_DosingSourceId");

                entity.HasOne(d => d.DosingSource)
                    .WithMany(p => p.DosingSourceOpcParameters)
                    .HasForeignKey(d => d.DosingSourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasIndex(e => e.MaterialTypeId, "IX_Materials_MaterialTypeId");

                entity.HasOne(d => d.MaterialType)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.MaterialTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasIndex(e => e.GroupId, "IX_Recipes_GroupId");

                entity.HasIndex(e => e.RecipeGroupId, "IX_Recipes_RecipeGroupId");

                entity.HasIndex(e => e.RecipeTypeId, "IX_Recipes_RecipeTypeId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.RecipeGroups)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.RecipeGroup)
                    .WithMany(p => p.RecipeRecipeGroups)
                    .HasForeignKey(d => d.RecipeGroupId);

                entity.HasOne(d => d.RecipeType)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.RecipeTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<RecipeMaterial>(entity =>
            {
                entity.HasIndex(e => e.MaterialId, "IX_RecipeMaterials_MaterialId");

                entity.HasIndex(e => e.RecipeId, "IX_RecipeMaterials_RecipeId");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.RecipeMaterials)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeMaterials)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RecipeMaterialType>(entity =>
            {
                entity.HasIndex(e => e.MaterialTypeId, "IX_RecipeMaterialTypes_MaterialTypeId");

                entity.HasIndex(e => e.RecipeTypeId, "IX_RecipeMaterialTypes_RecipeTypeId");

                entity.HasOne(d => d.MaterialType)
                    .WithMany(p => p.RecipeMaterialTypes)
                    .HasForeignKey(d => d.MaterialTypeId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.RecipeType)
                    .WithMany(p => p.RecipeMaterialTypes)
                    .HasForeignKey(d => d.RecipeTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasIndex(e => e.TaskId, "IX_Reports_TaskId");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.TaskId);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasIndex(e => e.CustomerId, "IX_Tasks_CustomerId");

                entity.HasIndex(e => e.RecipeId, "IX_Tasks_RecipeId");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.CustomerId);

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.RecipeId);
            });

            modelBuilder.Entity<TaskQueueItem>(entity =>
            {
                entity.HasIndex(e => e.TaskId, "IX_TaskQueueItems_TaskId");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskQueueItems)
                    .HasForeignKey(d => d.TaskId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
