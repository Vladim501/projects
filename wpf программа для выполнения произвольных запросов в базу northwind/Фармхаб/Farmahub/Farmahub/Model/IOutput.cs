using System.Data;

namespace Farmahub.Model
{
    // Интерфейс для вывода данных, любой функционал для вывода данных наружу должен реализовывать этот интерфейс
    public interface IOutput
    {
        void Write(DataTable content);
    }
}