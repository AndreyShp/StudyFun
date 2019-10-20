using System;
using System.Net;
using System.Web;

namespace StudyLanguages.Helpers {
    public static class RemoteClientHelper {
        public static string GetClientIpAddress(HttpRequestBase request) {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIp = "";

            if (szXForwardedFor == null) {
                szIp = szRemoteAddr;
            } else {
                szIp = szXForwardedFor;
                if (szIp.IndexOf(",", StringComparison.Ordinal) > 0) {
                    string[] arIPs = szIp.Split(',');

                    foreach (string item in arIPs) {
                        if (!IsPrivateIpAddress(item)) {
                            return item;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(szIp)) {
                szIp = request.ServerVariables["REMOTE_ADDR"];
            }
            return szIp;
        }

        public static string GetBrowserName(HttpRequestBase request) {
            string result = request.Browser != null ? request.Browser.Browser : null;
            if (string.IsNullOrEmpty(result)) {
                result = request.UserAgent;
            }
            return result;
        }

        private static bool IsPrivateIpAddress(string ipAddress) {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            IPAddress ip = IPAddress.Parse(ipAddress);
            byte[] octets = ip.GetAddressBytes();

            bool is24BitBlock = octets[0] == 10;
            if (is24BitBlock) {
                return true; // Return to prevent further processing
            }

            bool is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) {
                return true; // Return to prevent further processing
            }

            bool is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) {
                return true; // Return to prevent further processing
            }

            bool isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }
    }
}