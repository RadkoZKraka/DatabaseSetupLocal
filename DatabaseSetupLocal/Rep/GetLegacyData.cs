using System.Runtime.InteropServices;
using DatabaseSetupLocal.Models;
using OfficeOpenXml;

namespace DatabaseSetupLocal.Rep;

public static class GetLegacyData
{
    
    public static Dictionary<string, List<Dictionary<string, List<string>>>> GetData()
    {
        var excelPath = @"Formula1-strzały.xlsx";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            excelPath = @"Formula1-strzały.xlsx";
        }
        var shots = new Dictionary<string, List<Dictionary<string, List<string>>>>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage(new FileInfo(excelPath)))
        {
            var worksheetLoc = 2;
            for (int i = 0; i < 5; i++)
            {
                var currentSheet = package.Workbook.Worksheets[worksheetLoc];

                var row = 3;
                for (int j = 0; j < 5; j++)
                {
                    var column = 2;
                    for (int k = 0; k < 5; k++)
                    {
                        if (j == 4 && k == 2)
                        {
                            break;
                        }

                        var list = new List<Dictionary<string, List<string>>>();
                        var races = new Dictionary<string, List<string>>();
                        var currentRace = currentSheet.Cells[row - 2, column - 1].Text;

                        races.Add(currentRace,
                            new List<string>(currentSheet.Cells[row, column, row + 28, column]
                                .Select(x => (string) x.Value)
                                .ToList()));

                        list.Add(races);

                        if (!shots.ContainsKey(currentSheet.Name))
                        {
                            shots.Add(currentSheet.Name, new List<Dictionary<string, List<string>>>(list));
                        }

                        shots[currentSheet.Name].Add(races);

                        column += 3;
                    }

                    row += 29;
                }

                worksheetLoc++;
            }

            var r = 3;
        }

        return shots;
    }
    // public static List<ShotsModel> GetDataAsModel()
    // {
    //     var shots = new List<ShotsModel>();
    //     ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    //
    //     using (var package = new ExcelPackage(new FileInfo(@"C:\F1ShotsTest\Formula1-strzały.xlsx")))
    //     {
    //         var worksheetLoc = 2;
    //         for (int i = 0; i < 5; i++)
    //         {
    //             var currentSheet = package.Workbook.Worksheets[worksheetLoc];
    //
    //             var row = 3;
    //             for (int j = 0; j < 5; j++)
    //             {
    //                 var column = 2;
    //                 for (int k = 0; k < 5; k++)
    //                 {
    //                     if (j == 4 && k == 2)
    //                     {
    //                         break;
    //                     }
    //
    //                     var list = new List<Dictionary<string, List<string>>>();
    //                     var races = new List<Shots>();
    //                     var currentRace = currentSheet.Cells[row - 2, column - 1].Text;
    //
    //                     races.Add(currentRace,
    //                         new List<string>(currentSheet.Cells[row, column, row + 27, column]
    //                             .Select(x => (string) x.Value)
    //                             .ToList()));
    //
    //                     list.Add(races);
    //
    //                     if (!shots.ContainsKey(currentSheet.Name))
    //                     {
    //                         shots.Add(currentSheet.Name, new List<Dictionary<string, List<string>>>(list));
    //                     }
    //
    //                     shots[currentSheet.Name].Add(races);
    //
    //                     column += 3;
    //                 }
    //
    //                 row += 29;
    //             }
    //
    //             worksheetLoc++;
    //         }
    //
    //         var r = 3;
    //     }
    //
    //     return shots;
    // }
}