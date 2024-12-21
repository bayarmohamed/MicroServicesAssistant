using System.Threading.Tasks;

namespace EM2AExtension.Logic
{
    public interface IMaker
    {
        void AddFileToProject(EnvDTE.Project project, string fileName, string content);
        Task AddProjectToSolution(string project);
        Task<string> CreateProject(string projectName);
        string FindSolutionPath(string currentDirectory);
        EnvDTE.Project GetDefaultStartupProjects();
    }
}