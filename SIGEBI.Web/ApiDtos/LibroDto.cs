namespace SIGEBI.Web.ApiDtos
{
    public class LibroDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Ubicacion { get; set; }
        public int EjemplaresTotales { get; set; }
        public int EjemplaresDisponibles { get; set; }

        // 👇 Cambiar de string a int (para que coincida con la API)
        public int Estado { get; set; }

        public DateTime? FechaPublicacionUtc { get; set; }
        // Si necesitas CreadoUtc / ActualizadoUtc, puedes agregarlos también:
        // public DateTime CreadoUtc { get; set; }
        // public DateTime? ActualizadoUtc { get; set; }
    }

    public class CreateUpdateLibroDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Ubicacion { get; set; }
        public int EjemplaresTotales { get; set; }
        public DateTime? FechaPublicacionUtc { get; set; }
    }
}
