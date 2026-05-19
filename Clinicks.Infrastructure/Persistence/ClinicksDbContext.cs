using System;
using System.Collections.Generic;
using Clinicks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Infrastructure.Persistence;

public partial class ClinicksDbContext : DbContext
{
    public ClinicksDbContext()
    {
    }

    public ClinicksDbContext(DbContextOptions<ClinicksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Paciente> Pacientes { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<EstadoCama> EstadosCama { get; set; }
    public virtual DbSet<Pais> Paises { get; set; }
    public virtual DbSet<Provincia> Provincias { get; set; }
    public virtual DbSet<Ciudad> Ciudades { get; set; }
    public virtual DbSet<Direccion> Direcciones { get; set; }
    public virtual DbSet<Habitacion> Habitaciones { get; set; }
    public virtual DbSet<Cama> Camas { get; set; }
    public virtual DbSet<Internacion> Internaciones { get; set; }
    public virtual DbSet<MovimientoCama> MovimientosCama { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Dni).HasName("PK__Paciente__D87608A6A9A48A2E");

            entity.ToTable("Paciente");

            entity.Property(e => e.Dni)
                .ValueGeneratedNever()
                .HasColumnName("dni");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Paciente>().HasQueryFilter(p => p.Activo);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuario__2ED7D2AF6D9861A0");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ__Usuario__AB6E6164028903F3").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Rol).HasColumnName("rol");
        });

        modelBuilder.Entity<EstadoCama>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK_EstadoCama");
            entity.ToTable("EstadoCama");
            entity.Property(e => e.IdEstado).HasColumnName("id_estado").ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre")
                .IsRequired();

            entity.HasData(
                new EstadoCama { IdEstado = 1, Nombre = "Libre" },
                new EstadoCama { IdEstado = 2, Nombre = "Ocupado" }
            );
        });

        modelBuilder.Entity<Pais>(entity =>
        {
            entity.HasKey(e => e.IdPais).HasName("PK__Pais");
            entity.ToTable("Pais");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.IdProvincia).HasName("PK__Provincia");
            entity.ToTable("Provincia");
            entity.Property(e => e.IdProvincia).HasColumnName("id_provincia");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.PaisNavigation).WithMany(p => p.Provincias)
                .HasForeignKey(d => d.IdPais)
                .HasConstraintName("FK__Provincia__id_pa");
        });

        modelBuilder.Entity<Ciudad>(entity =>
        {
            entity.HasKey(e => e.IdCiudad).HasName("PK__Ciudad");
            entity.ToTable("Ciudad");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.IdProvincia).HasColumnName("id_provincia");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.ProvinciaNavigation).WithMany(p => p.Ciudades)
                .HasForeignKey(d => d.IdProvincia)
                .HasConstraintName("FK__Ciudad__id_provi");
        });

        modelBuilder.Entity<Direccion>(entity =>
        {
            entity.HasKey(e => e.IdDireccion).HasName("PK__Direccion");
            entity.ToTable("Direccion");
            entity.Property(e => e.IdDireccion).HasColumnName("id_direccion");
            entity.Property(e => e.Altura).HasColumnName("altura");
            entity.Property(e => e.Calle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.IdCiudad).HasColumnName("id_ciudad");
            entity.Property(e => e.Dni).HasColumnName("dni");

            entity.HasOne(d => d.CiudadNavigation).WithMany(p => p.Direcciones)
                .HasForeignKey(d => d.IdCiudad)
                .HasConstraintName("FK__Direccion__id_ci");

            entity.HasOne(d => d.PacienteNavigation).WithMany(p => p.Direcciones)
                .HasForeignKey(d => d.Dni)
                .HasConstraintName("FK_Direccion_Paciente");
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.HasKey(e => e.IdHabitacion).HasName("PK__Habitacion");
            entity.ToTable("Habitacion");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cama>(entity =>
        {
            entity.HasKey(e => new { e.NCama, e.IdHabitacion }).HasName("PK__Cama");
            entity.ToTable("Cama");
            entity.Property(e => e.NCama).HasColumnName("n_cama");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.IdEstado)
                .HasColumnName("id_estado")
                .HasDefaultValue(1);

            entity.HasOne(d => d.HabitacionNavigation).WithMany(p => p.Camas)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__Cama__id_habitac");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Camas)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("FK_Cama_EstadoCama");
        });

        modelBuilder.Entity<Internacion>(entity =>
        {
            entity.HasKey(e => e.IdInternacion).HasName("PK__Internacion");
            entity.ToTable("Internacion");
            entity.Property(e => e.IdInternacion).HasColumnName("id_internacion");
            entity.Property(e => e.Dni).HasColumnName("dni");
            entity.Property(e => e.FechaEgreso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_egreso");
            entity.Property(e => e.FechaIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ingreso");

            entity.HasOne(d => d.PacienteNavigation).WithMany(p => p.Internaciones)
                .HasForeignKey(d => d.Dni)
                .HasConstraintName("FK__Internacion__dni");
        });

        modelBuilder.Entity<MovimientoCama>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__MovimientoCama");
            entity.ToTable("MovimientoCama");
            
            entity.Property(e => e.IdMovimiento).HasColumnName("id_movimiento");
            entity.Property(e => e.IdInternacion).HasColumnName("id_internacion");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.NCama).HasColumnName("n_cama");
            
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");

            entity.HasOne(d => d.InternacionNavigation).WithMany(p => p.MovimientosCama)
                .HasForeignKey(d => d.IdInternacion)
                .HasConstraintName("FK__MovimientoCama__id_internacion");

            entity.HasOne(d => d.HabitacionNavigation).WithMany()
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__MovimientoCama__id_habitacion")
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CamaNavigation).WithMany(p => p.MovimientosCama)
                .HasForeignKey(d => new { d.NCama, d.IdHabitacion })
                .HasConstraintName("FK__MovimientoCama__cama")
                .OnDelete(DeleteBehavior.ClientSetNull);
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
