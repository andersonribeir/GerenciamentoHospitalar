using Microsoft.AspNetCore.Mvc;
using GerenciamentoHospitalar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GerenciamentoHospitalar.Controllers
{
    [Route("api/fichamedica")]
    [ApiController]
    [Authorize] // Protege todo o controlador com autenticação JWT
    public class FichaMedicaController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public FichaMedicaController(UserManager<Usuario> userManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        // Adicionar uma ficha médica
        [HttpPost("cadastro")]
        public async Task<IActionResult> AdicionarFichaMedica([FromBody] FichaMedica fichaMedica)
        {
            try
            {
                var userName = User.Identity.Name; // Obtém o nome do usuário autenticado

                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    fichaMedica.UsuarioId = user.Id;

                    _context.FichasMedicas.Add(fichaMedica);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Ficha médica adicionada com sucesso" });
                }

                return BadRequest(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
            }
        }

        // Excluir uma ficha médica com base no ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> ExcluirFichaMedica(int id)
        {
            try
            {
                var userName = User.Identity.Name; // Obtém o nome do usuário autenticado

                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    var fichaMedica = await _context.FichasMedicas.FirstOrDefaultAsync(f => f.Id == id && f.UsuarioId == user.Id);

                    if (fichaMedica != null)
                    {
                        _context.FichasMedicas.Remove(fichaMedica);
                        await _context.SaveChangesAsync();

                        return Ok(new { message = "Ficha médica excluída com sucesso" });
                    }

                    return NotFound(new { message = "Ficha médica não encontrada" });
                }

                return BadRequest(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
            }
        }

        // Alterar uma ficha médica com base no ID
        [HttpPut("update/{id}")]
        public async Task<IActionResult> AlterarFichaMedica(int id, [FromBody] FichaMedica fichaMedica)
        {
            try
            {
                var userName = User.Identity.Name; // Obtém o nome do usuário autenticado

                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    var existingFichaMedica = await _context.FichasMedicas.FirstOrDefaultAsync(f => f.Id == id && f.UsuarioId == user.Id);

                    if (existingFichaMedica != null)
                    {
                        // Atualize as propriedades da ficha médica com os valores do modelo enviado no corpo da solicitação
                        existingFichaMedica.Nome = fichaMedica.Nome;
                        // Atualize outras propriedades conforme necessário

                        await _context.SaveChangesAsync();

                        return Ok(new { message = "Ficha médica alterada com sucesso" });
                    }

                    return NotFound(new { message = "Ficha médica não encontrada" });
                }

                return BadRequest(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
            }
        }

        // Listar todas as fichas médicas
        [HttpGet("list")]
        public async Task<IActionResult> ListarFichasMedicas()
        {
            try
            {
                var userName = User.Identity.Name; // Obtém o nome do usuário autenticado

                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    var fichasMedicas = await _context.FichasMedicas.ToListAsync();
                    var fichasSecretas = new List<FichaMedica>();
                    foreach (FichaMedica ficha in fichasMedicas)
                    {
                        ficha.Usuario= null;
                        fichasSecretas.Add(ficha);
                    }


                    return Ok(fichasMedicas);
                }

                return BadRequest(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
            }
        }

        // Listar fichas médicas de um usuário específico
        [HttpGet("list/{id}")]
        public async Task<IActionResult> ListarFichasMedicasPorUsuario(string id)
        {
            try
            {
                var fichasMedicas = await _context.FichasMedicas.Where(f => f.UsuarioId == id).ToListAsync();

                return Ok(fichasMedicas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
            }
        }
    }
}
