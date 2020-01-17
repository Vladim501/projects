using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using DataTable = System.Data.DataTable;

namespace Farmahub.Model
{
    // пример расширения функционала, добавляем сохранение данных в Word
    public  class DocOutput:IOutput
    {
        public void Write(DataTable content)
        {
            try
            {

                int RowCount = content.Rows.Count; int ColumnCount = content.Columns.Count;
                Object[,] DataArray = new object[RowCount + 1, ColumnCount + 1];
                //int RowCount = 0; int ColumnCount = 0;
                int r = 0;
                for (int c = 0; c <= ColumnCount - 1; c++)
                {
                    DataArray[r, c] = content.Columns[c].ColumnName;
                    for (r = 0; r <= RowCount - 1; r++)
                    {
                        DataArray[r, c] = content.Rows[r][c];
                    } //end row loop
                } //end column loop

                Microsoft.Office.Interop.Word.Document oDoc = new Microsoft.Office.Interop.Word.Document();
                oDoc.Application.Visible = true;
                oDoc.PageSetup.Orientation = Microsoft.Office.Interop.Word.WdOrientation.wdOrientLandscape;

                dynamic oRange = oDoc.Content.Application.Selection.Range;
                String oTemp = "";
                for (r = 0; r <= RowCount - 1; r++)
                {
                    for (int c = 0; c <= ColumnCount - 1; c++)
                    {
                        oTemp = oTemp + DataArray[r, c] + "\t";

                    }
                }

                oRange.Text = oTemp;

                object Separator = Microsoft.Office.Interop.Word.WdTableFieldSeparator.wdSeparateByTabs;
                object Format = Microsoft.Office.Interop.Word.WdTableFormat.wdTableFormatWeb1;
                object ApplyBorders = true;
                object AutoFit = true;

                object AutoFitBehavior = Microsoft.Office.Interop.Word.WdAutoFitBehavior.wdAutoFitContent;
                oRange.ConvertToTable(ref Separator,
            ref RowCount, ref ColumnCount, Type.Missing, ref Format,
            ref ApplyBorders, Type.Missing, Type.Missing, Type.Missing,
             Type.Missing, Type.Missing, Type.Missing,
             Type.Missing, ref AutoFit, ref AutoFitBehavior,
             Type.Missing);

                oRange.Select();
                oDoc.Application.Selection.Tables[1].Select();
                oDoc.Application.Selection.Tables[1].Rows.AllowBreakAcrossPages = 0;
                oDoc.Application.Selection.Tables[1].Rows.Alignment = 0;
                oDoc.Application.Selection.Tables[1].Rows[1].Select();
                oDoc.Application.Selection.InsertRowsAbove(1);
                oDoc.Application.Selection.Tables[1].Rows[1].Select();

                //gotta do the header row manually
                for (int c = 0; c <= ColumnCount - 1; c++)
                {
                    oDoc.Application.Selection.Tables[1].Cell(1, c + 1).Range.Text = content.Columns[c].ColumnName;
                }

                oDoc.Application.Selection.Tables[1].Rows[1].Select();
                oDoc.Application.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;







            }
            catch (Exception e)
            {
                MessageBox.Show("Чтото пошло не так"+e);
                throw;
            }



        }
    }
}
