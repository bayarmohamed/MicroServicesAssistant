using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace EM2AExtension.Logic
{
    public class SolutionHelper
    {
        private readonly DTE2 _dte;

        public SolutionHelper(DTE2 dte)
        {
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        public void AddProjectToSolution(string projectFilePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(projectFilePath) || !File.Exists(projectFilePath))
            {
                throw new FileNotFoundException("Project file not found.", projectFilePath);
            }
            //https://learn.microsoft.com/en-us/dotnet/api/envdte.dte?view=visualstudiosdk-2022
            var solution = _dte.Solution;

            if (solution == null || !solution.IsOpen)
            {
                throw new InvalidOperationException("No solution is open.");
            }

            // Add the project to the solution
            solution.AddFromFile(projectFilePath);
        }

    }
}
