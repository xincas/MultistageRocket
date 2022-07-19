using System.IO;

namespace util
{
    public abstract class LoggerBase
    {
        public abstract void LogAppend(float velocity, float mass, float height);
        public abstract void Log(float velocity, float mass, float height);
    }

    public class Logger: LoggerBase
    {
        private string CurrentDirectory { get; set; }
        private string FileName { get; set; }
        private string FilePath { get; set; }

        public Logger()
        {
            CurrentDirectory = Directory.GetCurrentDirectory();
            FileName = "data_u.txt";
            FilePath = CurrentDirectory + "/" + FileName;
        }
        
        public override void LogAppend(float velocity, float mass, float height)
        {
            using (System.IO.StreamWriter w = System.IO.File.AppendText(this.FilePath))
            {
                w.WriteLine("{0} {1} {2}", velocity, mass, height);
            }
        }

        public override void Log(float velocity, float mass, float height)
        {
            using (System.IO.StreamWriter w = System.IO.File.CreateText(this.FilePath))
            {
                w.WriteLine("{0} {1} {2}", velocity, mass, height);
            }
        }
    }
}