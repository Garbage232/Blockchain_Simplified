using BlockChain_Simplified.Models;
using Microsoft.Data.SqlClient;
using System;
using System.IO;

namespace BlockChain_Simplified
{
    class Program
    {
        static void Main(string[] args)
        {
            int input = 0;
            int x = 0;
            while (x != 7)
            {
                Console.WriteLine("Type number");
                Console.WriteLine("1 === Retrieve Account Document table");
                Console.WriteLine("2 === Update Customer with Pdf document");
                Console.WriteLine("3 === Create pdf file from db stored under document column");
                Console.WriteLine("4 === Retrieve last row from Ledger History");

                x = int.Parse(Console.ReadLine());

                switch (x)
                {

                    case 1:
                        var instance2 = new Program();
                        string sql2 = "SELECT *  FROM [Account].[Document]";
                        var value2 = instance2.Ledger(sql2);
                        Console.WriteLine($"Customer ID     First Name  Last Name  Document");
                        if (value2.Document.Length > 20)
                            Console.WriteLine($"{value2.CustomerID}    {value2.FirstName}    {value2.LastName}    {value2.Document.Substring(0, 20)}");
                        else
                            Console.WriteLine($"{value2.CustomerID}    {value2.FirstName}    {value2.LastName}    {value2.Document}");
                        break;
                    case 2:
                        var instance3 = new Program();
                        var value3 = instance3.UpdateDbPdfasBase64();
                        break;
                    case 3:
                        var instance4 = new Program();
                        instance4.RetrievePdf();
                        break;
                    case 4:
                        var instance = new Program();
                        string sql = "SELECT * FROM [Account].[MSSQL_LedgerHistoryFor_1845581613]";
                        var value = instance.Ledger(sql);
                        if (value.Document.Length > 20)
                            Console.WriteLine($"{value.CustomerID}    {value.FirstName}    {value.LastName}    {value.Document.Substring(0, 20)}");
                        else
                            Console.WriteLine($"{value.CustomerID}    {value.FirstName}    {value.LastName}    {value.Document}");
                        break;
                    case 5:
                        var instance1 = new Program();
                        string sql1 = "SELECT * FROM Account.Document_Ledger ORDER BY ledger_transaction_id";
                        var value1 = instance1.Ledger(sql1);
                        Console.WriteLine($"{value1.CustomerID}====={value1.Document}");
                        break;
                    case 7:
                        input = 30;
                        break;
                    default:
                        Console.WriteLine("Unknown value");
                        break;
                }
                input++;
                if (input < 30)
                    continue;
                else
                    break;

                
            }
        }
         public DocumentTable Ledger(string sql)
        
        {
           DocumentTable document = new DocumentTable();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "<azuredatasource>";
                builder.UserID = "<azuredbuser>";
                builder.Password = "<azuredbpassword>";
                builder.InitialCatalog = "<azuredbname>";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                document = new DocumentTable
                                {
                                   CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    Document = reader["Document"].ToString(),
                                 FirstName = reader["FirstName"].ToString(),
                                 LastName = reader["LastName"].ToString()
                                };
               
                        }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return document;
        }
        public string ConvertPdftoBase(string pdfPath)
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            return pdfBase64;

        }

        public string UpdateDbPdfasBase64()
        {
            var instance = new Program();
            Console.WriteLine("Input file location and file name");
            var pdf = Console.ReadLine();
            var value = instance.ConvertPdftoBase(pdf);
            Console.WriteLine($"Base64==={value}");
            string sql = $"UPDATE [Account].[Document] SET [Document] = '{value}' WHERE[CustomerID] = 1";
            
            
          var value1 = instance.Ledger(sql);
            return value1.Document;

        }
        public void RetrievePdf()
        {
            var instance = new Program();

            string sql = "SELECT * FROM [Account].[Document]";
            var document = instance.Ledger(sql);
            byte[] PDFDecoded = Convert.FromBase64String(document.Document);
            string FileName = @"C:\Temp\ConvertedFromBase64-" + DateTime.Now.ToString("dd-hh-mm") + ".pdf";
            File.WriteAllBytes(FileName, PDFDecoded);
            Console.WriteLine($"{FileName} is created");


        }


    }

}
