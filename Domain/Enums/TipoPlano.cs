namespace AcademiaAgenda.Domain.Enums;

public enum TipoPlano
{
    Mensal = 1,
    Trimestral = 2,
    Anual = 3
}

public static class RegrasPlano
{
    public static int ObterLimiteMensal(TipoPlano plano) => plano switch
    {
        TipoPlano.Mensal => 12,
        TipoPlano.Trimestral => 20,
        TipoPlano.Anual => 30,
        _ => 0
    };
}