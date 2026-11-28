using TATA.BACKEND.PROYECTO1.CORE.Core.Entities;
using TATA.BACKEND.PROYECTO1.CORE.Core.Settings;

namespace TATA.BACKEND.PROYECTO1.CORE.Core.Interfaces
{
    public interface IJWTService
    {
        JWTSettings _settings { get; }

        /// <summary>
        /// Genera el JWT y devuelve tambien su jti (identificador unico del token),
        /// usado para crear/eliminar la sesion en Redis.
        /// </summary>
        (string Token, string Jti) GenerateJWToken(Usuario usuario);
    }
}
