using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LRengine.httpHandler
{
    public class MyDecompressionHandler : DelegatingHandler {
        private readonly DecompressionMethods _decompressionMethods;

        private const string s_gzip = "gzip";
        private const string s_deflate = "deflate";
        private static StringWithQualityHeaderValue s_gzipHeaderValue = new StringWithQualityHeaderValue(s_gzip);
        private static StringWithQualityHeaderValue s_deflateHeaderValue = new StringWithQualityHeaderValue(s_deflate);

        public MyDecompressionHandler(DecompressionMethods decompressionMethods, HttpMessageHandler innerHandler) : base(innerHandler) {
          
            if (decompressionMethods == DecompressionMethods.None) {
                throw new ArgumentOutOfRangeException(nameof(decompressionMethods));
            }
            _decompressionMethods = decompressionMethods;
        }

        internal bool GZipEnabled => (_decompressionMethods & DecompressionMethods.GZip) != 0;
        internal bool DeflateEnabled => (_decompressionMethods & DecompressionMethods.Deflate) != 0;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (GZipEnabled) {
                request.Headers.AcceptEncoding.Add(s_gzipHeaderValue);
            }
            if (DeflateEnabled) {
                request.Headers.AcceptEncoding.Add(s_deflateHeaderValue);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            while (true) {
                // Get first encoding
                using (IEnumerator<string> e = response.Content.Headers.ContentEncoding.GetEnumerator()) {
                    if (!e.MoveNext()) {
                        break;
                    }

                    string encoding = e.Current;
                    if (GZipEnabled && encoding == s_gzip) {
                        response.Content = new GZipDecompressedContent(response.Content);
                    } else if (DeflateEnabled && encoding == s_deflate) {
                        response.Content = new DeflateDecompressedContent(response.Content);
                    } else {
                        // Unknown content encoding.  Stop processing.
                        break;
                    }

                    
                }
            }


            return response;
        }

  
        private abstract class MyDecompressedContent : HttpContent {
            HttpContent _originalContent;
            bool _contentConsumed;

            public MyDecompressedContent(HttpContent originalContent) {
                _originalContent = originalContent;
                _contentConsumed = false;

                // Copy original response headers, but with the following changes:
                //   Content-Length is removed, since it no longer applies to the decompressed content
                //   The first Content-Encoding is removed, since we are processing that here
                foreach(var header in originalContent.Headers) {
                    Headers.Add(header.Key,header.Value);
                }
                //Headers.AddHeaders(originalContent.Headers);
                Headers.ContentLength = null;
                Headers.ContentEncoding.Clear();
                bool first = true;
                foreach (var encoding in originalContent.Headers.ContentEncoding) {
                    if (first) {
                        first = false;
                    } else {
                        Headers.ContentEncoding.Add(encoding);
                    }
                }
            }

          

            protected abstract Stream GetDecompressedStream(Stream originalStream);

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context) {
                using (Stream decompressedStream = await CreateContentReadStreamAsync().ConfigureAwait(false)) {
                    await decompressedStream.CopyToAsync(stream).ConfigureAwait(false);
                }
            }

            protected override async Task<Stream> CreateContentReadStreamAsync() {
                if (_contentConsumed) {
                    throw new InvalidOperationException("SR.net_http_content_stream_already_read");
                }

                _contentConsumed = true;
                Stream originalStream = await _originalContent.ReadAsStreamAsync().ConfigureAwait(false);
                return GetDecompressedStream(originalStream);
            }

            protected  override bool TryComputeLength(out long length) {
                length = 0;
                return false;
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    _originalContent.Dispose();
                }
                base.Dispose(disposing);
            }

            //获得原HttpContent
            public HttpContent getOriginalContent() {
                return _originalContent;
            }
        }




        private sealed class GZipDecompressedContent : MyDecompressedContent {
            public GZipDecompressedContent(HttpContent originalContent)
                : base(originalContent) { }

            protected override Stream GetDecompressedStream(Stream originalStream) =>
                new GZipStream(originalStream, CompressionMode.Decompress);
        }

        private sealed class DeflateDecompressedContent : MyDecompressedContent {
            public DeflateDecompressedContent(HttpContent originalContent)
                : base(originalContent) { }

            protected override Stream GetDecompressedStream(Stream originalStream) =>
                new DeflateStream(originalStream, CompressionMode.Decompress);
        }
    }
}
