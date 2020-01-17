using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;

namespace Farmahub.Model
{
    class Controller
    {
        #region loc Контейнер
        
        // подключаем контейнер
        private static IContainer Container { get; set; }
        public void LoadContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CsvOutput>().Named<IOutput>("CsvOutput");
            builder.RegisterType<DocOutput>().Named<IOutput>("DocOutput");
            Container = builder.Build();

        }

        #endregion

        

        #region Методы для вывода данных

        // метод для записи данных в csv
        // для расширения функциональности используем контейнер
        // чтобы мне добавить новую функцию по выводу данных, мне нужно создать клас, зарегестрировоать его в контейнере, и вывести кнопку c командой в gui
        public  void WriteDatatoCsv(DataTable table)
        {
            
            using (var scope = Container.BeginLifetimeScope())
            {
                var writer = scope.ResolveNamed<IOutput>("CsvOutput");
                writer.Write(table);
            }
        }

        // пишем даныне в word
        public void WriteDataToWord(DataTable table)
        {

            using (var scope = Container.BeginLifetimeScope())
            {
                var writer2 = scope.ResolveNamed<IOutput>("DocOutput");
                writer2.Write(table);
            }


        }

        #endregion



        #region Асинхронные методы
        
        // асинхрон
        public static Task<DataTable> AsyncGetDataFromBase(string _connectionString, string qwery, int? _stringout)
        {
            string connectionString = _connectionString;

            return Task.Run(() =>
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка соединения " + e);
                    throw;
                }

                var dt1 = new DataTable();

                //получаем результат запроса в DataTable...
                try
                {
 
                 using (var adapter = new System.Data.SqlClient.SqlDataAdapter(qwery, connectionString))
                    {
                        adapter.Fill(dt1);
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(" Что-то не так с запросом  " + e);
                    throw;
                }


                DataTable rezulTable1 = null;
                // выбираем количество записей

                if (_stringout != null && _stringout != 0) // проверяем, что строка не пустая, и не равна 0
                {
                    int countRows = (int) _stringout;
                    var t = dt1.AsEnumerable().Take(countRows);
                    // вот тут ошибка
                    rezulTable1 = t.CopyToDataTable();

                }
                else
                {
                    rezulTable1 = dt1;
                }

                return rezulTable1;
                
            });

        }

        public async Task<DataTable> GetSomeData(string _connectionString, string qwery, int? _stringout)
        {
           
            DataTable results = await AsyncGetDataFromBase(_connectionString, qwery, _stringout);
            return results;
        }

        #endregion



    }
}

