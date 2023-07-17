using System.Diagnostics;

namespace FoundryHost
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        private bool IsRunning
        {
            get
            {
                return node != null && started && !node.HasExited;
            }
        }
        private Process node;
        private bool started;
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                
                if (stoppingToken.IsCancellationRequested)
                {
                    if (IsRunning) node.Kill();
                    return;
                }
                
                else if (IsRunning) { return; }
                node = new Process();
                node.StartInfo.FileName = "node.exe";
                node.StartInfo.Arguments = $"\"{Config.Instance.Foundry}\" --dataPath=\"{Config.Instance.DataPath}\" --port=\"{Config.Instance.Port}\"";
                node.StartInfo.UseShellExecute = false;
                node.StartInfo.RedirectStandardOutput = true;
                node.StartInfo.RedirectStandardInput = true;
                started = node.Start();
                if (started) LogManager.Startup();
                await node.WaitForExitAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                LogManager.Cancelled();
            }
            catch (Exception ex)
            {
                LogManager.Error(ex);
            }
            finally
            {
                if (IsRunning) node.Kill();
                LogManager.Exited();
            }
        }
    }
}