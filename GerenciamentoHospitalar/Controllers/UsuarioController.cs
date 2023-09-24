using Microsoft.AspNetCore.Mvc;
using GerenciamentoHospitalar.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace GerenciamentoHospitalar.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IConfiguration _configuration;
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifique se o username já existe
                    var existingUser = await _userManager.FindByNameAsync(usuario.UserName);
                    if (existingUser != null)
                    {
                        // Se o username já existe, retorne uma resposta de erro
                        return BadRequest(new { message = "Este email já está associado a uma conta." });
                    }

                    var result = await _userManager.CreateAsync(usuario, usuario.PasswordHash);

                    if (result.Succeeded)
                    {
                        // Usuário registrado com sucesso; você pode retornar uma resposta JSON
                        return Ok(new { message = "Usuário registrado com sucesso" });
                    }

                    // Se ocorrerem erros durante o registro, retorne detalhes dos erros em um objeto JSON
                    return BadRequest(new { errors = result.Errors });
                }
                catch (System.Exception ex)
                {
                    return StatusCode(500, new { message = "Ocorreu um erro interno no servidor. Erro: " + ex.Message });
                }
            }

            // Se o modelo não for válido, retorne os erros de validação
            return BadRequest(ModelState);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, usuario.PasswordHash, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(usuario.UserName);
                user.PasswordHash = "Secret";
                // Verifique se o usuário foi encontrado
                if (user == null)
                {
                    return Unauthorized(new { message = "Usuário não encontrado" });
                }

                // Obtém a chave secreta a partir da configuração
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                // Define as informações do token, como emissor, audiência e data de expiração
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(5), // Tempo de expiração do token (1 hora, ajuste conforme necessário)
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Retorne o token JWT juntamente com todos os dados do usuário
                return Ok(new
                {
                    usuario = user,
                    token = tokenString
                });
            }

            // Se o login falhar, retorne uma resposta de erro
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

    }
}
