using Comandas.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

namespace Comandas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandasController : ControllerBase
    {
        private readonly ComandaContexto _context;

        public ComandasController(ComandaContexto context)
        {
            _context = context;
        }

        // Consulta as comandas com status ABERTA(1)
        // SELECT * FROM Comandas WHERE SituacaoComanda = 1
        // GET: api/Comandas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComandaGetDto>>> GetComandas()
        {
            // SELECT c.NumeroMesa, c.NomeCLiente FROM Comandas WHERE SituacaoComanda = 1
            var comandas = await _context.Comandas
                .Where(c => c.SituacaoComanda == 1)
                .Select(c => new ComandaGetDto
                {
                    Id = c.Id,
                    NumeroMesa = c.NumeroMesa,
                    NomeCliente = c.NomeCliente,
                    ComandaItens = c.ComandaItems
                                    .Select(ci => new ComandaItensGetDto
                                    {
                                        Id = ci.Id,
                                        Titulo = ci.CardapioItem.Titulo,
                                    })
                                    .ToList()
                })
                .ToListAsync();
            // retorna o conteudo com a lista de comandas
            return Ok(comandas);
        }

        // GET: api/Comandas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComandaGetDto>> GetComanda(int id)
        {
            // SELECT * FROM Comandas WHERE id = 1
            // Busca a comanda por id
            var comanda = await _context.Comandas
                                    .FirstOrDefaultAsync(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }

            var comandaDto = new ComandaGetDto
            {
                NumeroMesa = comanda.NumeroMesa,
                NomeCliente = comanda.NomeCliente
            };
            // SELECT Id, Titulo FROM ComandaItems ci WHERE ci.comandaId = 1
            // INNER JOIN CardapioItem cii on cii.Id = ci.CardapioItemId
            // Busca os itens da comanda 
            var comandaItensDto = await _context.ComandaItems
                                        .Include(ci => ci.CardapioItem)
                                        .Where(ci => ci.ComandaId == id)
                                        .Select(cii => new ComandaItensGetDto
                                        {
                                            Id = cii.Id,
                                            Titulo = cii.CardapioItem.Titulo
                                        })
                                    .ToListAsync();

            comandaDto.ComandaItens = comandaItensDto;

            return comandaDto;
        }

        // PUT: api/Comandas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComanda(int id, ComandaUpdateDto comanda)
        {
            if (id != comanda.Id)
            {
                return BadRequest();
            }
            // SELECT * FROM Comandas WHERE id = 2
            var comandaUpdate = await _context.Comandas
                .FirstAsync(cu => cu.Id == id);

            // verifica se foi informada uma nova mesa
            if (comanda.NumeroMesa > 0)
            {
                // verificar a disponibilidade da nova mesa
                // SELECT * FROM MESAS WHERE NumeroMesa = 2
                var mesa = await _context.Mesas
                    .FirstOrDefaultAsync(m => m.NumeroMesa == comanda.NumeroMesa);
                if (mesa == null)
                    return BadRequest("Mesa inválida.");
                if (mesa.SituacaoMesa != 0)
                    return BadRequest("Mesa ocupada.");

                // alocar a nova mesa 
                mesa.SituacaoMesa = 1;

                // desalocar a mesa atual
                var mesaAtual = await _context.Mesas
                    .FirstAsync(mesa => mesa.NumeroMesa == comandaUpdate.NumeroMesa);
                mesaAtual.SituacaoMesa = 0;

                // atualiza o numero da mesa na comanda
                comandaUpdate.NumeroMesa = comanda.NumeroMesa;
            }

            if (!string.IsNullOrEmpty(comanda.NomeCliente))
                comandaUpdate.NomeCliente = comanda.NomeCliente;

            foreach (var item in comanda.ComandaItens)
            {
                //incluir
                if (item.incluir)
                {
                    var novoComandaItem = new ComandaItem()
                    {
                        Comanda = comandaUpdate,
                        CardapioItemId = item.cardapioItemId
                    };

                    await _context.ComandaItems.AddAsync(novoComandaItem);

                    // verificar se o cardapio possui preparo, se sim criar o pedido da cozinha
                    var cardapioItem = await _context.CardapioItems.FindAsync(item);
                    if (cardapioItem.PossuiPreparo)
                    {
                        var novoPedidoCozinha = new PedidoCozinha()
                        {
                            Comanda = comandaUpdate,
                            SituacaoId = 1
                        };
                        await _context.PedidoCozinhas.AddAsync(novoPedidoCozinha);
                        var novoPedidoCozinhaItem = new PedidoCozinhaItem()
                        {
                            PedidoCozinha = novoPedidoCozinha,
                            ComandaItem = novoComandaItem
                        };
                        await _context.PedidoCozinhaItems.AddAsync(novoPedidoCozinhaItem);
                    }
                }

                // excluir
                if (item.excluir)
                {
                    var comandaItemExcluir = await _context.ComandaItems
                        .FirstAsync(tico => tico.Id == item.Id);

                    _context.ComandaItems.Remove(comandaItemExcluir);
                }

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComandaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Comandas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comanda>> PostComanda(ComandaDto comanda)
        {
            // verificar se a mesa está disponivel
            // Pesquisa a mesa no banco (SELECT * FROM MESAS WHERE NumeroMesa = 2)
            var mesa = _context.Mesas.FirstOrDefault(m => m.NumeroMesa == comanda.NumeroMesa);

            if (mesa is null)
                return BadRequest("Mesa não encontrada");

            if (mesa.SituacaoMesa != 0)
                return BadRequest("Esta mesa está ocupada");

            // altera a mesa para ocupada para nao permitir abrir outra comanda para a mesma mesa
            mesa.SituacaoMesa = 1;

            // criando uma nova comanda
            var novaComanda = new Comanda()
            {
                NumeroMesa = comanda.NumeroMesa,
                NomeCliente = comanda.NomeCliente
            };

            // adicionando a comanda no banco
            // INSERT INTO COMANDAS (Id, NUmeroMesa) VALUES(1,2)
            await _context.Comandas.AddAsync(novaComanda);

            foreach (var item in comanda.CardapioItems)
            {
                var novoItemComanda = new ComandaItem()
                {
                    Comanda = novaComanda,
                    CardapioItemId = item
                };

                // adicionando o novo item na comanda
                // INSERT INTO ComandaItens (Id, CardapioItemId, ComandaId) VALUES(AI, 3, 5)
                await _context.ComandaItems.AddAsync(novoItemComanda);

                // Verificar se o cardapio possui preparo
                // SELECT PossuiPreparo FROM CardapioItem Where Id = <item>
                var cardapioItem = await _context.CardapioItems.FindAsync(item);

                if (cardapioItem.PossuiPreparo)
                {
                    var novoPedidoCozinha = new PedidoCozinha()
                    {
                        Comanda = novaComanda,
                        SituacaoId = 1 // PENDENTE
                    };
                    // INSERT INTO PedidoCozinha (Id,ComandaId, SituacaoId) VALUES(1+, 6, 1 )
                    await _context.PedidoCozinhas.AddAsync(novoPedidoCozinha);

                    var novoPedidoCozinhaItem = new PedidoCozinhaItem()
                    {
                        PedidoCozinha = novoPedidoCozinha,
                        ComandaItem = novoItemComanda
                    };
                    await _context.PedidoCozinhaItems.AddAsync(novoPedidoCozinhaItem);
                }
            }

            // salvando a comanda
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComanda", new { id = novaComanda.Id }, comanda);
        }

        // DELETE: api/Comandas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComanda(int id)
        {
            var comanda = await _context.Comandas.FindAsync(id);
            if (comanda == null)
            {
                return NotFound();
            }
            _context.Comandas.Remove(comanda);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchComanda(int id)
        {   // consulto a comanda SELECT * from Comandas WHERE id = {id}
            var comanda = await _context.Comandas.FindAsync(id);
            if (comanda == null) // retorna um 404
                return NotFound();
            // alteração comanda
            comanda.SituacaoComanda = 2;

            // liberar a mesa
            var mesa = await _context.Mesas.FirstAsync(m => m.NumeroMesa == comanda.NumeroMesa);
            mesa.SituacaoMesa = 0;

            // UPDATE Comandas SET SituacaoComanda = 2 WHERE id = {id}
            await _context.SaveChangesAsync();
            // returna um 204
            return NoContent();
        }

        private bool ComandaExists(int id)
        {
            return _context.Comandas.Any(e => e.Id == id);
        }
    }
}
