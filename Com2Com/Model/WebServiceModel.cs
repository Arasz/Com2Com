using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace Com2Com.Model
{
    class WebServiceModel : IDisposable
    {
        /// <summary>
        /// HTTP client used to interact with web server
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Web service address
        /// </summary>
        private string _serviceAddress = @"http://localhost/retrieve.php";

        public WebServiceModel()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Gets JSON array from web server and converts it to SlaveModel collection
        /// </summary>
        /// <returns>Received collection of SlaveModels</returns>
        public async Task<IEnumerable<SlaveModel>> GetSlaveListAsync()
        {
            // TODO: Add exception handling
            // Read server response 
            HttpResponseMessage response = await _httpClient.GetAsync(_serviceAddress, HttpCompletionOption.ResponseContentRead);

            response.EnsureSuccessStatusCode();

            // Read response as a string
            string strContent = await response.Content.ReadAsStringAsync();
            strContent = ExtractJSON(strContent);
            // Deserialize objects
            return JsonConvert.DeserializeObject<List<SlaveModel>>(strContent);
        }

        /// <summary>
        /// Sends SlaveModels collection as JSON array to the HTTP server. 
        /// </summary>
        /// <param name="slaves">Slaves data to send</param>
        /// <returns></returns>
        public async Task PostSlaveListAsync(IEnumerable<SlaveModel> slaves)
        {
            // TODO: Add exception handling
            // Create HTTP request string with serialized JSON array of slave models
            string requestString = $"?slaves={JsonConvert.SerializeObject(slaves)}";
            // Pass this string to the HTTP request message 
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _serviceAddress);
            await _httpClient.SendAsync(message);
        }

        /// <summary>
        /// Extracts JSON array from text/HTML HTTP server response.
        /// </summary>
        /// <exception cref="ArgumentException">Throws when string doesn't contains JSON array</exception>
        /// <param name="htmlString">String with HTTP server response formated as HTML</param>
        /// <returns>JSON array string</returns>
        private string ExtractJSON(string htmlString)
        {
            if (!htmlString.Contains("["))
                throw new ArgumentException("No JSON array inside HTML string.", nameof(htmlString));

            int startIndex = htmlString.IndexOf('[');
            int endIndex = htmlString.LastIndexOf(']');
            int length = endIndex - startIndex;
            return htmlString.Substring(startIndex, length + 1);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~WebServiceModel() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
