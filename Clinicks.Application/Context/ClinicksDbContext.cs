using System;
using System.Collections.Generic;
using Clinicks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinicks.Application.Context;

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
    public virtual DbSet<Pais> Paises { get; set; }
    public virtual DbSet<Provincia> Provincias { get; set; }
    public virtual DbSet<Ciudad> Ciudades { get; set; }
    public virtual DbSet<Direccion> Direcciones { get; set; }
    public virtual DbSet<Habitacion> Habitaciones { get; set; }
    public virtual DbSet<Cama> Camas { get; set; }
    public virtual DbSet<Internacion> Internaciones { get; set; }
    public virtual DbSet<Ingreso> Ingresos { get; set; }
    public virtual DbSet<Egreso> Egresos { get; set; }
    public virtual DbSet<Traslado> Traslados { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=clinicks_bd;Trusted_Connection=True;TrustServerCertificate=True;");

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
            entity.Property(e => e.Ocupado)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ocupado");

            entity.HasOne(d => d.HabitacionNavigation).WithMany(p => p.Camas)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__Cama__id_habitac");
        });

        modelBuilder.Entity<Internacion>(entity =>
        {
            entity.HasKey(e => e.IdInternacion).HasName("PK__Internacion");
            entity.ToTable("Internacion");
            entity.Property(e => e.IdInternacion).HasColumnName("id_internacion");
            entity.Property(e => e.Dni).HasColumnName("dni");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.NCama).HasColumnName("n_cama");

            entity.HasOne(d => d.PacienteNavigation).WithMany(p => p.Internaciones)
                .HasForeignKey(d => d.Dni)
                .HasConstraintName("FK__Internacion__dni");

            entity.HasOne(d => d.CamaNavigation).WithMany(p => p.Internaciones)
                .HasForeignKey(d => new { d.NCama, d.IdHabitacion })
                .HasConstraintName("FK__Internacion__cama");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasKey(e => e.IdIngreso).HasName("PK__Ingreso");
            entity.ToTable("Ingreso");
            entity.Property(e => e.IdIngreso).HasColumnName("id_ingreso");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdInternacion).HasColumnName("id_internacion");

            entity.HasOne(d => d.InternacionNavigation).WithMany(p => p.Ingresos)
                .HasForeignKey(d => d.IdInternacion)
                .HasConstraintName("FK__Ingreso__id_inte");
        });

        modelBuilder.Entity<Egreso>(entity =>
        {
            entity.HasKey(e => e.IdEgreso).HasName("PK__Egreso");
            entity.ToTable("Egreso");
            entity.Property(e => e.IdEgreso).HasColumnName("id_egreso");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdInternacion).HasColumnName("id_internacion");

            entity.HasOne(d => d.InternacionNavigation).WithMany(p => p.Egresos)
                .HasForeignKey(d => d.IdInternacion)
                .HasConstraintName("FK__Egreso__id_inter");
        });

        modelBuilder.Entity<Traslado>(entity =>
        {
            entity.HasKey(e => e.IdTraslado).HasName("PK__Traslado");
            entity.ToTable("Traslado");
            entity.Property(e => e.IdTraslado).HasColumnName("id_traslado");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdInternacionDestino).HasColumnName("id_internacion_destino");
            entity.Property(e => e.IdInternacionOrigen).HasColumnName("id_internacion_origen");

            entity.HasOne(d => d.InternacionDestinoNavigation).WithMany(p => p.TrasladosDestino)
                .HasForeignKey(d => d.IdInternacionDestino)
                .HasConstraintName("FK__Traslado__destin");

            entity.HasOne(d => d.InternacionOrigenNavigation).WithMany(p => p.TrasladosOrigen)
                .HasForeignKey(d => d.IdInternacionOrigen)
                .HasConstraintName("FK__Traslado__origen");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
