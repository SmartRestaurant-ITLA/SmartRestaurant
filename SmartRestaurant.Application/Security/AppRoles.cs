namespace SmartRestaurant.Application.Security;

public static class AppRoles
{
    public const string Administrador = "Administrador";
    public const string Gerente = "Gerente";
    public const string Cocina = "Cocina";
    public const string Mesero = "Mesero";
    public const string Cliente = "Cliente";

    public const string AdministradorOGerente = Administrador + "," + Gerente;

    public static readonly IReadOnlyList<string> All =
        [Administrador, Gerente, Cocina, Mesero, Cliente];
}