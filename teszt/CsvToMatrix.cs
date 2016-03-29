using System.IO;
using System.Text;

namespace RobotMoverGUI {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    internal class CsvToMatrix {
        private readonly StreamReader _sr;

        public CsvToMatrix(string fileName) {
            FileName = fileName;
            var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            _sr = new StreamReader(fs, Encoding.GetEncoding("iso-8859-2"), false);
        }

        private string FileName { get; }

        public bool[,] Map { get; private set; }


        private int LineCount() {
            var lineNum = 0;
            while (_sr.ReadLine() != null) lineNum ++;
            return lineNum;
        }

        public void Read() {
            var lineCount = LineCount();
            _sr.DiscardBufferedData();
            _sr.BaseStream.Seek(0, SeekOrigin.Begin);
            var line = _sr.ReadLine();
            var i = 0;
            if (line != null) {
                var splitLine = line.Split(';');
                Map = new bool[lineCount, splitLine.LongLength];
                //MessageBox.Show(lineCount + " " + splitLine.LongLength);
                while (line != null) {
                    var j = 0;
                    foreach (var s in splitLine) {
                        switch (s) {
                            case "1":
                                Map[i, j] = true;
                                break;
                            case "0":
                                Map[i, j] = false;
                                break;
                        }
                        j++;
                    }
                    line = _sr.ReadLine();
                    splitLine = line?.Split(';');
                    i++;
                }
            }
            _sr.Close();
        }
    }
}