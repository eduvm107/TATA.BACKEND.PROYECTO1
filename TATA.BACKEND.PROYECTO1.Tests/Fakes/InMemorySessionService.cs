using System.Collections.Concurrent;
using TATA.BACKEND.PROYECTO1.CORE.Core.Interfaces;

namespace TATA.BACKEND.PROYECTO1.Tests.Fakes;

/// <summary>
/// Sustituto de RedisSessionService solo para las pruebas de integración: evita que
/// WebApplicationFactory necesite una instancia real de Redis arriba. Replica la misma
/// semántica (sesión 1800s, ticket de un solo uso con TTL 60s) con un diccionario en memoria.
/// No se usa en producción: ahí sigue mandando RedisSessionService (con Redis real).
/// </summary>
public class InMemorySessionService : ISessionService
{
    private const int SesionTtlSegundos = 1800;
    private const int TicketTtlSegundos = 60;

    private record Entrada(string Valor, DateTimeOffset Expira);

    private readonly ConcurrentDictionary<string, Entrada> _store = new();

    public Task<SesionCreada> CrearSesionAsync(int userId, string jti)
    {
        var ahora = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expira = ahora + SesionTtlSegundos;

        _store[$"SLI_SESSION_{jti}"] = new Entrada(
            $"{userId}", DateTimeOffset.UtcNow.AddSeconds(SesionTtlSegundos));

        var ticket = Guid.NewGuid().ToString("N");
        _store[$"SLI_TICKET_{ticket}"] = new Entrada(
            jti, DateTimeOffset.UtcNow.AddSeconds(TicketTtlSegundos));

        return Task.FromResult(new SesionCreada(ticket, ahora, expira));
    }

    public Task EliminarSesionAsync(string jti)
    {
        _store.TryRemove($"SLI_SESSION_{jti}", out _);
        return Task.CompletedTask;
    }

    public Task<string?> CanjearTicketAsync(string ticket)
    {
        if (_store.TryRemove($"SLI_TICKET_{ticket}", out var entrada) && entrada.Expira > DateTimeOffset.UtcNow)
        {
            return Task.FromResult<string?>(entrada.Valor);
        }

        return Task.FromResult<string?>(null);
    }
}
