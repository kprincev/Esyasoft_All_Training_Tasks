using InsertDataInMSSql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InsertDataInMSSql
{ 
    public class Mathods
    {
        public void GetInputAndStoreMssql()
        {

            bool exit = true;
            while (exit)
            {
                string Email="";
               
                bool isValidAge = false;
                Console.Write("Inserting Data");
                Console.WriteLine("---------------");
                Console.Write("\nEnter employee Name:");
                string name = Console.ReadLine();

                
             
                bool isValidEmail = false;
                while (!isValidEmail)
                {
                    Console.Write("\nEnter Email:");
                    Email = Console.ReadLine();
                    if (Email.Contains("@") && Email.Contains("."))
                    {
                        isValidEmail = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Email Format. Please enter a valid email.");


                    }
                }
            bool  isValid = false;
            string phone = "";
            while (!isValid)
            { 
                Console.Write("\nEnter Phone Number :");
                 phone = Console.ReadLine();
                if (phone.Length == 10 && phone.All(char.IsDigit))
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Invalid Phone Number. Please enter a valid 10-digit phone number.");
                }
            }
            Console.Write("\nEnter Salary :");
            int salary = int.Parse(Console.ReadLine());


            Services.InsertDataInMsql(name, Email, phone,salary);
                Console.WriteLine("Do you want to insert more data? (yes/no): ");
                string choice = Console.ReadLine().ToLower();
                if(choice !="yes")
                {
                    exit = false;

                }
            }

        }
    
}


    internal class Program
    {
        static void Main(string[] args)
        {
            Mathods ob=new Mathods();
            if(ConfigReader.flag=="input")
            {
                ob.GetInputAndStoreMssql();
            }
            else if (ConfigReader.flag=="export")
            {
                Services.ExportStoredProcToCsv();
            }

        }
    }
}
