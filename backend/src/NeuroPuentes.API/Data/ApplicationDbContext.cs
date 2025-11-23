using Microsoft.EntityFrameworkCore;
using NeuroPuentesAPI.models;

namespace NeuroPuentesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets para cada modelo
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Entrevista> Entrevistas { get; set; }
        public DbSet<Dialogo> Dialogos { get; set; }
        public DbSet<Contexto> Contextos { get; set; }
        public DbSet<Caracteristica> Caracteristicas { get; set; }
        public DbSet<Caracts_Rel> CaractsRels { get; set; }
        public DbSet<Eval_Entrevista> EvalEntrevistas { get; set; }
        public DbSet<Eval_Categoria> EvalCategorias { get; set; }
        public DbSet<Feedback_Entrevista> FeedbackEntrevistas { get; set; }
        public DbSet<Feedback_Usuario> FeedbackUsuarios { get; set; }
        public DbSet<Stats_Usuario> StatsUsuarios { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<Calificacion_Usuario> CalificacionUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convertir nombre de tabla a snake_case
                var tableName = entity.GetTableName();
                var snakeCaseName = ConvertToSnakeCase(tableName);
                entity.SetTableName(snakeCaseName);

                // Convertir nombres de columnas a snake_case
                foreach (var property in entity.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    var snakeCaseColumnName = ConvertToSnakeCase(columnName ?? property.Name);
                    property.SetColumnName(snakeCaseColumnName);
                }
            }

            ConfigureUsuarioModel(modelBuilder);
            ConfigureContextoModel(modelBuilder);
            ConfigureEntrevistaModel(modelBuilder);
            ConfigureDialogoModel(modelBuilder);
            ConfigureCaracteristicaModel(modelBuilder);
            ConfigureCaractsRelModel(modelBuilder);
            ConfigureEvalEntrevistaModel(modelBuilder);
            ConfigureEvalCategoriaModel(modelBuilder);
            ConfigureFeedbackEntrevistaModel(modelBuilder);
            ConfigureFeedbackUsuarioModel(modelBuilder);
            ConfigureStatsUsuarioModel(modelBuilder);
            ConfigureTipModel(modelBuilder);
            ConfigureCalificacionUsuarioModel(modelBuilder);
        }

        private string ConvertToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            return string.Concat(input.Select((x, i) => 
                i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
        }


        private void ConfigureUsuarioModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                entity.Property(u => u.NombreUsuario)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);
                    
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);
                    
                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Rol)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(u => u.FechaRegistro)
                    .IsRequired();

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.HasIndex(u => u.NombreUsuario)
                    .IsUnique();
            });
        }

        private void ConfigureContextoModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contexto>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(c => c.Descripcion)
                    .IsRequired();

                entity.Property(c => c.Scope)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                    
                entity.Property(c => c.Origen)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(c => c.Vigencia)
                    .IsRequired();

                entity.Property(c => c.FechaCreacion)
                    .IsRequired();

                // Relación opcional con Usuario (CreadoPor)
                entity.HasOne(c => c.Creador) // Usar la propiedad de navegación
                    .WithMany()
                    .HasForeignKey(c => c.CreadoPor)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private void ConfigureEntrevistaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entrevista>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);
                    
                entity.Property(e => e.Descripcion)
                    .IsRequired();
                    
                entity.Property(e => e.ContextoSnapshot)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.Property(e => e.DuracionMin)
                    .IsRequired();

                entity.Property(e => e.NumeroTurnos)
                    .IsRequired();

                // Relación con Usuario
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Entrevistas) // Ahora existe esta propiedad
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Contexto
                entity.HasOne(e => e.Contexto)
                    .WithMany(c => c.Entrevistas) // Ahora existe esta propiedad
                    .HasForeignKey(e => e.ContextoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.ContextoId);
                entity.HasIndex(e => e.FechaCreacion);
            });
        }

        private void ConfigureDialogoModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dialogo>(entity =>
            {
                entity.HasKey(d => d.Id);
                
                entity.Property(d => d.Texto)
                    .IsRequired();
                    
                entity.Property(d => d.Timestamp)
                    .IsRequired();

                entity.Property(d => d.Sender)
                    .HasConversion<string>()
                    .HasMaxLength(10);

                // Relación con Entrevista
                entity.HasOne(d => d.Entrevista)
                    .WithMany(e => e.Dialogos)
                    .HasForeignKey(d => d.EntrevistaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(d => d.EntrevistaId);
                entity.HasIndex(d => new { d.EntrevistaId, d.Turno });
            });
        }

        private void ConfigureCaracteristicaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Caracteristica>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(c => c.Descripcion)
                    .IsRequired();
                    
                entity.Property(c => c.Grupo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(c => c.Vigencia)
                    .IsRequired();

                entity.HasIndex(c => c.Grupo);
                entity.HasIndex(c => c.Vigencia);
            });
        }

        private void ConfigureCaractsRelModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Caracts_Rel>(entity =>
            {
                // Clave compuesta
                entity.HasKey(cr => new { cr.CaracteristicaId, cr.ContextoId });

                // Relación con Caracteristica
                entity.HasOne<Caracteristica>()
                    .WithMany()
                    .HasForeignKey(cr => cr.CaracteristicaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Contexto
                entity.HasOne<Contexto>()
                    .WithMany()
                    .HasForeignKey(cr => cr.ContextoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureEvalEntrevistaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Eval_Entrevista>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.ScoreFinal)
                    .IsRequired();

                // Relación con Entrevista
                entity.HasOne<Entrevista>()
                    .WithMany()
                    .HasForeignKey(e => e.EntrevistaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.EntrevistaId)
                    .IsUnique(); // Una evaluación por entrevista
            });
        }

        private void ConfigureEvalCategoriaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Eval_Categoria>(entity =>
            {
                entity.HasKey(ec => ec.Id);
                
                entity.Property(ec => ec.Categoria)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(ec => ec.Score)
                    .IsRequired();

                // Relación con Eval_Entrevista
                entity.HasOne<Eval_Entrevista>()
                    .WithMany()
                    .HasForeignKey(ec => ec.EvalEntrevistaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ec => ec.EvalEntrevistaId);
                entity.HasIndex(ec => ec.Categoria);
            });
        }

        private void ConfigureFeedbackEntrevistaModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback_Entrevista>(entity =>
            {
                entity.HasKey(f => f.Id);
                
                entity.Property(f => f.Tipo)
                    .IsRequired()
                    .HasMaxLength(20);
                    
                entity.Property(f => f.Mensaje)
                    .IsRequired();

                entity.Property(f => f.Fecha)
                    .IsRequired();

                // Relación con Entrevista
                entity.HasOne<Entrevista>()
                    .WithMany()
                    .HasForeignKey(f => f.EntrevistaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(f => f.EntrevistaId);
                entity.HasIndex(f => f.Tipo);
                entity.HasIndex(f => f.Fecha);
            });
        }

        private void ConfigureFeedbackUsuarioModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback_Usuario>(entity =>
            {
                entity.HasKey(f => f.Id);
                
                entity.Property(f => f.Tipo)
                    .IsRequired()
                    .HasMaxLength(20);
                    
                entity.Property(f => f.Mensaje)
                    .IsRequired();
                    
                entity.Property(f => f.Categoria)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(f => f.Fecha)
                    .IsRequired();

                // Relación con Usuario
                entity.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(f => f.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(f => f.UsuarioId);
                entity.HasIndex(f => f.Tipo);
                entity.HasIndex(f => f.Categoria);
                entity.HasIndex(f => f.Fecha);
            });
        }

        private void ConfigureStatsUsuarioModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stats_Usuario>(entity =>
            {
                entity.HasKey(s => s.Id);
                
                entity.Property(s => s.FechaCorte)
                    .IsRequired();
                    
                entity.Property(s => s.TotalEntrevistas)
                    .IsRequired();
                    
                entity.Property(s => s.TiempoTotalMin)
                    .IsRequired();
                    
                entity.Property(s => s.ScorePromedio)
                    .IsRequired();

                // Relación con Usuario
                entity.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(s => s.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => s.UsuarioId);
                entity.HasIndex(s => s.FechaCorte);
                
                // Índice único para evitar stats duplicados por usuario/fecha
                entity.HasIndex(s => new { s.UsuarioId, s.FechaCorte })
                    .IsUnique();
            });
        }

        private void ConfigureTipModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tip>(entity =>
            {
                entity.HasKey(t => t.Id);
                
                entity.Property(t => t.Contenido)
                    .IsRequired();
                    
                entity.Property(t => t.Categoria)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(t => t.Fecha)
                    .IsRequired();

                entity.Property(t => t.Usado)
                    .IsRequired();

                entity.Property(t => t.Vigencia)
                    .IsRequired();

                // Relación con Usuario
                entity.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Entrevista
                entity.HasOne<Entrevista>()
                    .WithMany()
                    .HasForeignKey(t => t.EntrevistaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => t.UsuarioId);
                entity.HasIndex(t => t.EntrevistaId);
                entity.HasIndex(t => t.Categoria);
                entity.HasIndex(t => t.Vigencia);
            });
        }

        private void ConfigureCalificacionUsuarioModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calificacion_Usuario>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.Property(c => c.Calificacion)
                    .IsRequired();
                    
                entity.Property(c => c.Fecha)
                    .IsRequired();

                // Relación con Usuario
                entity.HasOne<Usuario>()
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.UsuarioId);
                entity.HasIndex(c => c.Fecha);
                
                // Check constraint usando el nuevo enfoque
                entity.ToTable(tb => tb.HasCheckConstraint(
                    "CK_CalificacionUsuario_Calificacion", 
                    "\"Calificacion\" >= 1 AND \"Calificacion\" <= 10"));
            });
        }
    }
}