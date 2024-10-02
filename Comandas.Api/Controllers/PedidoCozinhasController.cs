using Comandas.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeComandas.BancoDeDados;

namespace Comandas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoCozinhasController : ControllerBase
    {   // variavel do banco de dados
        private readonly ComandaContexto _context;
        // o contrutor do controlador 
        public PedidoCozinhasController(ComandaContexto contexto)
        {
            _context = contexto;
        }

        // GET: api/<PedidoCozinhasController>
        /// <summary>
        /// 
        /// </summary>
        /// <returns>[ {Id, NumeroPesa, NomeCliente,TItulo },...  ]</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoCozinhaGetDto>>> GetPedidos([FromQuery] int? situacaoId)
        {
            var query = _context.PedidoCozinhas
                            .Include(p => p.Comanda)
                            .Include(p => p.PedidoCozinhaItems)
                              .ThenInclude(p => p.ComandaItem)
                                .ThenInclude(p => p.CardapioItem)
                            .AsQueryable();

            if (situacaoId > 0)
                query = query.Where(w => w.SituacaoId == situacaoId);

            return await query
                .Select(s => new PedidoCozinhaGetDto()
                {
                    Id = s.Id,
                    NumeroMesa = s.Comanda.NumeroMesa,
                    NomeCliente = s.Comanda.NomeCliente,
                    Titulo = s.PedidoCozinhaItems.First().ComandaItem.CardapioItem.Titulo
                }).ToListAsync();
        }

        // Atualizar um pedido de cozinha para um novo status
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoCozinha(int id, PedidoCozinhaUpdateDto pedido)
        {
            // Consulta o pedido pelo Id informados
            // SELECT * FROM PedidoCozinha WHERE id = @id
            var pedidoCozinha = await _context
                                        .PedidoCozinhas
                                        .FirstAsync(p => p.Id == id);
            // Alteração do Status
            pedidoCozinha.SituacaoId = pedido.NovoStatusId;
            // Gravação no Banco de Dados
            // UPDATE PEdidoCozinha SET SituacaoId = 3 WHERE Id = @id
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
