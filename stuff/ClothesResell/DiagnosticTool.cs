using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

class DiagnosticTool
{
    static void Main()
    {
        StringBuilder sb = new StringBuilder();
        try
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
            dbPath = Path.GetFullPath(dbPath);
            string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                
                // Get all tables
                DataTable tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                sb.AppendLine("=== ALL TABLES IN DATABASE ===");
                foreach (DataRow row in tables.Rows)
                {
                    sb.AppendLine($"  {row["TABLE_NAME"]}");
                }
                
                // Focus on tblbasket and tblBuyItems
                sb.AppendLine("\n=== DETAILED COLUMN INFO ===");
                
                string[] tablesToCheck = { "tblbasket", "tblBuyItems", "tblPastTransactions" };
                foreach (string tableName in tablesToCheck)
                {
                    try
                    {
                        DataTable columns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
                        sb.AppendLine($"\n{tableName}:");
                        sb.AppendLine("  Columns:");
                        foreach (DataRow col in columns.Rows)
                        {
                            string colName = col["COLUMN_NAME"].ToString();
                            string dataType = col["DATA_TYPE"].ToString();
                            sb.AppendLine($"    - {colName} (Type: {dataType})");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"\n{tableName}: ERROR - {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"ERROR: {ex.Message}");
            sb.AppendLine($"{ex.StackTrace}");
        }
        
        // Write to file in the bin/Debug directory
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diagnostic_output.txt");
        File.WriteAllText(outputPath, sb.ToString());
        Console.WriteLine($"Diagnostic output written to {outputPath}");
    }
}
