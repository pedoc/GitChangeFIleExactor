using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using LibGit2Sharp;

namespace GitChangeFIleExactor
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(option =>
                {
                    if (option.Check())
                    {
                        var repo = new Repository(option.RepoDir);
                        var waitCommits = new List<Commit>();
                        foreach (var commit in repo.Commits)
                        {
                            if (option.Commits.Contains(commit.Id.ToString(8).ToString())
                            || option.Commits.Contains(commit.Id.ToString()))
                            {
                                waitCommits.Add(commit);
                            }
                        }
                        var orderdWaitCommits= waitCommits.OrderBy(item => item.Committer.When).ToList();//按照提交时间升序，确保如果一个文件在多次commit中被修改，使用最后的版本
                        Console.WriteLine($"输入总 commitId数：{option.Commits.Count()}，实际发现数：{orderdWaitCommits.Count()}，详情如下（按时间升序），请确认是否继续(y/n)?");
                        orderdWaitCommits.ForEach(item =>
                        {
                            Console.WriteLine($"{item.Id}:{item.MessageShort}");
                        });
                        var input = Console.ReadLine();
                        if (!string.IsNullOrEmpty(input) && input.Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Red;
                            foreach (var commit in orderdWaitCommits)
                            {
                                var parentCommit = commit.Parents.First();
                                var patchs = DiffCommit(repo, parentCommit.Id, commit.Id);
                                foreach (var patch in patchs)
                                {
                                    var realPath = Path.Combine(option.RepoDir, patch.Path);
                                    var outputPath = Path.Combine(option.OutputDir, patch.Path);
                                    var outputPathDir = Path.GetDirectoryName(outputPath);
                                    if (outputPathDir != null && !Directory.Exists(outputPathDir)) Directory.CreateDirectory(outputPathDir);
                                    if (patch.Status != ChangeKind.Deleted)
                                    {
                                        if(File.Exists(outputPath)) Console.WriteLine($"文件已存在，将被覆盖({outputPath})");
                                        File.Copy(realPath, outputPath, true);
                                        Console.WriteLine($"已拷贝变化文件({realPath})至({outputPath})");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{patch.Path} 状态为已删除，自动跳过");
                                    }
                                }
                            }
                            Process.Start(option.OutputDir);//自动打开资源管理器
                        }
                        else Console.WriteLine("输入有误");
                    }
                    Console.WriteLine("按任意键退出");
                    Console.ReadKey();
                }).WithNotParsed(error =>
                {
                    Console.WriteLine($"参数输入有误，无法完成任务");
                    Console.ReadKey();
                });
        }

        private static Patch DiffCommit(IRepository repo, ObjectId oldId, ObjectId newId)
        {
            var commitLeft = repo.Lookup<Commit>(oldId);
            var commitRight = repo.Lookup<Commit>(newId);
            Console.WriteLine($"正在比对差异 [old]{commitLeft.Message} - [new]{commitRight.Message}");
            var patch = repo.Diff.Compare<Patch>(commitLeft.Tree, commitRight.Tree);
            return patch;
        }
    }
}
