using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace RevAssuranceApi.WebServices
{
      public class WebServiceClient
    {
        #region Member Variables
        private string _soapEnvelope =
        @"<soap:Envelope
            xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
            xmlns:xsd='http://www.w3.org/2001/XMLSchema'
            xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
        <soap:Body></soap:Body></soap:Envelope>";
        #endregion

        #region Properties
        public List<Parameter> Parameters { set; get; }
        public string Url { get; set; }
        public string WCFContractName { get; set; }
        public string WebMethod { get; set; }
        public ServiceType WSServiceType { get; set; }
        #endregion

        #region Private Methods
        private string CreateSoapEnvelope()
        {
            string MethodCall = "<" + this.WebMethod + @" xmlns=""http://tempuri.org/"">";
            string StrParameters = string.Empty;
            foreach (Parameter param in this.Parameters)
            {
                StrParameters = StrParameters + "<" + param.Name + ">" + param.Value + "</" + param.Name + ">";
            }
            MethodCall = MethodCall + StrParameters + "</" + this.WebMethod + ">";
            StringBuilder sb = new StringBuilder(_soapEnvelope);
            sb.Insert(sb.ToString().IndexOf("</soap:Body>"), MethodCall);
            return sb.ToString();
        }
        private HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.Url);
            if (this.WSServiceType == WebServiceClient.ServiceType.WCF)
                webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + this.WCFContractName + "/" + this.WebMethod + "\"");
            else
                webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + this.WebMethod + "\"");
            webRequest.Headers.Add("To", this.Url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private string StripResponse(string SoapResponse)
        {
            string RegexExtract = @"<" + this.WebMethod + "Result>(?<Result>.*?)</" + this.WebMethod + "Result>";

            return Regex.Match(SoapResponse, RegexExtract).Groups["Result"].Captures[0].Value;
        }
        #endregion

        #region Public Methods
        public void BeginInvokeService(AsyncCallback InvokeCompleted)
        {
            DelegateInvokeService Invoke = new DelegateInvokeService(this.InvokeService);

            IAsyncResult result = Invoke.BeginInvoke(InvokeCompleted, null);
        }
        // public string EndInvokeService(IAsyncResult result)
        // {
        //     var asyncResult = (AsyncResult)result;

        //     ReturnMessage msg = (ReturnMessage)asyncResult.GetReplyMessage();



        //     return msg.ReturnValue.ToString();
        // }

        public string InvokeService()
        {
            try
            {
                WebResponse response = null;
                string strResponse = "";
                //Create the request
                HttpWebRequest req = this.CreateWebRequest();
                //write the soap envelope to request stream
                using (Stream stm = req.GetRequestStream())
                {
                    using (StreamWriter stmw = new StreamWriter(stm))
                    {
                        stmw.Write(this.CreateSoapEnvelope());
                    }
                }
                //get the response from the web service
                response = req.GetResponse();
                Stream str = response.GetResponseStream();
                StreamReader sr = new StreamReader(str);
                strResponse = sr.ReadToEnd();
                return strResponse;
            }
            catch (Exception ex)
            {
                var exM = ex;
            }
            return null;
        }
        #endregion

        #region Delegates
        public delegate string DelegateInvokeService();
        #endregion

        #region Enumerators
        public enum ServiceType
        {
            Traditional = 0,
            WCF = 1
        }
        #endregion

        #region Classes
        public class Parameter
        {
            public string Name { set; get; }
            public string Value { set; get; }
        }
        #endregion

    }
}