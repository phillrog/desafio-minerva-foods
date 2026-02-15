using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.PaymentCondition.Queries.GetAllOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentConditionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentConditionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém a listagem de todas as formas de pagamentos cadastrados.
        /// </summary>
        /// <response code="200">Retorna a lista de formas de pagamentos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentConditionResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPaymentConditionQuery());
            return Ok(result);
        }
    }
}
