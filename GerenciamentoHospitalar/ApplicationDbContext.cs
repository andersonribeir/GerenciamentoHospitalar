using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoHospitalar.Models
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public DbSet<FichaMedica> FichasMedicas { get; set; } // Adicione esta linha para definir a entidade FichaMedica

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Suas configurações de modelo aqui...

            // Configuração de relacionamento entre Usuario e FichaMedica
            builder.Entity<FichaMedica>()
                .HasOne(f => f.Usuario);

        }
    }
}
