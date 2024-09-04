namespace Comandas.Api.Dtos
{
    public class ComandaUpdateDto
    {
        public int Id { get; set; }
        public int NumeroMesa { get; set; }
        public string NomeCliente { get; set; }
        public int[] CardapioItems { get; set; } = [];
    }
}
