namespace Comandas.Api.Dtos
{
    public class UsuarioRequest
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
    }
}
