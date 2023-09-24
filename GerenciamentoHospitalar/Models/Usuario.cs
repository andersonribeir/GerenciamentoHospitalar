using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GerenciamentoHospitalar.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }


        public int TipoUsuario { get; set; } //1 para paciente, 2 para médico;
        // Outros campos de perfil, se necessário


    }
    public class UsuarioDTO
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int TipoUsuario { get; set; }
        // Outras propriedades, se necessário
    }
}