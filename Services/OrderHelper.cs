using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services
{
    public class OrderHelper
    {
        public static decimal ShippingFee { get; } = 5;

        public static Dictionary<string, string> PaymentMethods { get; } = new()
        {
            { "Cash", "Cash on Delivery" },
            { "Card", "Credit Card" }
        };

        public static List<string> PaymentStatuses { get; } = new()
        {
            "Pending", "Accepted","Refunded"
        };

        public static List<string> OrderStatuses { get; } = new()
        {
            "Created", "Shipped", "Returned", "Delivered", "Cancelled"
        };

        /*
         * Receives a string of product identifiers, separated by '-'
         * Example: 9-9-7-9-6
         * 
         * Returns a list of pairs (dictionary):
         *     - the pair name is the product ID
         *     - the pair value is the product quantity
         * Example:
         * {
         *     9: 3,
         *     7: 1,
         *     6: 1
         * }
         */
        public static Dictionary<int, int> GetProductDictionary(string productIdentifiers)
        {
            var productDictionary = new Dictionary<int, int>();

            if (productIdentifiers.Length > 0)
            {
                string[] productIdArray = productIdentifiers.Split('-');
                foreach (var spaproductId in productIdArray)
                {
                    try
                    {
                        int id = int.Parse(spaproductId);

                        if (productDictionary.ContainsKey(id))
                        {
                            productDictionary[id] += 1;
                        }
                        else
                        {
                            productDictionary.Add(id, 1);
                        }
                    }
                    catch (Exception) { }
                }
            }
            return productDictionary;
        }
    }
}