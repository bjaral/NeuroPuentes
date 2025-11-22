using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{

    public class Stats_Usuario
    {
        public int Id { get; set; }
        
        public required int UsuarioId { get; set; }

        public required DateTime FechaCorte { get; set; }
        public required int TotalEntrevistas { get; set; }
        public required float TiempoTotalMin { get; set; }
        public required float ScorePromedio { get; set; }

    }

}