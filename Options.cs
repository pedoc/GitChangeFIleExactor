using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace GitChangeFIleExactor
{
    public class Options
    {
        [Option('r', Required = true, HelpText = "仓库所在目录")]
        public string RepoDir { get; set; }
        [Option('c', Required = true, HelpText = "提交Id(8位)或Sha，支持多个值")]
        public IEnumerable<string> Commits { get; set; }=new string[]{};

        [Option('o', Required = false, HelpText = "输出目录，如果不指定则在程序目录下")]
        public string OutputDir { get; set; }

        public bool Check()
        {
            if (!Directory.Exists(RepoDir))
            {
                Console.WriteLine("输入的文件夹不存在");
                return false;
            }

            if (!Commits.Any())
            {
                Console.WriteLine("未指定任何提交标识");
                return false;
            }

            if (string.IsNullOrEmpty(OutputDir))
            {
                OutputDir = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-mm-dd HH fff"));
            }
            if (Directory.Exists(OutputDir))
            {
                Console.WriteLine($"指定的输出目录已存在，可能导致文件覆盖 {OutputDir}");
            }
            else Directory.CreateDirectory(OutputDir);
            
            return true;
        }
    }
}
