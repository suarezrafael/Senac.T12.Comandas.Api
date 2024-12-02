using Comandas.Api.Controllers;
using Comandas.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

namespace Comandas.Tests
{
    public class ComandasControllerTests
    {
        private readonly ComandasController _controller;
        private readonly ComandaContexto _contexto;

        public ComandasControllerTests()
        {
            // Configuração do banco InMemory
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ComandaContexto>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            _contexto = scope.ServiceProvider.GetRequiredService<ComandaContexto>();

            _controller = new ComandasController(_contexto);

            InserirDados();
        }

        private void InserirDados()
        {
            var mesa = new Mesa
            {
                NumeroMesa = 1,
                SituacaoMesa = 0
            };

            var cardapioItem = new CardapioItem
            {
                Titulo = "Pizza",
                Descricao = "Pizza de Calabresa",
                Preco = 40.00m,
                PossuiPreparo = true
            };

            var comanda = new Comanda
            {
                NumeroMesa = 1,
                NomeCliente = "Cliente Teste",
                SituacaoComanda = 1
            };

            var comandaItem = new ComandaItem
            {
                Comanda = comanda,
                CardapioItem = cardapioItem
            };

            _contexto.Mesas.Add(mesa);
            _contexto.CardapioItems.Add(cardapioItem);
            _contexto.Comandas.Add(comanda);
            _contexto.ComandaItems.Add(comandaItem);
            _contexto.SaveChanges();
        }

        [Fact]
        public async Task GetComandas_ReturnsOpenComandas()
        {
            // Act
            var result = await _controller.GetComandas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comandas = Assert.IsType<List<ComandaGetDto>>(okResult.Value);
            Assert.NotEmpty(comandas);
            Assert.All(comandas, c => Assert.Equal(1, c.NumeroMesa));
        }

        [Fact]
        public async Task GetComanda_ReturnsComandaById()
        {
            // Arrange
            var comandaId = _contexto.Comandas.First().Id;

            // Act
            var result = await _controller.GetComanda(comandaId);

            // Assert
            var comanda = Assert.IsType<ComandaGetDto>(result.Value);
            Assert.Equal(comandaId, comanda.Id);
        }

        [Fact]
        public async Task GetComanda_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _controller.GetComanda(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutComanda_UpdatesComandaDetails()
        {
            // Arrange
            var comandaId = _contexto.Comandas.First().Id;
            var updateDto = new ComandaUpdateDto
            {
                Id = comandaId,
                NumeroMesa = 2,
                NomeCliente = "Cliente Atualizado",
                ComandaItens = Array.Empty<ComandaItemUpdateDto>()
            };

            var novaMesa = new Mesa
            {
                NumeroMesa = 2,
                SituacaoMesa = 0
            };

            _contexto.Mesas.Add(novaMesa);
            await _contexto.SaveChangesAsync();

            // Act
            var result = await _controller.PutComanda(comandaId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var comandaAtualizada = _contexto.Comandas.First(c => c.Id == comandaId);
            Assert.Equal("Cliente Atualizado", comandaAtualizada.NomeCliente);
            Assert.Equal(2, comandaAtualizada.NumeroMesa);
        }

        [Fact]
        public async Task PutComanda_ReturnsBadRequestForInvalidMesa()
        {
            // Arrange
            var comandaId = _contexto.Comandas.First().Id;
            var updateDto = new ComandaUpdateDto
            {
                Id = comandaId,
                NumeroMesa = 3, // Mesa não existente
                NomeCliente = "Cliente Atualizado",
                ComandaItens = Array.Empty<ComandaItemUpdateDto>()
            };

            // Act
            var result = await _controller.PutComanda(comandaId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Mesa inválida.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostComanda_CreatesNewComanda()
        {
            // Arrange
            var comandaDto = new ComandaDto
            {
                NumeroMesa = 2,
                NomeCliente = "Novo Cliente",
                CardapioItems = new[] { _contexto.CardapioItems.First().Id }
            };

            var novaMesa = new Mesa
            {
                NumeroMesa = 2,
                SituacaoMesa = 0
            };

            _contexto.Mesas.Add(novaMesa);
            await _contexto.SaveChangesAsync();

            // Act
            var result = await _controller.PostComanda(comandaDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var novaComanda = _contexto.Comandas.Last();
            Assert.Equal(2, novaComanda.NumeroMesa);
            Assert.Equal("Novo Cliente", novaComanda.NomeCliente);
        }

        [Fact]
        public async Task DeleteComanda_DeletesExistingComanda()
        {
            // Arrange
            var comandaId = _contexto.Comandas.First().Id;

            // Act
            var result = await _controller.DeleteComanda(comandaId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(_contexto.Comandas.Any(c => c.Id == comandaId));
        }

        [Fact]
        public async Task PatchComanda_ClosesComanda()
        {
            // Arrange
            var comandaId = _contexto.Comandas.First().Id;

            // Act
            var result = await _controller.PatchComanda(comandaId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var comanda = _contexto.Comandas.First(c => c.Id == comandaId);
            Assert.Equal(2, comanda.SituacaoComanda); // Verifica se a situação foi alterada para 2 (fechada)
        }
    }
}
