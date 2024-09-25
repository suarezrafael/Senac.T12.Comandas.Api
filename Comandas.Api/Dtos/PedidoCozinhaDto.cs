namespace Comandas.Api.Dtos
{
    public class PedidoCozinhaDto
    {
        public int Id { get; set; }
        public int NumeroMesa { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
    }
}
