using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder;
using DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder;
using DesafioMinervaFoods.Application.Features.Orders.Queries.GetAllOrders;
using DesafioMinervaFoods.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo pedido no sistema.
        /// </summary>
        /// <remarks>
        /// Pedidos acima de R$ 5.000,00 serão criados com status 'Criado' e exigirão aprovação manual.
        /// Caso contrário, o status será 'Pago'.
        /// </remarks>
        /// <param name="command">Dados para criação do pedido.</param>
        /// <response code="201">Pedido criado com sucesso.</response>
        /// <response code="400">Dados inválidos enviados na requisição ou falha na validação.</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.OrderId }, result);
        }

        /// <summary>
        /// Obtém a listagem de todos os pedidos cadastrados.
        /// </summary>
        /// <response code="200">Retorna a lista de pedidos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(result);
        }

        /// <summary>
        /// Obtém os detalhes de um pedido específico por ID.
        /// </summary>
        /// <param name="id">Identificador único do pedido.</param>
        /// <response code="200">Retorna os detalhes do pedido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Aprova manualmente um pedido que requer análise.
        /// </summary>
        /// <param name="id">Identificador único do pedido.</param>
        /// <response code="204">Pedido aprovado com sucesso.</response>
        /// <response code="400">Regra de negócio violada ou ID inválido.</response>
        /// <response code="404">Pedido não encontrado.</response>
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Id inválido" });

            var result = await _mediator.Send(new ApproveOrderCommand(id));

            if (!result.IsSuccess)
                return NotFound(result);

            return NoContent();
        }
    }
}