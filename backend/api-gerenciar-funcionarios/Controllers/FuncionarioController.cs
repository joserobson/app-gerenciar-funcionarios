using api_gerenciar_funcionarios.Core.Application;
using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using api_gerenciar_funcionarios.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_gerenciar_funcionarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : ControllerBase
    {
        private readonly CriarFuncionarioUseCase _criarFuncionarioUseCase;
        private readonly ObterFuncionariosUseCase _obterFuncionariosUseCase;
        private readonly ObterFuncionarioPorIdUseCase _obterFuncionarioPorIdUseCase;
        private readonly AtualizarFuncionarioUseCase _atualizarFuncionarioUseCase;
        private readonly DeletarFuncionarioUseCase _deletarFuncionarioUseCase;        

        public FuncionarioController(
            CriarFuncionarioUseCase criarFuncionarioUseCase,
            ObterFuncionariosUseCase obterFuncionariosUseCase,
            ObterFuncionarioPorIdUseCase obterFuncionarioPorIdUseCase,
            AtualizarFuncionarioUseCase atualizarFuncionarioUseCase,
            DeletarFuncionarioUseCase deletarFuncionarioUseCase)
        {
            _criarFuncionarioUseCase = criarFuncionarioUseCase;
            _obterFuncionariosUseCase = obterFuncionariosUseCase;
            _obterFuncionarioPorIdUseCase = obterFuncionarioPorIdUseCase;
            _atualizarFuncionarioUseCase = atualizarFuncionarioUseCase;
            _deletarFuncionarioUseCase = deletarFuncionarioUseCase;            
        }

        // GET: api/Funcionarios
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FuncionarioDTO>>> ObterFuncionarios()
        {
            var resposta = await _obterFuncionariosUseCase.Executar();
            if (!resposta.Success)
                return BadRequest(resposta.ErrorMessages);

            return Ok(resposta.Data);
            
        }

        // GET: api/Funcionarios/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FuncionarioDTO>> ObterFuncionario(Guid id)
        {
            var resposta = await _obterFuncionarioPorIdUseCase.Executar(id);
            if (!resposta.Success)
                return BadRequest(resposta.ErrorMessages);

            return Ok(resposta.Data);                      
        }
       

        // POST: api/Funcionarios
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FuncionarioDTO>> CriarFuncionario([FromBody] FuncionarioEntradaDTO funcionarioInput)
        {
            try
            {                
                var result = await _criarFuncionarioUseCase.Executar(funcionarioInput);
                if (!result.Success)
                    return BadRequest(result.ErrorMessages);

                return CreatedAtAction(nameof(ObterFuncionario), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.Message });
            }
        }

        // PUT: api/Funcionarios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> AtualizarFuncionario(FuncionarioEntradaDTO funcionarioInput)
        {            
            try
            {
                var result = await _atualizarFuncionarioUseCase.Executar(funcionarioInput);
                if (!result.Success)
                    return BadRequest(result.ErrorMessages);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.Message });
            }
        }

        // DELETE: api/Funcionarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarFuncionario(Guid id)
        {
            try
            {
                var resposta = await _deletarFuncionarioUseCase.Executar(id);
                if (!resposta.Success)
                    return BadRequest(resposta.ErrorMessages);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
