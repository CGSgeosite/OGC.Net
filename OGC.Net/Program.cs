/******************************************************************************
 *
 * Name: OGC.net
 * Purpose: A free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV, converting
 *          into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector
 *          or raster to PostgreSQL database.
 *
 ******************************************************************************
 * (C) 2019-2023 Geosite Development Team of CGS (R)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Geosite.Messager;
using System.Text;

/* csproj 文件增加
 <ItemGroup>
    <None Include="gdal\**" CopyToOutputDirectory="Always" />
</ItemGroup>
 */

namespace Geosite
{
    static class Program
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Registration language family code
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var argCount = args.Length;

            if (argCount == 0)
            {
                //Attach to Winform
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                //Attach to Console
                AttachConsole(-1);
                
                var applicationName = Assembly.GetExecutingAssembly().GetName().Name;
                (int Left, int Top)? cursorPosition = null;

                var title = $@"{applicationName} for {RuntimeInformation.OSDescription} / {RuntimeInformation.ProcessArchitecture} "; 
                var copyright = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))!).Copyright;
                var splitLine = new string('*', Math.Max(title.Length, copyright.Length));
                
                Console.WriteLine();
                Console.WriteLine(splitLine);
                Console.WriteLine(title);
                Console.WriteLine(copyright);
                Console.WriteLine(splitLine);

                try
                {
                    //args ---> Dictionary<string, List<string>>
                    var options = ConsoleIO.Arguments(
                        args: args,
                        offset: 0 //1=Skip command parameter 
                    );

                    var helper = options != null && (options.ContainsKey("?") || options.ContainsKey("h") || options.ContainsKey("help"));

                    var commandName = args[0].ToLower();
                    switch (commandName)
                    {
                        case "mpj":
                        case "mapgismpj":
                            {
                                if (helper)
                                {
                                    CommandHelper(commandName);
                                    break;
                                }

                                if (options != null)
                                {
                                    if (!options.TryGetValue("i", out var inputFile))
                                        options.TryGetValue("input", out inputFile);
                                    if (inputFile == null)
                                        throw new Exception("Input file not found.");

                                    if (!options.TryGetValue("o", out var outputFile))
                                        options.TryGetValue("output", out outputFile);
                                    if (outputFile == null)
                                        throw new Exception("Output file not found.");

                                    var isDirectory = Path.GetExtension(outputFile[0]) == string.Empty;
                                    var inputFileCount = inputFile.Count;

                                    if (inputFileCount == 1 && !isDirectory || inputFileCount > 1 && isDirectory) {
                                        for (var i = 0; i < inputFileCount; i++) {
                                            var sourceFile = inputFile[i];
                                            var mapgisMpj = new MapGis.MapGisProject();
                                            var localI = i + 1;
                                            mapgisMpj.OnMessagerEvent += (_, e) =>
                                            {
                                                var userStatus = !string.IsNullOrWhiteSpace(e.Message)
                                                    ? inputFileCount > 1
                                                        ? $"[{localI}/{inputFileCount}] {e.Message}"
                                                        : e.Message
                                                    : null;
                                                ShowProgress(message: userStatus, code: e.Code, progress: e.Progress);
                                            };
                                            string targetFile;
                                            if (!isDirectory)
                                                targetFile = Path.ChangeExtension(outputFile[0], ".json");
                                            else {
                                                var postfix = 0;
                                                do {
                                                    targetFile = Path.Combine(
                                                        outputFile[0],
                                                        Path.GetFileNameWithoutExtension(sourceFile) + (postfix == 0 ? "" : $"({postfix})") + ".json");
                                                    if (!File.Exists(targetFile))
                                                        break;
                                                    postfix++;
                                                } while (true);
                                            }
                                            mapgisMpj.Open(file: sourceFile);
                                            var directoryName = Path.GetDirectoryName(targetFile);
                                            if (Directory.Exists(directoryName) == false)
                                                Directory.CreateDirectory(directoryName!);
                                            mapgisMpj.Export(saveAs: targetFile);
                                        }
                                    }
                                    else
                                        throw new Exception("Input and output parameters do not match.");
                                }
                            }
                            break;
                        case "mapgis":
                            {
                                if (helper)
                                {
                                    CommandHelper(commandName);
                                    break;
                                }
                                if (options != null) {
                                    if (!options.TryGetValue("i", out var inputFile))
                                        options.TryGetValue("input", out inputFile);
                                    if (inputFile == null)
                                        throw new Exception("Input file not found.");

                                    if (!options.TryGetValue("o", out var outputFile))
                                        options.TryGetValue("output", out outputFile);
                                    if (outputFile == null)
                                        throw new Exception("Output file not found.");

                                    if (!options.TryGetValue("f", out var format))
                                        options.TryGetValue("format", out format);

                                    if (!options.TryGetValue("t", out var treePath))
                                        options.TryGetValue("treepath", out treePath);

                                    if (!options.TryGetValue("d", out var description))
                                        options.TryGetValue("description", out description);

                                    options.TryGetValue("pcolor", out var pcolor);

                                    var isDirectory = Path.GetExtension(outputFile[0]) == string.Empty;
                                    var inputFileCount = inputFile.Count;

                                    if (inputFileCount == 1 && !isDirectory || inputFileCount > 1 && isDirectory) {
                                        for (var i = 0; i < inputFileCount; i++) {
                                            var sourceFile = inputFile[i];
                                            var localI = i + 1;
                                            var mapgis = new MapGis.MapGisFile();
                                            mapgis.OnMessagerEvent += (_, e) =>
                                            {
                                                var userStatus = !string.IsNullOrWhiteSpace(e.Message)
                                                    ? inputFileCount > 1
                                                        ? $"[{localI}/{inputFileCount}] {e.Message}"
                                                        : e.Message
                                                    : null;
                                                ShowProgress(message: userStatus, code: e.Code, progress: e.Progress);
                                            };
                                            mapgis.Open(mapgisFile: sourceFile, pcolorFile: pcolor?[0]);
                                            string targetFile;
                                            if (!isDirectory)
                                                targetFile = outputFile[0];
                                            else {
                                                var postfix = 0;
                                                do {
                                                    targetFile = Path.Combine(
                                                        outputFile[0],
                                                        Path.GetFileNameWithoutExtension(sourceFile) + (postfix == 0 ? "" : $"({postfix})") + "." + (format != null ? format[0] : "geojson"));
                                                    if (!File.Exists(targetFile))
                                                        break;
                                                    postfix++;
                                                } while (true);
                                            }
                                            var directoryName = Path.GetDirectoryName(targetFile);
                                            if (Directory.Exists(directoryName) == false)
                                                Directory.CreateDirectory(directoryName!);
                                            mapgis.Export(
                                                saveAs: targetFile,
                                                format: format != null ? format[0] : "geojson",
                                                treePath: treePath?[0],
                                                extraDescription: description == null
                                                    ? null
                                                    : XElement.Parse(description[0])
                                            );
                                        }
                                    }
                                    else
                                        throw new Exception("Input and output parameters do not match.");
                                }
                            }
                            break;
                        case "shp":
                        case "shapefile":
                            {
                                if (helper)
                                {
                                    CommandHelper(commandName);
                                    break;
                                }
                                if (options != null) {
                                    if (!options.TryGetValue("i", out var inputFile))
                                        options.TryGetValue("input", out inputFile);
                                    if (inputFile == null)
                                        throw new Exception("Input file not found.");

                                    if (!options.TryGetValue("o", out var outputFile))
                                        options.TryGetValue("output", out outputFile);
                                    if (outputFile == null)
                                        throw new Exception("Output file not found.");

                                    if (!options.TryGetValue("f", out var format))
                                        options.TryGetValue("format", out format);

                                    if (!options.TryGetValue("c", out var codePage))
                                        options.TryGetValue("codepage", out codePage);

                                    if (!options.TryGetValue("t", out var treePath))
                                        options.TryGetValue("treepath", out treePath);

                                    if (!options.TryGetValue("d", out var description))
                                        options.TryGetValue("description", out description);

                                    var isDirectory = Path.GetExtension(outputFile[0]) == string.Empty;
                                    var inputFileCount = inputFile.Count;
                                    if (inputFileCount == 1 && !isDirectory || inputFileCount > 1 && isDirectory) {
                                        for (var i = 0; i < inputFileCount; i++) {
                                            var sourceFile = inputFile[i];
                                            var localI = i + 1;
                                            var shapeFile = new ShapeFileHelper.ShapeFileReader();
                                            shapeFile.OnMessagerEvent += (_, e) =>
                                            {
                                                var userStatus = !string.IsNullOrWhiteSpace(e.Message)
                                                    ? inputFileCount > 1
                                                        ? $"[{localI}/{inputFileCount}] {e.Message}"
                                                        : e.Message
                                                    : null;
                                                ShowProgress(message: userStatus, code: e.Code, progress: e.Progress);
                                            };
                                            shapeFile.Open(
                                                sourceFile,
                                                int.Parse(codePage == null ? $"{Encoding.Default.CodePage}" : codePage[0])
                                            );
                                            string targetFile;
                                            if (!isDirectory)
                                                targetFile = outputFile[0];
                                            else {
                                                var postfix = 0;
                                                do {
                                                    targetFile = Path.Combine(
                                                        outputFile[0],
                                                        Path.GetFileNameWithoutExtension(sourceFile) + (postfix == 0 ? "" : $"({postfix})") + "." + (format != null ? format[0] : "geojson"));
                                                    if (!File.Exists(targetFile))
                                                        break;
                                                    postfix++;
                                                } while (true);
                                            }
                                            var directoryName = Path.GetDirectoryName(targetFile);
                                            if (Directory.Exists(directoryName) == false)
                                                Directory.CreateDirectory(directoryName!);
                                            shapeFile.Export(
                                                saveAs: targetFile,
                                                format: format == null ? "geojson" : format[0],
                                                treePath: treePath?[0],
                                                extraDescription: description == null
                                                    ? null
                                                    : XElement.Parse(description[0])
                                            );
                                        }
                                    }
                                    else
                                        throw new Exception("Input and output parameters do not match.");
                                }
                            }
                            break;
                        case "txt":
                        case "csv":
                        case "excel":
                            {
                                if (helper)
                                {
                                    CommandHelper(commandName);
                                    break;
                                }
                                if (options != null) {
                                    if (!options.TryGetValue("i", out var inputFile))
                                        options.TryGetValue("input", out inputFile);
                                    if (inputFile == null)
                                        throw new Exception("Input file not found.");

                                    if (!options.TryGetValue("o", out var outputFile))
                                        options.TryGetValue("output", out outputFile);
                                    if (outputFile == null)
                                        throw new Exception("Output file not found.");

                                    if (!options.TryGetValue("f", out var format))
                                        options.TryGetValue("format", out format);

                                    if (!options.TryGetValue("t", out var treePath))
                                        options.TryGetValue("treepath", out treePath);

                                    if (!options.TryGetValue("d", out var description))
                                        options.TryGetValue("description", out description);

                                    if (!options.TryGetValue("c", out var coordinateFieldName))
                                        options.TryGetValue("coordinate", out coordinateFieldName);

                                    var isDirectory = Path.GetExtension(outputFile[0]) == string.Empty;
                                    var inputFileCount = inputFile.Count;
                                    if (inputFileCount == 1 && !isDirectory || inputFileCount > 1 && isDirectory) {
                                        for (var i = 0; i < inputFileCount; i++) {
                                            var sourceFile = inputFile[i];
                                            var localI = i + 1;
                                            var freeTextFields = commandName switch {
                                                ".txt" => FreeText.TXT.TXT.GetFieldNames(sourceFile),
                                                ".csv" => FreeText.CSV.CSV.GetFieldNames(sourceFile),
                                                _ => FreeText.Excel.Excel.GetFieldNames(sourceFile)
                                            };

                                            if (freeTextFields.Length == 0)
                                                throw new Exception("No valid fields found.");

                                            var position = freeTextFields.Any(f => f == "_position_") ? "_position_" :
                                                coordinateFieldName?[0];

                                            //Polymorphism: assigning derived class objects to base class objects
                                            FreeText.FreeText freeText = commandName switch {
                                                ".txt" => new FreeText.TXT.TXT(coordinateFieldName: position),
                                                ".csv" => new FreeText.CSV.CSV(coordinateFieldName: position),
                                                _ => new FreeText.Excel.Excel(coordinateFieldName: position)
                                            };

                                            freeText.OnMessagerEvent += (_, e) =>
                                            {
                                                var userStatus = !string.IsNullOrWhiteSpace(e.Message)
                                                    ? inputFileCount > 1
                                                        ? $"[{localI}/{inputFileCount}] {e.Message}"
                                                        : e.Message
                                                    : null;
                                                ShowProgress(message: userStatus, code: e.Code, progress: e.Progress);
                                            };
                                            freeText.Open(sourceFile);
                                            string targetFile;
                                            if (!isDirectory)
                                                targetFile = outputFile[0];
                                            else {
                                                var postfix = 0;
                                                do {
                                                    targetFile = Path.Combine(
                                                        outputFile[0],
                                                        Path.GetFileNameWithoutExtension(sourceFile) +
                                                        (postfix == 0 ? "" : $"({postfix})") + "." +
                                                        (format != null ? format[0] : "geojson"));
                                                    if (!File.Exists(targetFile))
                                                        break;
                                                    postfix++;
                                                } while (true);
                                            }

                                            var directoryName = Path.GetDirectoryName(targetFile);
                                            if (Directory.Exists(directoryName) == false)
                                                Directory.CreateDirectory(directoryName!);
                                            freeText.Export(
                                                saveAs: targetFile,
                                                format: format == null ? "geojson" : format[0],
                                                treePath: treePath?[0],
                                                extraDescription: description == null
                                                    ? null
                                                    : XElement.Parse(description[0])
                                            );
                                        }
                                    }
                                    else
                                        throw new Exception("Input and output parameters do not match.");
                                }
                            }
                            break;
                        default:
                            throw new Exception("");
                    }
                }
                catch (Exception error)
                {
                    if (!string.IsNullOrEmpty(error.Message))
                    {
                        Console.WriteLine();
                        Console.WriteLine($@"Exception: {error.Message}");
                    }
                    else
                    {
                        Console.WriteLine($@"Usage: {applicationName} [Command] [Options]");
                        Console.WriteLine();
                        Console.WriteLine(@"Command:");
                        Console.WriteLine(@"    -?/help");
                        Console.WriteLine(@"    mpj/mapgismpj");
                        Console.WriteLine(@"    mapgis");
                        Console.WriteLine(@"    shp/shapefile");
                        Console.WriteLine(@"    txt/csv/excel");
                        Console.WriteLine(@"Options:");
                        Console.WriteLine(@"    -[key] [value]");
                        Console.WriteLine();
                        Console.WriteLine($@"Run '{applicationName} [Command] -?/help' for more information on a command.");
                    }
                }

                SendKeys.SendWait("{ENTER}");
                return;

                ///// <param name="message">message</param>
                ///// <param name="code">statusCode（0/null=Pre-processing；1=processing；200=Post-processing）</param>
                ///// <param name="progress">progress（0～100，only for code = 1）</param>
                void ShowProgress(string message = null!, int? code = null, int? progress = null)
                {
                    var showProgressTask = Task.Run(
                        () =>
                        {
                            try
                            {
                                switch (code)
                                {
                                    case 1: //processing
                                        cursorPosition ??= (Console.CursorLeft, Console.CursorTop);
                                        if (cursorPosition != null)
                                        {
                                            Console.SetCursorPosition(0, cursorPosition.Value.Top);
                                            Console.Write(new string(' ', Console.WindowWidth));
                                            Console.SetCursorPosition(0, cursorPosition.Value.Top);
                                            Console.Write(@"{0} {1}%", message, progress);
                                        }
                                        break;
                                    default: //Pre-processing / Post-processing
                                        if (!string.IsNullOrWhiteSpace(message))
                                        {
                                            cursorPosition = (Console.CursorLeft, Console.CursorTop);
                                            if (cursorPosition != null)
                                            {
                                                if (cursorPosition.Value.Left > 0)
                                                    Console.WriteLine();
                                                Console.WriteLine(message);
                                            }
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                //
                            }
                        }
                    );
                    showProgressTask.Wait();
                }

                void CommandHelper(string command)
                {
                    switch (command)
                    {
                        case "mpj":
                        case "mapgismpj":
                            Console.WriteLine(@"Command: mpj/mapgismpj [Options]");
                            Console.WriteLine(@"    Options: -i/input InputFile[s] -o/output OutputFile/OutputFolder");
                            Console.WriteLine(@"        InputFile[s]: *.mpj");
                            Console.WriteLine(@"        OutputFile/OutputFolder: *.geojson/folderPath");
                            Console.WriteLine(@"Example:");
                            Console.WriteLine($@"   {applicationName} mpj -i ./mapgis.mpj -o ./test.geojson");
                            Console.WriteLine($@"   {applicationName} mpj -i ./mapgis1.mpj ./mapgis2.mpj -o ./testfolder");
                            break;
                        case "mapgis":
                            Console.WriteLine(@"Command: mapgis [Options]");
                            Console.WriteLine(@"    Options: -i/input InputFile[s] -o/output OutputFile/OutputFolder -f/format Format -t/treepath TreePath -d/description Description -pcolor Pcolor");
                            Console.WriteLine(@"        InputFile[s]: *.wt, *.wl, *.wp");
                            Console.WriteLine(@"        OutputFile/OutputFolder: *.shp, *.geojson, *.gml, *.kml, *.xml / folderPath");
                            Console.WriteLine(@"        Format: shp/shapefile, geojson[default], gml, kml, xml");
                            Console.WriteLine(@"        TreePath: null[default]");
                            Console.WriteLine(@"        Description: null[default]");
                            Console.WriteLine(@"        Pcolor: MapGisSlib/Pcolor.lib");
                            Console.WriteLine(@"Example:");
                            Console.WriteLine($@"   {applicationName} mapgis -i ./point.wt -o ./test.shp -f shapefile");
                            Console.WriteLine($@"   {applicationName} mapgis -i ./line.wl -o ./test.shp -f shapefile -pcolor ./Slib/Pcolor.lib");
                            Console.WriteLine($@"   {applicationName} mapgis -i ./point.wt ./line.wl ./polygon.wp -o ./testfolder -f shapefile");
                            break;
                        case "shp":
                        case "shapefile":
                            Console.WriteLine(@"Command: shp/shapefile [Options]");
                            Console.WriteLine(@"    Options: -i/input InputFile[s] -o/output OutputFile/OutputFolder -f/format Format -t/treepath TreePath -d/description Description -c/codepage CodePage");
                            Console.WriteLine(@"        InputFile[s]: *.shp");
                            Console.WriteLine(@"        OutputFile/OutputFolder: *.geojson, *.gml, *.kml, *.shp, *.xml / folderPath");
                            Console.WriteLine(@"        Format: geojson[default], gml, kml, shp, xml");
                            Console.WriteLine(@"        TreePath: null[default]");
                            Console.WriteLine(@"        Description: null[default]");
                            Console.WriteLine(@"        CodePage: 936[default]");
                            Console.WriteLine(@"Example:");
                            Console.WriteLine($@"   {applicationName} shapefile -i ./theme.shp -o ./test.geojson -f geojson");
                            break;
                        case "txt":
                        case "csv":
                        case "excel":
                            Console.WriteLine(@"Command: excel/txt/csv [Options]");
                            Console.WriteLine(@"    Options: -i/input InputFile[s] -o/output OutputFile/OutputFolder -f/format Format -t/treepath TreePath -d/description Description -c/coordinate Coordinate");
                            Console.WriteLine(@"        InputFile[s]: *.txt/csv/xls/xlsx/xlsb");
                            Console.WriteLine(@"        OutputFile/OutputFolder: *.geojson, *.gml, *.kml, *.shp, *.xml");
                            Console.WriteLine(@"        Format: geojson[default], gml, kml, shp, xml");
                            Console.WriteLine(@"        TreePath: null[default]");
                            Console.WriteLine(@"        Description: null[default]");
                            Console.WriteLine(@"        Coordinate: _position_[default]");
                            Console.WriteLine(@"Example:");
                            Console.WriteLine($@"   {applicationName} txt -i ./line.txt -o ./test.shp -f shp");
                            Console.WriteLine($@"   {applicationName} csv -i ./line.csv -o ./test.shp -f shp");
                            Console.WriteLine($@"   {applicationName} excel -i ./line.xls -o ./test.xml -f xml");
                            break;
                    }
                }
            }
        }
    }
}
