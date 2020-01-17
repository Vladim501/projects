using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;

namespace WebApplication11
{
    public class KeyValueModule : NancyModule
    {
        private IKeyValueService storage;

        public KeyValueModule(IKeyValueService inject)
        {
            storage = inject;

            Get("/api/{key}", args =>
            {
                if (storage.GetEntry(args.key) is KeyValueClass entry)
                    return entry;
                return HttpStatusCode.NoContent;
            });

            Post("/api/{key}={Value}", args =>
            {
                KeyValueClass entry = this.Bind<KeyValueClass>();

                if (string.IsNullOrWhiteSpace(entry.Key) || string.IsNullOrWhiteSpace(entry.Value))
                {
                    return HttpStatusCode.BadRequest;
                }

                storage.AddEntry(entry.Key, entry.Value);

                return HttpStatusCode.OK;
            });

            Delete("/api/{key}", args =>
            {
                storage.RemoveEntry(args.key);
                return HttpStatusCode.OK;
            });
        }

    }
}
