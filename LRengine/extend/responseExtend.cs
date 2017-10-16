using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LRengine.extend
{
    public static class responseExtend {
        public static string RawHeader(this HttpResponseMessage response) {
            return response.Headers.ToString() + response.Content.Headers.ToString();
        }

        public static async Task<string> RawBodyAsync(this HttpResponseMessage response) {
            Stream responseStream = await response.Content.ReadAsStreamAsync();
            if (response.Content.Headers.ContentEncoding.Any(i => string.Equals("gzip", i, StringComparison.OrdinalIgnoreCase))) {
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            }

            using (var reader = new StreamReader(responseStream)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
