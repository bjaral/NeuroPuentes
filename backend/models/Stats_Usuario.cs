using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{

    public class Stats_Usuario
    {
        public int _id { get; set; }
        
        public required int Usuario_id { get; set; }

        public required DateTime Fecha_corte { get; set; }
        public required int Total_Entrevistas { get; set; }
        public required float Tiempo_total_min { get; set; }
        public required float Score_promedio { get; set; }

    }

}