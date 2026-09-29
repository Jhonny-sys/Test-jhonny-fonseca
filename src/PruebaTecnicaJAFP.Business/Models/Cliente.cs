namespace PruebaTecnicaJAFP.Business.Models;

public sealed record Cliente(
    int ClnId,
    short ClnTipoId,
    string ClnNumeroIdentificacion,
    string ClnRazonSocial,
    short ClnPaisCodigo,
    int ClnDptColCodigoDane,
    int ClnDvsPltColCodigoDane);

public sealed record ClienteInput(
    short ClnTipoId,
    string ClnNumeroIdentificacion,
    string ClnRazonSocial,
    short ClnPaisCodigo,
    int ClnDptColCodigoDane,
    int ClnDvsPltColCodigoDane);

public sealed record CatalogoItem(int Codigo, string Nombre);
