using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace ThingSharp.Bindings
{
    public class HTTPBinding : ProtocolBinding
    {
        private BackgroundWorker mListeningThread;
        private string[] prefixes;
        HttpListener listener;
        IBindingClient client;
        bool KEEP_LISTENING = true;
        public HTTPBinding(string[] prefixes)
        {
            this.prefixes = prefixes;
            mListeningThread = new BackgroundWorker();
            mListeningThread.DoWork += ListeningThread_DoWork;
        }

        public override void AddClient(IBindingClient client)
        {
            this.client = client;
        }

        private class ValueObject
        {
            public Object value { get; set; }
        }

        private class HypermediaLinks
        {
            public Object links;
        }

        private class UpdateResponse
        {
            public Object error { get; set; }
        }

        //private static void InstallCertificate(string cerFileName)
        //{
        //    X509Certificate2 certificate = new X509Certificate2(cerFileName);
        //    X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);

        //    store.Open(OpenFlags.ReadWrite);
        //    store.Add(certificate);
        //    store.Close();
        //}

        private long threadCount = 0;
        public void Listen()
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            Console.WriteLine("Listening...");
            //System.Diagnostics.Trace.WriteLine("Listening...");

            //listener.Start();

            ////////using (var client = listener.AcceptTcpClient())
            ////////using (var sslStream = new SslStream(client.GetStream(), false, App_CertificateValidation))
            ////////{
            ////////    sslStream.AuthenticateAsServer(serverCertificate, true, SslProtocols.Tls12, false);

            ////////    //send/receive from the sslStream
            ////////}

            while (KEEP_LISTENING)
            {
                listener.Start();
                if (!listener.IsListening)
                    break;

                try
                {
                    // Note: The GetContext method blocks while waiting for a request. 
                    HttpListenerContext context = listener.GetContext();

                    // Send the request off to the processing thread
                    HandleRequest(context);
                    threadCount++;

                    //HttpListenerRequest request = context.Request;
                    //Console.WriteLine("Thread Count: {0}  {1}", threadCount, request.Url);
                }
                catch (Exception e)
                {
                    // Got here because the program was closed while we where listening.
                    break;
                }
            }

        }

        private void HandleRequest(HttpListenerContext context)
        {
            var action = new Action(() => HandleRequestAsync(context));
            var task = Task.Factory.StartNew(action);
        }

        private void HandleRequestAsync(HttpListenerContext context)
        {
            try
            {

                Stopwatch sw = null;

                // Start timing the amount of time it takes for each request
                sw = Stopwatch.StartNew();

                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = "{\"Command\":\"Not Supported\"}";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                Object r;
                if (client != null)
                {
                    if (request.HttpMethod == "GET")
                    {
                        try
                        {
                            r = client.Read(request.Url);

                            if (r == null)
                            {
                                //Console.WriteLine("--Resource Not Responding");
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                                responseString = "{\"Resource\":\"Not Responding\"}";
                                buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                            }
                            else
                            {
                                if (!(r is Thing || r is List<HypermediaLink>))
                                {
                                    ValueObject valObj = new ValueObject() { value = r };
                                    r = JsonConvert.SerializeObject(valObj);
                                }
                                else if (r is Thing)
                                {
                                    r = JsonConvert.SerializeObject(r);
                                }
                                else if (r is List<HypermediaLink>)
                                {
                                    HypermediaLinks links = new HypermediaLinks() { links = r };
                                    r = JsonConvert.SerializeObject(links);
                                }
                                response.StatusCode = (int)HttpStatusCode.OK;
                            }
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("--INTERNAL ERROR!");
                            response.StatusCode = (int)GetStatusCodeForException(e);
                            r = e.Message;
                        }
                        if (r != null)
                        {
                            buffer = System.Text.Encoding.UTF8.GetBytes(r.ToString());
                        }
                    }
                    else if (request.HttpMethod == "PUT")
                    {
                        UpdateResponse ur = new UpdateResponse() { error = null };
                        try
                        {
                            TextReader reader = new StreamReader(request.InputStream);
                            String content = reader.ReadToEnd();
                            ValueObject valObj = JsonConvert.DeserializeObject<ValueObject>(content);
                            r = client.Write(request.Url, valObj.value);
                            if (r == null)
                            {
                                response.StatusCode = (int)HttpStatusCode.NotFound;
                                ur.error = "Not Responding";
                            }
                            else
                            {
                                if ((bool)r == true)
                                    response.StatusCode = (int)HttpStatusCode.Accepted;
                                else
                                    response.StatusCode = (int)HttpStatusCode.NotFound; // Error 404
                            }
                        }
                        catch (Exception e)
                        {
                            r = e.Message;
                            ur.error = e;
                            response.StatusCode = (int)GetStatusCodeForException(e);
                        }
                        String urs = JsonConvert.SerializeObject(ur);
                        buffer = System.Text.Encoding.UTF8.GetBytes(urs);
                    }
                }

                System.IO.Stream output = null;
                try
                {
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "application/json";
                    output = response.OutputStream;

                    if (output != null)
                    {
                        output.Write(buffer, 0, buffer.Length);
                        // You must close the output stream.
                        if (output != null)
                            output.Close();
                    }
                }
                catch (Exception e)
                {
                    // Usually get here during debugging if we take to long to responde.
                    // Just clode the stream and move on.
                    output.Close();
                }

                if (sw != null)
                {
                    sw.Stop();
                    //Console.WriteLine("RequestReceived -- Overall TimeElapsed: {0} ({1})", sw.Elapsed, threadCount);
                }

                threadCount--;
            }
            catch(Exception e)
            {
                Debug.WriteLine("{0}", e);
            }

        }

        public HttpStatusCode GetStatusCodeForException(Exception e)
        {
            if (e is Resource.ResourceNotFoundException)
                return HttpStatusCode.NotFound;
            else if (e is Resource.ResourceOperationNotAllowedException)
                return HttpStatusCode.MethodNotAllowed;
            else
                return HttpStatusCode.InternalServerError;
        }

        public override void StartListening()
        {
            mListeningThread.RunWorkerAsync();
        }

        private void ListeningThread_DoWork(object sender, DoWorkEventArgs e)
        {
            Listen();
        }

        public override void StopListening()
        {
            try
            {
                KEEP_LISTENING = false;
                listener.Stop();
                //mListeningThread.CancelAsync();
            }
            catch (Exception e)
            { }

        }
    }
}
