using System.IO;

namespace util
{
    public abstract class LoggerBase
    {
        public abstract void LogAppend(float velocity, float mass, float height, float time);
        public abstract void Log(float velocity, float mass, float height, float time);
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
        
        public override void LogAppend(float velocity, float mass, float height, float time)
        {
            using (System.IO.StreamWriter w = System.IO.File.AppendText(this.FilePath))
            {
                w.WriteLine(
                    "{0} {1} {2} {3}", 
                    velocity.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")), 
                    mass.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")), 
                    height.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")),
                    time.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));
            }
        }

        public override void Log(float velocity, float mass, float height, float time)
        {
            using (System.IO.StreamWriter w = System.IO.File.CreateText(this.FilePath))
            {
                w.WriteLine(
                    "{0} {1} {2} {3}", 
                    velocity.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")), 
                    mass.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")), 
                    height.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")),
                    time.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));
            }
        }
    }
}