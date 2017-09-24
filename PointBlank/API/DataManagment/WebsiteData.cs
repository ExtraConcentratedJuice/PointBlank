﻿using System;
using System.Net;
using System.Net.Security;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Text;

namespace PointBlank.API.DataManagment
{
    /// <summary>
    /// Allows for easy interaction of websites
    /// </summary>
    public class WebsiteData
    {
        #region Public Functions
        /// <summary>
        /// Downloads the web page
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="data">The web page as a string</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the data was gotten successfully</returns>
        public static bool GetData(string url, out string data, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    using(WeebClient wc = new WeebClient())
                    {
                        data = wc.DownloadString(url);
                        return true;
                    }
                }

                data = client.DownloadString(url);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Could not get the data from URL! " + url, ex, false, false);
                data = null;
                return false;
            }
        }

        /// <summary>
        /// Downloads the web page using async
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="method">The method to run once the download is done</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the data was gotten successfully</returns>
        public static bool GetDataAsync(string url, DownloadStringCompletedEventHandler method, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    WeebClient wc = new WeebClient();

                    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadString);
                    wc.DownloadStringCompleted += method;
                    wc.DownloadStringAsync(new Uri(url));
                    return true;
                }

                client.DownloadStringCompleted += method;
                client.DownloadStringAsync(new Uri(url));
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to get the data using async from " + url, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Posts data to the website and downloads the response page
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="postData">The data to send as post request</param>
        /// <param name="returnData">The response page as string</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the data was posted successfully</returns>
        public static bool PostData(string url, NameValueCollection postData, out string returnData, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    using (WeebClient wc = new WeebClient())
                    {
                        returnData = Encoding.Unicode.GetString(wc.UploadValues(url, postData));
                        return true;
                    }
                }

                returnData = Encoding.Unicode.GetString(client.UploadValues(url, postData));
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to post data to " + url, ex, false, false);
                returnData = null;
                return false;
            }
        }

        /// <summary>
        /// Posts data to the website and downloads the response page using async
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="postData">The data to send as post request</param>
        /// <param name="method">The method to run once the post is done</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the data was posted successfully</returns>
        public static bool PosDataAsync(string url, NameValueCollection postData, UploadValuesCompletedEventHandler method, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    WeebClient wc = new WeebClient();

                    wc.UploadValuesCompleted += new UploadValuesCompletedEventHandler(OnUploadValues);
                    wc.UploadValuesCompleted += method;
                    wc.UploadValuesAsync(new Uri(url), postData);
                    return true;
                }

                client.UploadValuesAsync(new Uri(url), postData);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to post data via async to " + url, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Downloads a file from the website
        /// </summary>
        /// <param name="url">The URL to the file</param>
        /// <param name="path">The path to save the file at</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the file was downloaded successfully</returns>
        public static bool DownloadFile(string url, string path, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    using (WeebClient wc = new WeebClient())
                    {
                        wc.DownloadFile(url, path);
                        return true;
                    }
                }

                client.DownloadFile(url, path);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to download file from " + url, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Downloads a file from the website using async
        /// </summary>
        /// <param name="url">The URL to the file</param>
        /// <param name="path">The path to save the file at</param>
        /// <param name="method">The method to run once the download is done</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the file was downloaded successfully</returns>
        public static bool DownloadFileAsync(string url, string path, AsyncCompletedEventHandler method = null, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    WeebClient wc = new WeebClient();

                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadFile);
                    if (method != null)
                        wc.DownloadFileCompleted += method;
                    wc.DownloadFileAsync(new Uri(url), path);
                    return true;
                }

                client.DownloadFileAsync(new Uri(url), path);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to download file via async from " + url, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Uploads a file to the website
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="path">The path to the file to upload</param>
        /// <param name="data">The website's response</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the file was uploaded successfully</returns>
        public static bool UploadFile(string url, string path, out byte[] data, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    using (WeebClient wc = new WeebClient())
                    {
                        data = wc.UploadFile(url, path);
                        return true;
                    }
                }

                data = client.UploadFile(url, path);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to upload file to " + url, ex, false, false);
                data = null;
                return false;
            }
        }

        /// <summary>
        /// Uploads a file to the website using async
        /// </summary>
        /// <param name="url">The URL to the website</param>
        /// <param name="path">The path to the file to upload</param>
        /// <param name="method">The method to run once the upload is done</param>
        /// <param name="useNewClient">Use a new client</param>
        /// <param name="client">A custom client(leave null to create a new one)</param>
        /// <returns>If the file was uploaded successfully</returns>
        public static bool UploadFileAsync(string url, string path, UploadFileCompletedEventHandler method = null, bool useNewClient = true, WeebClient client = null)
        {
            try
            {
                if (useNewClient || client == null)
                {
                    WeebClient wc = new WeebClient();

                    wc.UploadFileCompleted += new UploadFileCompletedEventHandler(OnUploadFile);
                    if (method != null)
                        wc.UploadFileCompleted += method;
                    wc.UploadFileAsync(new Uri(url), path);
                    return true;
                }

                client.UploadFileAsync(new Uri(url), path);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Failed to upload file via async to " + url, ex, false, false);
                return false;
            }
        }
        #endregion

        #region Event Functions
        private static void OnDownloadString(object sender, DownloadStringCompletedEventArgs args)
        {
            WeebClient wc = (WeebClient)sender;

            wc.Dispose();
        }

        private static void OnUploadValues(object sender, UploadValuesCompletedEventArgs args)
        {
            WeebClient wc = (WeebClient)sender;

            wc.Dispose();
        }

        private static void OnDownloadFile(object sender, AsyncCompletedEventArgs args)
        {
            WeebClient wc = (WeebClient)sender;

            wc.Dispose();
        }

        private static void OnUploadFile(object sender, UploadFileCompletedEventArgs args)
        {
            WeebClient wc = (WeebClient)sender;

            wc.Dispose();
        }
        #endregion
    }

    /// <summary>
    /// A website browsing client that supports cookies and redirections
    /// </summary>
    public class WeebClient : WebClient
    {
        #region Properties
        /// <summary>
        /// The current site URL
        /// </summary>
        public Uri Url { get; private set; }
        /// <summary>
        /// The jar of cookies currently stored
        /// </summary>
        public CookieContainer CookieJar { get; private set; }
        #endregion

        /// <summary>
        /// A website browsing client that supports cookies and redirections
        /// </summary>
        /// <param name="cookieJar">The jar of cookies to send to the server</param>
        public WeebClient(CookieContainer cookieJar)
        {
            base.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            this.CookieJar = cookieJar;
        }

        /// <summary>
        /// A website browsing client that supports cookies and redirections
        /// </summary>
        public WeebClient()
        {
            base.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            this.CookieJar = new CookieContainer();
        }

        #region Overrides
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);

            Url = response.ResponseUri;

            return response;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(PointBlank.ValidateCertificate);

            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
                ((HttpWebRequest)request).CookieContainer = CookieJar;
            ((HttpWebRequest)request).AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }
        #endregion
    }
}
