using System;
using System.Data;
using System.Threading.Tasks;
#if NET45
      //TODO
#elif NETCOREAPP2_1
using Microsoft.AspNetCore.Http;
#endif


namespace SqlSyringe.Standard {
    /// <summary>The Syringe middleware</summary>
    public partial class Syringe {
        /// <summary>
        /// Accepts the options or throws and Exception if not valid.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="System.ArgumentNullException">
        /// options - The Syringe options are mandatory.
        /// or
        /// options - The Syringe FromIp option is mandatory.
        /// </exception>
        private void AcceptOptions(InjectionOptions options) {
            _options = options ?? throw new ArgumentNullException(nameof(options), "The Syringe options are mandatory.");

            if (options.FromIp == null)
            {
                throw new ArgumentNullException(nameof(options), "The Syringe FromIp option is mandatory.");
            }
        }
    }
}