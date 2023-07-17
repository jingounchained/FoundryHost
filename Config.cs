using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace FoundryHost
{
    public class Config
    {
        
        public string DataPath { get; set; } = Environment.GetEnvironmentVariable("FOUNDRY_VTT_DATA_PATH", EnvironmentVariableTarget.Machine);
        private string _mainjs = string.Empty;
        public string Foundry
        {
            get
            {
                if (string.IsNullOrEmpty(_mainjs))
                {
                    _mainjs = Environment.GetEnvironmentVariable("FOUNDRY", EnvironmentVariableTarget.Machine); 
                    if(string.IsNullOrEmpty(_mainjs)) _mainjs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\app\main.js");
                    if (string.IsNullOrEmpty(_mainjs)) throw new Exception($"Path to main.js was not provided and could not be found at {_mainjs}.");
                }
                return _mainjs;
            }
            set
            {
                _mainjs = value;
            }
        }
        private int _port;
        public int Port { 
            get
            {
                if(_port == 0)
                {
                    if(int.TryParse(Environment.GetEnvironmentVariable("FOUNDRY_VTT_PORT", EnvironmentVariableTarget.Machine), out int result))
                    {
                        _port = result;
                    }
                }
                return _port;
            }
            set
            {
                _port = value;
            }
        } 

        private static Config _instance;
        private static string[] _args;
        public static Config Instance 
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Config(_args);
                }
                return _instance;
            }
        }
        public static async void Setup(string[] args)
        {
            _args = args;
            _instance = new Config(_args);
        }
        private Config(string[] args)
        {
            /*
             * Load args from the Command Line
             * Only check for and read the options.json file if parameters are missing
             * parsed parameters should take precedence over the json file
             */
            if(args != null) ParseArgs(args);
            if (string.IsNullOrEmpty(Foundry))
            {
                throw new Exception("The Foundry Path must either be provided as a Start Parameter or as an Environmental Variable." +
                    "\r\nSyntax:\r\n" +
                    "\tParameter should be --foundry=\"\\path\\to\\main.js\"\r\n" +
                    "\tEnvironmental variable should be named named FOUNDRY and set to \"path\\to\\main.js\"");
            }
            else if (string.IsNullOrEmpty(DataPath) || Port == 0)
            {
                OptionsFile options = OptionsFile.Load();
                if(options == null) Environment.Exit(1);
                if (string.IsNullOrEmpty(DataPath)) DataPath = options.dataPath;
                if (Port == 0) Port = options.port;
            }
        }

        private void ParseArgs(string[] args)
        {
            foreach (string arg in args)
            {
                int index = arg.IndexOf('=') + 1;
                if (index == 1) continue;
                if (arg.ToUpper().StartsWith("--FOUNDRY")) Foundry = arg.Substring(index);
                else if (arg.ToUpper().StartsWith("--DATAPATH")) DataPath = arg.Substring(index);
                else if (arg.ToUpper().StartsWith("--PORT"))
                {
                    if (int.TryParse(arg.Substring(index), out int parsedPort))
                    {
                        Port = parsedPort;
                    }
                    else
                    {
                        throw new Exception($"Could not parse the port from command line parameter {arg}");
                    }
                }
            }
        }
    }

    [Serializable]
    public class OptionsFile
    {
        [JsonProperty("dataPath")]
        public string dataPath { get; set; } = "";

        [JsonProperty("port")]
        public int port { get; set; }

        public static string FilePath
        {
            get
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return System.IO.Path.Combine(appdata, @"FoundryVTT\Config\options.json");
            }
        }
        public OptionsFile() 
        {


        }
        
        

        public static OptionsFile Load()
        {
            try
            {
                if (!File.Exists(FilePath)) throw new Exception($"File Not Found: {FilePath}");
                string json = string.Empty;
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    json = reader.ReadToEnd();
                }
                return json;
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
                return null;
            }
        }

        public static implicit operator OptionsFile(string json)
        {
            return JsonConvert.DeserializeObject<OptionsFile>(json);
        }
    }
}
