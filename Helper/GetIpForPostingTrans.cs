using System;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace RevAssuranceApi.Helper
{
    /*This class is use to get the Ip of server and compare it with the Encrypted
    server Ip which is set up in app settings. If the comparation is equal 
    then Transaction posting would be allowed hence, the transaction won't be posted
    */
    public class GetIpForPostingTrans
    {
        IConfiguration _IConfiguration;
        LogManager _LogManager;
        public GetIpForPostingTrans(IConfiguration IConfiguration, LogManager LogManager)
        {
            _IConfiguration = IConfiguration;
            _LogManager = LogManager;
        }
        public bool GetIp()
        {
            try
            {
                string hostName = Dns.GetHostName();

                _LogManager.SaveLog("hostName : " + hostName);

                string RuntimeIpAddress = Dns.GetHostByName(hostName).AddressList[1].ToString();

                _LogManager.SaveLog("RuntimeIpAddress : " + RuntimeIpAddress);
                // string compare = Cryptors.EncryptNoKey("192.168.1.65");
                // string IpSetUp =  Cryptors.DecryptNoKey(_IConfiguration["ipAddressForTransPosting"]);

                string IpSetUp = Cryptors.DecryptNoKey(_IConfiguration["ipAddressForTransPosting"]);

                _LogManager.SaveLog("IpSetUp : " + IpSetUp);
                if (RuntimeIpAddress.Equals(IpSetUp))
                {
                    return true;
                }
                else if (Dns.GetHostByName(hostName).AddressList[2].ToString() == IpSetUp)
                {
                    return true;
                }
                else if (Dns.GetHostByName(hostName).AddressList[3].ToString() == IpSetUp)
                {
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Internal Error When getting Ip to Post Transaction Error: {exM}");
                return false;
            }
        }
    }
}