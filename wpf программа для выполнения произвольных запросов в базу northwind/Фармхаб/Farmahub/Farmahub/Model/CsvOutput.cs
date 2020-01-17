using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Farmahub.Model
{

    // класс для вывода данных в csv файл

    // реализуем интерфейс IOutput
    public class CsvOutput:IOutput
    {
        public void Write(DataTable content)
        {
            try
            {
             // пишем данные в csv
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = content.Columns.Cast<DataColumn>().
                Select(column => column.ColumnName);
            sb.AppendLine(string.Join(";", columnNames));
            foreach (DataRow row in content.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(";", fields));
            }

            File.WriteAllText("csvdata.csv", sb.ToString());

                // Открываем папку с файлом
                
                Process PrFolder = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                string file = @"csvdata.csv";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Normal;
                psi.FileName = "explorer";
                psi.Arguments = @"/n, /select, " + file;
                PrFolder.StartInfo = psi;
                PrFolder.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show(" Ошибка записи "+e);
                throw;
            }
           

        }
    }
}