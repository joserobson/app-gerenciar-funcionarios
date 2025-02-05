using api_gerenciar_funcionarios.Controllers;
using api_gerenciar_funcionarios.Dtos;
using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Core.Application;
using api_gerenciar_funcionarios.Infrastructure.Persistence;
using api_gerenciar_funcionarios.Infrastructure.Repositories;
using api_gerenciar_funcionarios.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api_gerenciar_funcionarios.Tests
{
    public class FuncionarioControllerTests
    {
        private readonly FuncionarioDbContext _context;
        private readonly FuncionarioRepository _funcionarioRepository;
        private readonly CriarFuncionarioUseCase _criarFuncionarioUseCase;
        private readonly ObterFuncionariosUseCase _obterFuncionariosUseCase;
        private readonly ObterFuncionarioPorIdUseCase _obterFuncionarioPorIdUseCase;
        private readonly AtualizarFuncionarioUseCase _atualizarFuncionarioUseCase;
        private readonly DeletarFuncionarioUseCase _deletarFuncionarioUseCase;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly IValidator<FuncionarioEntradaDTO> _validator;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public FuncionarioControllerTests()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<FuncionarioDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseFuncionarios")
                .Options;

            _context = new FuncionarioDbContext(options);
            _funcionarioRepository = new FuncionarioRepository(_context);

            _validator = new FuncionarioValidator();

            // Configura o mock do UnitOfWork
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

            // Configura o mock do UserManager
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Configura os Use Cases reais com o banco in-memory
            _criarFuncionarioUseCase = new CriarFuncionarioUseCase(_funcionarioRepository, _userManagerMock.Object, _validator,_unitOfWorkMock.Object);
            _obterFuncionariosUseCase = new ObterFuncionariosUseCase(_funcionarioRepository);
            _obterFuncionarioPorIdUseCase = new ObterFuncionarioPorIdUseCase(_funcionarioRepository);
            _atualizarFuncionarioUseCase = new AtualizarFuncionarioUseCase(_funcionarioRepository, _userManagerMock.Object, _validator);
            _deletarFuncionarioUseCase = new DeletarFuncionarioUseCase(_funcionarioRepository);
                        
        }

        [Fact]
        public async Task ObterFuncionarios_DeveRetornarListaDeFuncionarios()
        {
            // Arrange
            _context.Funcionarios.RemoveRange(_context.Funcionarios);
            await _context.SaveChangesAsync();

            var funcionario = new Funcionario
            {
                Id = Guid.NewGuid(),
                Nome = "João",
                Sobrenome = "Silva",
                NumeroDocumento = "123456789",
                Telefones = "11987654321",
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Cargo = Cargo.Diretor,
                IdentityUser = new IdentityUser { Email="jose.robson@uai.com.br"}
            };

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.ObterFuncionarios();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<FuncionarioDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnFuncionarios = Assert.IsType<List<FuncionarioDTO>>(okResult.Value);
            Assert.Single(returnFuncionarios);
        }

        [Fact]
        public async Task CriarFuncionario_DeveCriarNovoFuncionario()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "12333456789",
                Telefones = new List<string> { "11987654321", "329999934311" },
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!",
                Cargo = Cargo.Diretor
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(actionResult.Value);
        }

        [Fact]
        public async Task AtualizarFuncionario_DeveAtualizarFuncionarioExistente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var funcionario = new Funcionario
            {
                Id = id,
                Nome = "João",
                Sobrenome = "Silva",
                NumeroDocumento = "123456788889",
                Telefones = "11987654321",
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Cargo = Cargo.Diretor,
                IdentityUser = new IdentityUser { Email= "joao.silva@example.com" }
            };

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João Atualizado",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "1234567888889",
                Telefones = new List<string> { "11987654321", "32999934311" },
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Id = id,
                Cargo = Cargo.Diretor
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);
            // Act
            var result = await controller.AtualizarFuncionario(funcionarioInput);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedFuncionario = await _context.Funcionarios.FindAsync(id);
            Assert.Equal("João Atualizado", updatedFuncionario.Nome);
        }

        [Fact]
        public async Task DeletarFuncionario_DeveDeletarFuncionarioExistente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var funcionario = new Funcionario
            {
                Id = id,
                Nome = "João",
                Sobrenome = "Silva",
                NumeroDocumento = "123456789",
                Telefones = "11987654321",
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1)
            };

            _context.Funcionarios.Add(funcionario);
            await _context.SaveChangesAsync();

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.DeletarFuncionario(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedFuncionario = await _context.Funcionarios.FindAsync(id);
            Assert.Null(deletedFuncionario);
        }

        [Fact]
        public async Task CriarFuncionario_SemNome_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123456789",
                Telefones = ["11987654321,11987654322"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!",
                Cargo = Cargo.Diretor,
            };


            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Campo Nome obrigatório.", errors);
        }

        [Fact]
        public async Task CriarFuncionario_SemSobrenome_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123456789",
                Telefones = ["11987654321,11987654322"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!",
                Cargo = Cargo.Diretor,
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Campo Sobrenome obrigatório.", errors);
        }

        [Fact]
        public async Task CriarFuncionario_SemEmail_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                NumeroDocumento = "123456789",
                Telefones = ["11987654321", "11987654322"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!",
                Cargo = Cargo.Funcionario,
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Campo Email obrigatório.", errors);
        }

        [Fact]
        public async Task CriarFuncionario_SemNumeroDocumento_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                Telefones = ["11987654321,11987654322"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!"
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert            
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Campo Numero do Documento obrigatório.", errors);

        }

        [Fact]
        public async Task CriarFuncionario_SemTelefone_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123456789",
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!"
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert            
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Campo Telefone obrigatório.", errors);
        }


        [Fact]
        public async Task CriarFuncionario_NumeroDocumentoDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            _context.Funcionarios.Add(new Funcionario
            {
                Id = Guid.NewGuid(),
                Nome = "João",
                Sobrenome = "Silva",
                NumeroDocumento = "123456789",
                Telefones = "11987654321,11987654322",
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                IdentityUserId = "1",
                Cargo = Cargo.Diretor
            });
            await _context.SaveChangesAsync();

            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123456789",
                Telefones = ["11987654321", "11987654322"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!",
                Cargo = Cargo.Diretor
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert            
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Número do Documento já em uso.", errors);
        }

        [Fact]
        public async Task CriarFuncionario_TelefoneUnico_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123456789",
                Telefones = ["11987654321"],
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "Senha123!"
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("Forneça pelo menos 2 Telefones.", errors);

        }

        [Fact]
        public async Task CriarFuncionario_MenorDeIdade_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "1234996789",
                Telefones = ["11987654321", "11987654322"],
                NomeGestor = "Maria",
                DataNascimento = DateTime.Now.AddYears(-17), // Menor de 18 anos
                Senha = "Senha123!"
            };

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(actionResult.Value);
            Assert.Contains("O funcionário não pode ser menor de idade.", errors);
        }

        [Fact]
        public async Task CriarFuncionario_SenhaFraca_DeveRetornarBadRequest()
        {
            // Arrange
            var funcionarioInput = new FuncionarioEntradaDTO
            {
                PrimeiroNome = "João",
                Sobrenome = "Silva",
                Email = "joao.silva@example.com",
                NumeroDocumento = "123444456789",
                Telefones = new List<string> { "11987654321", "11987654322" },
                NomeGestor = "Maria",
                DataNascimento = new DateTime(1990, 1, 1),
                Senha = "123" // Senha fraca
            };

            // Configura o mock para simular a falha na criação do usuário
            var identityErrors = new List<IdentityError>
                    {
                        new IdentityError { Description = "A senha deve ter pelo menos 8 caracteres." },
                        new IdentityError { Description = "A senha deve conter pelo menos uma letra maiúscula (A-Z)." }
                    };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            var controller = new FuncionarioController(
                _criarFuncionarioUseCase, _obterFuncionariosUseCase, _obterFuncionarioPorIdUseCase,
                _atualizarFuncionarioUseCase, _deletarFuncionarioUseCase);

            // Act
            var result = await controller.CriarFuncionario(funcionarioInput);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);

            // Verifica se a mensagem de erro contém os erros de senha fraca
            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(badRequestResult.Value);
            Assert.Contains("A senha deve ter pelo menos 8 caracteres.", errors);
            Assert.Contains("A senha deve conter pelo menos uma letra maiúscula (A-Z).", errors);
        }

    }
}
