using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Diagnostics;

namespace Com2Com.Model
{
    class WebServerSynchronizationEventArgs: EventArgs
    {
        public IEnumerable<SlaveModel> SlavesStates
        {
            get; set;
        }

        public WebServerSynchronizationEventArgs(IEnumerable<SlaveModel> slavesState)
        {
            SlavesStates = slavesState;
        }
    }

    class WebServiceModel : IDisposable
    {
        #region Events
        /// <summary>
        /// Occurs when data from server is received
        /// </summary>
        public event EventHandler<WebServerSynchronizationEventArgs> ServerDataReceived;

        void OnServerDataReceived(IEnumerable<SlaveModel> slavesState)
        {
            var eventHandler = ServerDataReceived;

            eventHandler?.Invoke(this, new WebServerSynchronizationEventArgs(slavesState));
        }

        private async void _syncTimer_TickAsync(object sender, EventArgs e)
        {
            _receivedSlavesState = await GetSlaveListAsync();

            if (_dataChanged)
                OnServerDataReceived(_updatedSlavesStates);

            _lastReceivedSlavesState = _receivedSlavesState;
        }

        #endregion

        /// <summary>
        /// HTTP client used to interact with web server
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Web service address
        /// </summary>
        private string _serviceAddress = @"http://localhost/retrieve.php";

        /// <summary>
        /// Timer used for getting data from web server in regular intervals
        /// </summary>
        private DispatcherTimer _syncTimer;

        private IEnumerable<SlaveModel> _lastReceivedSlavesState = new List<SlaveModel>();
        private IEnumerable<SlaveModel> _receivedSlavesState;


        /// <summary>
        /// Indicates that data received from server has changed
        /// </summary>
        private bool _dataChanged
        {
            get
            {
                return _updatedSlavesStates?.Any() ?? true;
            }
        }

        private IEnumerable<SlaveModel> _updatedSlavesStates
        {
            get { return _receivedSlavesState?.Except(_lastReceivedSlavesState); }
        }

        /// <summary>
        ///  Synchronization rate
        /// </summary>
        public TimeSpan SyncInterval { get; set; } = TimeSpan.FromSeconds(15);

        public WebServiceModel()
        {
            _httpClient = new HttpClient();
            _syncTimer = new DispatcherTimer() { Interval = SyncInterval, };

            _syncTimer.Tick += _syncTimer_TickAsync;
        }

        /// <summary>
        /// Begins synchronization with web server
        /// </summary>
        /// <param name="syncInterval">Synchronization interval</param>
        public void RunWebServerSync(TimeSpan? syncInterval = null)
        {
            if (syncInterval != null)
                _syncTimer.Interval = syncInterval.Value;
            _syncTimer.Start();
        }

        /// <summary>
        /// Stops web server synchronization
        /// </summary>
        public void StopWebServerSync() => _syncTimer.Stop();


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

            Debug.Print($"Get response: {response.Content.ReadAsStringAsync().Result} ");

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
            //string requestString = $"?slaves={JsonConvert.SerializeObject(slaves)}";
            string requestString = $"{JsonConvert.SerializeObject(slaves)}";
            // Pass this string to the HTTP request message 
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, _serviceAddress)
            {
                Content = new FormUrlEncodedContent( new Dictionary<string, string> { ["slaves"]= requestString }),
            };
            var response = await _httpClient.SendAsync(message);

            Debug.Print($"Post response: {response.Content.ReadAsStringAsync().Result}");
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
