using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tabla Roles
            modelBuilder.Entity<Rol>().ToTable("Roles");

            // Tabla Usuarios
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.RolId);

            // Tabla Categorias
            modelBuilder.Entity<Categoria>().ToTable("Categorias");

            // Tabla Proveedores
            modelBuilder.Entity<Proveedor>().ToTable("Proveedores");

            // Tabla Productos
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioCompra)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Producto>()
                .Property(p => p.PrecioVenta)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId);
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany()
                .HasForeignKey(p => p.ProveedorId);

            // Tabla Movimientos
            modelBuilder.Entity<Movimiento>().ToTable("Movimientos");
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.Producto)
                .WithMany()
                .HasForeignKey(m => m.ProductoId);
        }
    }
}