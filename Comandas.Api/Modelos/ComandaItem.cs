using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeComandas.Modelos
{
    public class ComandaItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CardapioItemId { get; set; }
        public virtual CardapioItem CardapioItem { get; set; }
        public int ComandaId { get; set; }
        public virtual Comanda Comanda { get; set; }
    }
}
