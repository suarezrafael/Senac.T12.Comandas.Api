using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

namespace Comandas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoCozinhasController : ControllerBase
    {
        private readonly ComandaContexto _context;

        public PedidoCozinhasController(ComandaContexto context)
        {
            _context = context;
        }

        // GET: api/PedidoCozinhas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoCozinha>>> GetPedidoCozinhas()
        {
            return await _context.PedidoCozinhas.ToListAsync();
        }

        // GET: api/PedidoCozinhas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoCozinha>> GetPedidoCozinha(int id)
        {
            var pedidoCozinha = await _context.PedidoCozinhas.FindAsync(id);

            if (pedidoCozinha == null)
            {
                return NotFound();
            }

            return pedidoCozinha;
        }

        // PUT: api/PedidoCozinhas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoCozinha(int id, PedidoCozinha pedidoCozinha)
        {
            if (id != pedidoCozinha.Id)
            {
                return BadRequest();
            }

            _context.Entry(pedidoCozinha).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoCozinhaExists(id))
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

        // POST: api/PedidoCozinhas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PedidoCozinha>> PostPedidoCozinha(PedidoCozinha pedidoCozinha)
        {
            _context.PedidoCozinhas.Add(pedidoCozinha);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPedidoCozinha", new { id = pedidoCozinha.Id }, pedidoCozinha);
        }

        // DELETE: api/PedidoCozinhas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedidoCozinha(int id)
        {
            var pedidoCozinha = await _context.PedidoCozinhas.FindAsync(id);
            if (pedidoCozinha == null)
            {
                return NotFound();
            }

            _context.PedidoCozinhas.Remove(pedidoCozinha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PedidoCozinhaExists(int id)
        {
            return _context.PedidoCozinhas.Any(e => e.Id == id);
        }
    }
}
