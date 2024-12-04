namespace Comandas.Api.Dtos
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}
