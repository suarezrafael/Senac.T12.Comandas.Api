using SistemaDeComandas.BancoDeDados;
using SistemaDeComandas.Modelos;

namespace Comandas.Api
{
    public static class InicializarDados
    {
        public static void Semear(ComandaContexto banco)
        {
            if (!banco.CardapioItems.Any())
            {
                banco.CardapioItems.AddRange(
                    new CardapioItem()
                    {
                        Descricao = "XIS SALADA , BIFE, OVO, PRESUNTO, QUEIJO",
                        PossuiPreparo = true,
                        Preco = 20.00M,
                        Titulo = "XIS SALADA"
                    },
                    new CardapioItem()
                    {
                        Descricao = "XIS BACON, BIFE, OVO, BACON, ALFACE, CEBOLA",
                        PossuiPreparo = true,
                        Preco = 15M,
                        Titulo = "XIS BACON"
                    },
                    new CardapioItem
                    {
                        Descricao = "COCA COLA LATA 350ML ",
                        PossuiPreparo = false,
                        Preco = 9M,
                        Titulo = "COCA COLA LATA 350 ML"
                    }
                ); // ADDRange
            } // IF 

            if (!banco.Usuarios.Any())
            {
                banco.Usuarios.AddRange(
                    new Usuario
                    {
                        Email = "admin@admin.com",
                        Nome = "Admin",
                        Senha = "admin"
                    }
                );
            }

            if (!banco.Mesas.Any())
            {
                banco.Mesas.AddRange(
                    new Mesa { NumeroMesa = 1, SituacaoMesa = 1 },
                    new Mesa { NumeroMesa = 2, SituacaoMesa = 1 },
                    new Mesa { NumeroMesa = 3, SituacaoMesa = 1 },
                    new Mesa { NumeroMesa = 4, SituacaoMesa = 1 }
                );
            }

            if (!banco.Comandas.Any())
            {
                var comanda = new Comanda() { NomeCliente = "RAFAEL VIEIRA SUAREZ", NumeroMesa = 1, SituacaoComanda = 1 };
                banco.Comandas.Add(comanda);

                ComandaItem[] comandaItems =
                {       new ComandaItem()
                        {
                            Comanda = comanda,
                            CardapioItemId = 1
                        },
                        new ComandaItem()
                        {
                            Comanda = comanda,
                            CardapioItemId = 2
                        }
                    };

                if (!banco.ComandaItems.Any())
                {
                    banco.ComandaItems.AddRange(comandaItems);
                }

                var pedidoCozinha = new PedidoCozinha { Comanda = comanda };
                var pedidoCozinha2 = new PedidoCozinha { Comanda = comanda };

                PedidoCozinhaItem[] pedidoCozinhaItems =
                {
                    new PedidoCozinhaItem{PedidoCozinha = pedidoCozinha , ComandaItem = comandaItems[0] },
                    new PedidoCozinhaItem{PedidoCozinha = pedidoCozinha2 , ComandaItem = comandaItems[1] }
                };

                banco.PedidoCozinhas.Add(pedidoCozinha);
                banco.PedidoCozinhas.Add(pedidoCozinha2);
                banco.PedidoCozinhaItems.AddRange(pedidoCozinhaItems);
            }
            // INSERT INTO CardapioItem (Columns) VALUES(1, 'SALSICHAO')
            banco.SaveChanges();
        }
    }
}
