using MAT05_Notas.Entities;

namespace MAT05_Notas.Services
{
    public interface INotasService
    {
        Task<IResult> Cargar_Desglose(DesgloseRubro desgloserubro);
        Task<IResult> Asignar_Actualizar_Nota(Notas notas);

        Task<IResult> Obtener_Desglose_Por_ID(string grupo, string curso);

        Task<IResult> Obtener_Notas_By_Id(string numero_identificacion, string curso);
    }
}
