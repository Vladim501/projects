using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Farmahub.Annotations;
using Farmahub.Model;

namespace Farmahub.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {

        // моя строка подключения использую MSSQL Server 2012 база northwind
        // string connectionString = @"Data Source=VLADIM\SQLEXPRESS;Initial Catalog=northwind;Integrated Security=True";
        // подключаем контроллер
        Controller cr = new Controller();


        #region Ссвойства для View
        
        // свойство для текстового запроса
        private string _request;
        public string Request
        {

            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged();
            }

        }

        // ссвойство для отображения данных в датагрид
        private DataTable _data;
        public DataTable Data
        {

            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged();
            }

        }

        // свойство для количества строк вывода
        private int? _stringout;
        public int? StringOut
        {

            get { return _stringout; }
            set
            {
                _stringout = value;
                OnPropertyChanged();
            }


        }

        // свойство для строки подключения
        string _connectionString;
        public string Conn
        {

            get { return _connectionString; }
            set
            {
                _connectionString = value;
                OnPropertyChanged();
            }

        }

        #endregion


        #region Команды для кнопок во View
        
        // команда выполнения запроса
        public Command CommandToRun
        {
            get { return new Command(async () =>
            {

                if (_request==null && _connectionString==string.Empty)
                {
                    MessageBox.Show("Запрос пустой или пустая строка подключения.");
                }
                else
                {


                    Data = await cr.GetSomeData(_connectionString, Request, _stringout);
                }

                

            }); }
        }
        
        // команда для сохранения данных в csv
        public Command SaveDataToCSV
        {
            get
            {
                return new Command( () =>
                {

                    if (_data == null)
                    {
                        MessageBox.Show("Данных для сохранения нет");
                    }
                    else
                    {
                       cr.WriteDatatoCsv(_data);
                    }



                });
            }
        }

        // команда для сохранения в word
        public Command SaveDataToWord

        {
            get
            {
                return new Command(() =>
                {

                    if (_data == null)
                    {
                        MessageBox.Show("Данных для сохранения нет");
                    }
                    else
                    {
                        cr.WriteDataToWord(_data);
                    }



                });
            }
        }

        #endregion




        // конструктор
        public MainViewModel()
        {
            // загружаем контейнер
            cr.LoadContainer();

        }

        // реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
