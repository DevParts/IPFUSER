using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LaserMacsaUser.Common
{
    public class CodeCache
    {
        private readonly Dictionary<string, CacheEntry> _cache;
        private readonly int _maxSize;
        private readonly TimeSpan _ttl;
        private readonly object _lockObject = new object();
        private readonly System.Threading.Timer _cleanupTimer;

        public CodeCache(int maxSize = 1000, TimeSpan? ttl = null)
        {
            _maxSize = maxSize;
            _ttl = ttl ?? TimeSpan.FromMinutes(10);
            _cache = new Dictionary<string, CacheEntry>();
            
            // Limpiar caché cada minuto
            _cleanupTimer = new System.Threading.Timer(CleanupExpiredEntries, null, 
                TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void Add(string key, string code, int jobId)
        {
            lock (_lockObject)
            {
                // Si existe, actualizar
                if (_cache.ContainsKey(key))
                {
                    _cache[key] = new CacheEntry
                    {
                        Code = code,
                        JobId = jobId,
                        LastAccessed = DateTime.Now,
                        CreatedAt = _cache[key].CreatedAt
                    };
                    return;
                }

                // Si el caché está lleno, eliminar el menos usado
                if (_cache.Count >= _maxSize)
                {
                    var oldest = _cache.OrderBy(x => x.Value.LastAccessed)
                                      .First();
                    _cache.Remove(oldest.Key);
                }

                _cache[key] = new CacheEntry
                {
                    Code = code,
                    JobId = jobId,
                    LastAccessed = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
            }
        }

        public string? Get(string key, int jobId)
        {
            lock (_lockObject)
            {
                if (_cache.TryGetValue(key, out CacheEntry? entry))
                {
                    // Verificar que no esté expirado
                    if (DateTime.Now - entry.CreatedAt > _ttl)
                    {
                        _cache.Remove(key);
                        return null;
                    }

                    // Verificar que sea del mismo job
                    if (entry.JobId != jobId)
                    {
                        return null;
                    }

                    // Actualizar último acceso
                    entry.LastAccessed = DateTime.Now;
                    return entry.Code;
                }

                return null;
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _cache.Clear();
            }
        }

        public void ClearByJobId(int jobId)
        {
            lock (_lockObject)
            {
                var keysToRemove = _cache
                    .Where(x => x.Value.JobId == jobId)
                    .Select(x => x.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
        }

        private void CleanupExpiredEntries(object? state)
        {
            lock (_lockObject)
            {
                var expiredKeys = _cache
                    .Where(x => DateTime.Now - x.Value.CreatedAt > _ttl)
                    .Select(x => x.Key)
                    .ToList();

                foreach (var key in expiredKeys)
                {
                    _cache.Remove(key);
                }
            }
        }

        private class CacheEntry
        {
            public string Code { get; set; } = string.Empty;
            public int JobId { get; set; }
            public DateTime LastAccessed { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}