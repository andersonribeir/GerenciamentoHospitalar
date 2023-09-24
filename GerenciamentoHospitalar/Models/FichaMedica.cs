namespace GerenciamentoHospitalar.Models
{
    public class FichaMedica
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string UsuarioId { get; set; } // Chave estrangeira para o usuário
        public Usuario Usuario { get; set; } // Propriedade de navegação para o usuário
        // Outros atributos das fichas médicas, como histórico médico, exames, etc.
    }
}
