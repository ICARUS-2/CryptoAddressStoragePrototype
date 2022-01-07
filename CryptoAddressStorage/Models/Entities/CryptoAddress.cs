using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models.Entities
{
    public class CryptoAddress
    {
        [Key]
        public int Id { get; set; }
        public string IdentityUserId { get; set; }
        public string PublicKey { get; set; }
        public string Coin { get; set; }
        public string AccessLevel { get; set; }
        public string Title { get; set; }

        [NotMapped]
        public string Balance
        {
            get
            {
                const string UNAVAILABLE = "Unavailable";
                switch (Coin)
                {
                    case "Monero":
                        return UNAVAILABLE;

                    case "Bitcoin":
                        const string URL = "https://blockchain.info/q/addressbalance/";
                        HttpClient client = new HttpClient();
                        Uri uri = new Uri(URL + PublicKey);

                        HttpResponseMessage res = client.GetAsync(uri).GetAwaiter().GetResult();
                        string body = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        bool isValid = int.TryParse(body, out int satoshiBalance);

                        if (!isValid)
                        {
                            return UNAVAILABLE;
                        }

                        double btcBalance = (double) satoshiBalance / CryptoAddressHelper.HelperBitcoinProperties.DIVISOR;

                        return String.Format("{0} BTC", btcBalance);
                }

                return "N/A";
            }
        }
        
        [NotMapped]
        public string Format
        {
            get
            {
                return GetFormat(PublicKey, Coin);
            }
        }

        public static string GetFormat(string pubKey, string coinType)
        {
            switch(coinType)
            {
                case "Bitcoin":
                    return CryptoAddressHelper.CheckBitcoinAddressFormat(pubKey);

                case "Monero":
                    return CryptoAddressHelper.CheckMoneroAddressFormat(pubKey);
            }

            return "Unknown Format";
        }
    }

    public static class CryptoAddressHelper
    {
        public static string UNKNOWN = "Unknown";
        public static class HelperBitcoinProperties
        {
            public const int MIN_ADDR_LENGTH = 26;
            public const int MAX_ADDR_LENGTH = 35;
            public const int MIN_BC1_ADDR_LENGTH = 32;
            public const int MAX_BC1_ADDR_LENGTH = 90;
            public const string LEGACY_START = "1";
            public const string SEGWIT_START = "3";
            public const string SEGWIT_NATIVE_START = "bc1q";
            public const string TAPROOT_START = "bc1p";
            public const int DIVISOR = 100000000;
        }

        public static class HelperMoneroProperties
        {
            public const string ADDRESS_START = "4";
            public const string ADDRESS_START2 = "8";
            public const int ADDRESS_LENGTH = 95;
            public const int INTEGRATED_ADDRESS_LENGTH = 106;
        }

        public static string CheckBitcoinAddressFormat(string key)
        {
            if (key.Contains('.') || key.Contains(" "))
                return UNKNOWN;

            if (key.StartsWith(HelperBitcoinProperties.LEGACY_START)
                && key.Length >= HelperBitcoinProperties.MIN_ADDR_LENGTH
                && key.Length <= HelperBitcoinProperties.MAX_ADDR_LENGTH)
            {
                return "Legacy";
            }

            if (key.StartsWith(HelperBitcoinProperties.SEGWIT_START)
                && key.Length >= HelperBitcoinProperties.MIN_ADDR_LENGTH
                && key.Length <= HelperBitcoinProperties.MAX_ADDR_LENGTH)
            {
                return "Nested SegWit";
            }

            if (key.StartsWith(HelperBitcoinProperties.SEGWIT_NATIVE_START)
                && key.Length >= HelperBitcoinProperties.MIN_BC1_ADDR_LENGTH
                && key.Length <= HelperBitcoinProperties.MAX_BC1_ADDR_LENGTH)
            {
                IEnumerable<Char> uppercaseChars = key.ToCharArray().Where(c => Char.IsUpper(c));
                if (uppercaseChars.Count() > 0)
                    return UNKNOWN;

                return "Native SegWit (BECH32)";
            }

            if (key.StartsWith(HelperBitcoinProperties.TAPROOT_START)
                && key.Length >= HelperBitcoinProperties.MIN_BC1_ADDR_LENGTH
                && key.Length <= HelperBitcoinProperties.MAX_BC1_ADDR_LENGTH)
            {
                IEnumerable<Char> uppercaseChars = key.ToCharArray().Where(c => Char.IsUpper(c));
                if (uppercaseChars.Count() > 0)
                    return UNKNOWN;

                return "Taproot";
            }

            return UNKNOWN;
        }

        public static string CheckMoneroAddressFormat(string key)
        {
            if (key.Contains('.') || key.Contains(" "))
                return UNKNOWN;

            if ((key.StartsWith(HelperMoneroProperties.ADDRESS_START)
                || key.StartsWith(HelperMoneroProperties.ADDRESS_START2)))
            {
                if (key.Length == HelperMoneroProperties.ADDRESS_LENGTH)
                    return "Standard Sub-Address";
                else if (key.Length == HelperMoneroProperties.INTEGRATED_ADDRESS_LENGTH)
                    return "Integrated Address";
                else
                    return UNKNOWN;
            }

            return UNKNOWN;
        }
    }
}
