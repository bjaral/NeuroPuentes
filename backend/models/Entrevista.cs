using System.ComponentModel.DataAnnotations;

namespace NeuroPuentesAPI.models
{

    public class Entrevistas
    {
        public int _id { get; set; }
        public required int Usuario_id { get; set; }
        public required int Contexto_id { get; set; }

        public required string Titulo { get; set; } = String.Empty;
        public required string Descripcion { get; set; } = String.Empty;

        public required float Duracion_min { get; set; }
        public required int Numero_turnos { get; set; }

        public required DateTime Fecha_creacion {get;set;}
        public DateTime? Fecha_cierre {get;set;}

        public required string contexto_snapshot {get;set;} = String.Empty;

    }

}