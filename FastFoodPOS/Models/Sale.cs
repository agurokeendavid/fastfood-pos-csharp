﻿using FastFoodPOS.ErrorHandler;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodPOS.Models
{
    class Sale
    {
        public decimal Value { get; set; }
        public int OrderCount { get; set; }
        public int CustomerCount { get; set; }
        public DateTime Date { get; set; }

        public static List<Sale> GetSalesBetween(DateTime from, DateTime to)
        {
            List<Sale> result = new List<Sale>();
            using (var cmd = new OleDbCommand("SELECT * FROM SalesView WHERE day BETWEEN #@@from# AND #@@to#", Database.GetConnection()))
            {
                //BindParams not working
                //idk why
                cmd.CommandText = cmd.CommandText.Replace("@@from", from.AddDays(-1).ToShortDateString());
                cmd.CommandText = cmd.CommandText.Replace("@@to", to.AddDays(1).ToShortDateString());
                Database.GetConnection().Open();
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Sale
                        {
                            Value = reader.GetDecimal(0),
                            Date = reader.GetDateTime(1),
                            OrderCount = reader.GetInt16(2),
                            CustomerCount = reader.GetInt32(3),
                        });
                    }
                }
                Database.GetConnection().Close();
            }
            return result;
        }

        public static Sale GetSale(DateTime day)
        {
            Sale result = new Sale() { Value = 0, Date = day, OrderCount = 0, CustomerCount = 0 };
            using (var cmd = new OleDbCommand("SELECT * FROM SalesView WHERE day = ?", Database.GetConnection()))
            {
                Database.BindParameters(cmd, day.ToShortDateString());
                Database.GetConnection().Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Value = reader.GetDecimal(0);
                        result.OrderCount = reader.GetInt16(2);
                        result.CustomerCount = reader.GetInt32(3);
                    }
                }
                Database.GetConnection().Close();
            }
            return result;
        }

        public static List<Sale> GetSalesFromLastWeek()
        {
            return GetSalesBetween(DateTime.Now.AddDays(-6), DateTime.Now);
        }

        public static string[] GetDaysLabelFromLastWeek()
        {
            return GetDaysLabel(DateTime.Now.AddDays(-6), DateTime.Now);
        }

        public static string[] GetDaysLabel(DateTime from, DateTime to)
        {
            if (from > to) throw new Level1Exception("Start date must not be Greater than end date");
            List<string> result = new List<string>();
            to = to.AddDays(1);
            while(!from.ToShortDateString().Equals(to.ToShortDateString()))
            {
                result.Add(from.ToString("MMM dd"));
                from = from.AddDays(1);
            }
            return result.ToArray<string>();
        }
    }
}
