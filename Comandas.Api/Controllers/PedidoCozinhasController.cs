﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

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
        /// <returns>[ {id, ComandaId, SituacaoId },...  ]</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoCozinha>>> GetPedidos([FromQuery] int? situacaoId)
        {
            // SELECT * FROM PedidoCozinha p
            // INNER JOIN Comanda c on c.Id = p.ComandaId

            var query = _context.PedidoCozinhas
                            .Include(p => p.Comanda)
                            .Include(p => p.PedidoCozinhaItems)
                            .AsQueryable();

            if (situacaoId > 0)
                query = query.Where(w => w.SituacaoId == situacaoId);

            return await query.ToListAsync();
        }

        // GET api/<PedidoCozinhasController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

    }
}
