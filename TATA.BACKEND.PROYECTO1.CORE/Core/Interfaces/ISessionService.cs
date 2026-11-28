namespace TATA.BACKEND.PROYECTO1.CORE.Core.Interfaces
{
    /// <summary>
    /// Sesion creada en Redis al hacer login: ticket de un solo uso (TTL 60s)
    /// y la ventana de validez de la sesion (TTL 1800s), en epoch segundos UTC.
    /// </summary>
    public record SesionCreada(string Ticket, long StartedAt, long ExpiresAt);

    public interface ISessionService
    {
        Task<SesionCreada> CrearSesionAsync(int userId, string jti);

        Task EliminarSesionAsync(string jti);

        /// <summary>
        /// Canjea (GETDEL) el ticket de un solo uso. Devuelve el jti asociado,
        /// o null si el ticket no existe, ya fue usado o expiro (TTL 60s).
        /// </summary>
        Task<string?> CanjearTicketAsync(string ticket);
    }
}
