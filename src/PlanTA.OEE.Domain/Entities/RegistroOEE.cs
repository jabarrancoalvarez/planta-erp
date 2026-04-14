using PlanTA.OEE.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.OEE.Domain.Entities;

public class RegistroOEE : BaseEntity<RegistroOEEId>
{
    private RegistroOEE() { }

    public Guid ActivoId { get; private set; }
    public Guid? TurnoId { get; private set; }
    public Guid? OrdenFabricacionId { get; private set; }
    public DateTimeOffset Fecha { get; private set; }

    public int MinutosPlanificados { get; private set; }
    public int MinutosFuncionamiento { get; private set; }
    public int PiezasTotales { get; private set; }
    public int PiezasBuenas { get; private set; }
    public decimal TiempoCicloTeoricoSeg { get; private set; }

    public decimal Disponibilidad { get; private set; }
    public decimal Rendimiento { get; private set; }
    public decimal Calidad { get; private set; }
    public decimal OEE { get; private set; }

    public string? Notas { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static Result<RegistroOEE> Crear(
        Guid activoId, DateTimeOffset fecha,
        int minutosPlanificados, int minutosFuncionamiento,
        int piezasTotales, int piezasBuenas, decimal tiempoCicloTeoricoSeg,
        Guid empresaId, Guid? turnoId = null, Guid? ordenFabricacionId = null,
        string? notas = null)
    {
        if (minutosFuncionamiento > minutosPlanificados)
            return Result<RegistroOEE>.Failure(RegistroOEEErrors.TiemposInvalidos);
        if (piezasBuenas > piezasTotales)
            return Result<RegistroOEE>.Failure(RegistroOEEErrors.PiezasInvalidas);

        var disp = minutosPlanificados > 0
            ? Math.Round((decimal)minutosFuncionamiento / minutosPlanificados, 4)
            : 0;

        var segundosFuncionamiento = minutosFuncionamiento * 60m;
        var piezasTeoricas = tiempoCicloTeoricoSeg > 0 ? segundosFuncionamiento / tiempoCicloTeoricoSeg : 0;
        var rend = piezasTeoricas > 0
            ? Math.Round(Math.Min(piezasTotales / piezasTeoricas, 1m), 4)
            : 0;

        var cal = piezasTotales > 0
            ? Math.Round((decimal)piezasBuenas / piezasTotales, 4)
            : 0;

        var oee = Math.Round(disp * rend * cal, 4);

        return Result<RegistroOEE>.Success(new RegistroOEE
        {
            Id = new RegistroOEEId(Guid.NewGuid()),
            ActivoId = activoId,
            TurnoId = turnoId,
            OrdenFabricacionId = ordenFabricacionId,
            Fecha = fecha,
            MinutosPlanificados = minutosPlanificados,
            MinutosFuncionamiento = minutosFuncionamiento,
            PiezasTotales = piezasTotales,
            PiezasBuenas = piezasBuenas,
            TiempoCicloTeoricoSeg = tiempoCicloTeoricoSeg,
            Disponibilidad = disp,
            Rendimiento = rend,
            Calidad = cal,
            OEE = oee,
            Notas = notas,
            EmpresaId = empresaId
        });
    }
}
