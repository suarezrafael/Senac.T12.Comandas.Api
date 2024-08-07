using Microsoft.EntityFrameworkCore;
using SistemaDeComandas.Modelos;

namespace SistemaDeComandas.BancoDeDados
{
    public class ComandaContexto : DbContext
    {
        // criar as variaveis que representam tables
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<CardapioItem> CardapioItems { get; set; }
        public DbSet<Modelos.Comanda> Comandas { get; set; }
        public DbSet<ComandaItem> ComandaItems { get; set; }
        public DbSet<PedidoCozinha> PedidoCozinhas { get; set; }
        public DbSet<PedidoCozinhaItem> PedidoCozinhaItems { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // para configurar a conexão do banco de dados
        public ComandaContexto(DbContextOptions<ComandaContexto> options) : base(options)
        {
        }

        // para configurar os relacionamentos das tabelas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Uma Comanda possui muitos ComandaItems
            // E sua chave extrangeira é ComandaId
            modelBuilder.Entity<Comanda>()
                .HasMany<ComandaItem>()
                .WithOne(ci => ci.Comanda)
                .HasForeignKey(f => f.ComandaId);

            modelBuilder.Entity<ComandaItem>()
                .HasOne(ci => ci.Comanda)
                .WithMany(ci => ci.ComandaItems)
                .HasForeignKey(f => f.ComandaId);

            // O Item da comanda possui um Item de Cardápio
            // E sua chave extrangeira é CardapioItemId
            modelBuilder.Entity<ComandaItem>()
                .HasOne(ci => ci.CardapioItem)
                .WithMany()
                .HasForeignKey(ci => ci.CardapioItemId);

            // Pedido Cozinha com Pedido Cozinha Item
            modelBuilder.Entity<PedidoCozinha>()
                .HasMany<PedidoCozinhaItem>()
                .WithOne(pci => pci.PedidoCozinha)
                .HasForeignKey(pci => pci.PedidoCozinhaId);

            modelBuilder.Entity<PedidoCozinhaItem>()
                .HasOne(tico => tico.PedidoCozinha)
                .WithMany(tico => tico.PedidoCozinhaItems)
                .HasForeignKey(teco => teco.PedidoCozinhaId);

            // pedido cozinha item possui um comanda item
            // E sua chave extrangeira é ComandaItemId
            modelBuilder.Entity<PedidoCozinhaItem>()
                .HasOne(pci => pci.ComandaItem)
                .WithMany()
                .HasForeignKey(pci => pci.ComandaItemId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
