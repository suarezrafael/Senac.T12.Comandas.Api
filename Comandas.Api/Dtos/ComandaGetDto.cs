namespace Comandas.Api.Dtos
{
    public class ComandaGetDto
    {
        public int NumeroMesa { get; set; }
        public string NomeCliente { get; set; }
        public List<ComandaItensGetDto> ComandaItens { get; set; } = new List<ComandaItensGetDto>();
    }

    public class ComandaItensGetDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } // VEM da tabela CardapioItem 
    }
}
