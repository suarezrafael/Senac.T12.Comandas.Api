using Comandas.Api.Controllers;
using Comandas.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

namespace Comandas.Tests
{
    public class PedidoCozinhasControllerTests
    {
        private readonly PedidoCozinhasController _controller;
        private readonly ComandaContexto _contexto;

        public PedidoCozinhasControllerTests()
        {
            // Configuração do banco InMemory
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ComandaContexto>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            _contexto = scope.ServiceProvider.GetRequiredService<ComandaContexto>();

            _controller = new PedidoCozinhasController(_contexto);

            InserirDados();
        }

        private void InserirDados()
        {
            var cardapioItem = new CardapioItem
            {
                Titulo = "Pizza",
                Descricao = "Pizza de Mussarela",
                Preco = 30.00m,
                PossuiPreparo = true
            };

            var comanda = new Comanda
            {
                NumeroMesa = 1,
                NomeCliente = "Cliente 1",
                SituacaoComanda = 1
            };

            var comandaItem = new ComandaItem
            {
                CardapioItem = cardapioItem,
                Comanda = comanda
            };

            var pedidoCozinha = new PedidoCozinha
            {
                Comanda = comanda,
                SituacaoId = 1
            };

            var pedidoCozinhaItem = new PedidoCozinhaItem
            {
                PedidoCozinha = pedidoCozinha,
                ComandaItem = comandaItem
            };

            // Adiciona os dados ao contexto
            _contexto.CardapioItems.Add(cardapioItem);
            _contexto.Comandas.Add(comanda);
            _contexto.ComandaItems.Add(comandaItem);
            _contexto.PedidoCozinhas.Add(pedidoCozinha);
            _contexto.PedidoCozinhaItems.Add(pedidoCozinhaItem);

            _contexto.SaveChanges();
        }

        [Fact]
        public async Task GetPedidosBySituacaoAsync_ReturnsPedidos()
        {
            // Arrange
            var situacaoId = 1;

            // Act
            var result = await _controller.GetPedidos(situacaoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pedidos = Assert.IsType<List<PedidoCozinhaGetDto>>(okResult.Value);
            Assert.NotEmpty(pedidos);
        }

        [Fact]
        public async Task UpdatePedidoAsync_UpdatesPedido()
        {
            // Arrange
            var pedido = _contexto.PedidoCozinhas.First();
            var updateDto = new PedidoCozinhaUpdateDto
            {
                NovoStatusId = 3
            };

            // Act
            var result = await _controller.PutPedidoCozinha(pedido.Id, updateDto);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(3, _contexto.PedidoCozinhas.First().SituacaoId);
        }
    }
}
