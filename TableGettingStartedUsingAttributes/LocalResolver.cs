namespace TableGettingStartedUsingAttributes
{
    using Microsoft.Azure.KeyVault.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class LocalResolver : IKeyResolver
    {
        private Dictionary<string, IKey> keys = new Dictionary<string, IKey>();

        public void Add(IKey key)
        {
            keys[key.Kid] = key;
        }

        public async Task<IKey> ResolveKeyAsync(string kid, CancellationToken token)
        {
            IKey result;

            keys.TryGetValue(kid, out result);

            return await Task.FromResult(result);
        }
    }
}
