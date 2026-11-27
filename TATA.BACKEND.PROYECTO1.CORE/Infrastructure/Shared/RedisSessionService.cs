using StackExchange.Redis;
using System.Text.Json;
using TATA.BACKEND.PROYECTO1.CORE.Core.Interfaces;

namespace TATA.BACKEND.PROYECTO1.CORE.Infrastructure.Shared
{
    public class RedisSessionService : ISessionService
    {
        // RFC seguridad de sesion: sesion 1800s (30 min), ticket de un solo uso 60s
        private const int SesionTtlSegundos = 1800;
        private const int TicketTtlSegundos = 60;

        private readonly IConnectionMultiplexer _redis;

        public RedisSessionService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private IDatabase Db => _redis.GetDatabase();

        public async Task<SesionCreada> CrearSesionAsync(int userId, string jti)
        {
            var ahora = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var expira = ahora + SesionTtlSegundos;

            var sesion = JsonSerializer.Serialize(new
            {
                user_id = userId,
                jti,
                started_at = ahora,
                expires_at = expira,
            });

            await Db.StringSetAsync($"SLI_SESSION_{jti}", sesion, TimeSpan.FromSeconds(SesionTtlSegundos));

            var ticket = Guid.NewGuid().ToString("N");
            await Db.StringSetAsync($"SLI_TICKET_{ticket}", jti, TimeSpan.FromSeconds(TicketTtlSegundos));

            return new SesionCreada(ticket, ahora, expira);
        }

        public Task EliminarSesionAsync(string jti) =>
            Db.KeyDeleteAsync($"SLI_SESSION_{jti}");

        public async Task<string?> CanjearTicketAsync(string ticket)
        {
            var valor = await Db.StringGetDeleteAsync($"SLI_TICKET_{ticket}");
            return valor.IsNullOrEmpty ? null : valor.ToString();
        }
    }
}
