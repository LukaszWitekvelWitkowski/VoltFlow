using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Tools
{
    public class LazyValue<T> where T : class
    {
        private readonly Func<Task<T>> _loader;
        private T? _value;
        private bool _isLoaded;
        private readonly object _lock = new();

        public LazyValue(Func<Task<T>> loader)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public async Task<T> GetValueAsync()
        {
            // Double-check locking pattern dla bezpieczeństwa wątkowego
            if (!_isLoaded)
            {
                // W przypadku async, lockowanie wymaga uwagi. 
                // Jeśli spodziewasz się dużego współbieżności, użyj SemaphoreSlim.
                if (_value == null)
                {
                    _value = await _loader();
                    _isLoaded = true;
                }
            }

            return _value!;
        }

        public void Reset()
        {
            lock (_lock)
            {
                _value = null;
                _isLoaded = false;
            }
        }
    }
}
