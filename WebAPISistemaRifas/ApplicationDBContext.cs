using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPISistemaRifas.Entidades;

namespace WebAPISistemaRifas
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ParticipantesCartasRifa>()
                .HasKey(r => new { r.IdRifa, r.IdParticipantes,r.IdCartas });
        }
        public DbSet<Participantes> Participantes { get; set; }
        public DbSet<Rifa> Rifas { get; set; }
        public DbSet<Premios> Premios { get; set; }
        public DbSet<Cartas> Cartas { get; set; }
        public DbSet<ParticipantesCartasRifa> ParticipantesCartasRifa { get; set; }

    }
}
