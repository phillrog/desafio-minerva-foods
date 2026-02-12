using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Cria um novo pedido no sistema.
        /// </summary>
        /// <remarks>
        /// Pedidos acima de R$ 5.000,00 serão criados com status 'Criado' e exigirão aprovação manual.
        /// Caso contrário, o status será 'Pago'.
        /// </remarks>
        /// <param name="request">Dados para criação do pedido.</param>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="400">Dados inválidos enviados na requisição.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetAll), new { id = result.OrderId }, result);
            }
            catch (ValidationException ex)
            {
                // Retorna os erros do FluentValidation de forma estruturada
                return BadRequest(ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }
        }

        /// <summary>
        /// Obtém a listagem de todos os pedidos cadastrados.
        /// </summary>
        /// <response code="200">Retorna a lista de pedidos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Aprova manualmente um pedido que requer análise.
        /// </summary>
        /// <param name="id">Identificador único do pedido.</param>
        /// <response code="204">Pedido aprovado com sucesso.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [HttpPut("{id}/approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(Guid id)
        {
            var success = await _orderService.ApproveOrderAsync(id);
            if (!success)
                return NotFound(new { mensagem = "Pedido não encontrado ou não requer aprovação." });

            return NoContent();
        }
    }
}