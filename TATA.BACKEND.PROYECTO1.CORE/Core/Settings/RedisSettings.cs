namespace TATA.BACKEND.PROYECTO1.CORE.Core.Settings
{
    public class RedisSettings
    {
        // host:puerto (sin parametros ssl=... embebidos: ver Ssl)
        public string ConnectionString { get; set; } = string.Empty;

        // ElastiCache en AWS exige TLS (transit_encryption_enabled); en local dev suele ser false.
        public bool Ssl { get; set; } = false;
    }
}
