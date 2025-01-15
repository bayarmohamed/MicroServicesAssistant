using EnvDTE;
using EnvDTE80;
using Project = EnvDTE.Project;
namespace EM2AExtension.Logic
{
    public class SolutionFolderFinder
    {
        public static ProjectItem FindSolutionSubFolder(DTE2 dte, string folderName)
        {
            foreach (Project project in dte.Solution.Projects)
            {
                if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
                {
                    var folder = FindInProjectItems(project.ProjectItems, folderName);
                    if (folder != null)
                        return folder;
                }
            }
            return null;
        }

        private static ProjectItem FindInProjectItems(ProjectItems items, string folderName)
        {
            foreach (ProjectItem item in items)
            {
                if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder ||
                    item.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
                {
                    if (item.Name == folderName)
                        return item;

                    // Recursively search in subitems
                    var subFolder = FindInProjectItems(item.ProjectItems, folderName);
                    if (subFolder != null)
                        return subFolder;
                }
            }
            return null;
        }
    }
}
