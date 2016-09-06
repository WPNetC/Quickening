using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static Quickening.Services.ProjectService;

namespace Quickening.Services
{
    internal static class IDEService
    {
        internal static Project GetCurrentProject()
        {
            try
            {
                IntPtr hierarchyPointer, selectionContainerPointer;
                Object selectedObject = null;
                IVsMultiItemSelect multiItemSelect;
                uint projectItemId;

                IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

                monitorSelection.GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelect, out selectionContainerPointer);

                IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                      projectItemId,
                                                      (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                      out selectedObject));
                }

                Project selectedProject = selectedObject as Project;
                return selectedProject;
            }
            catch
            {
                return null;
            }
        }

        /*
        private static void AddFolderOrFile(Project project, string path)
        {
            try
            {
                // Adding a folder.
                if (string.IsNullOrEmpty(Path.GetExtension(path)))
                {
                    foreach (ProjectItem item in project.ProjectItems)
                    {
                        if (item.Kind == IDE_FOLDER_GUID)
                        {
                            var x = item.FileNames[0];
                        }
                    }
                    // Adds a new folder to the solution.
                    project.ProjectItems.AddFolder(path);
                    return;
                }
                // Adding a file.
                else
                {
                    // Check if containing folder exists in project.
                    ProjectItem folder = null;
                    foreach (ProjectItem item in project.ProjectItems)
                    {
                        if (item.Name.ToLower() == Path.GetDirectoryName(path).ToLower())
                        {
                            folder = item;
                            break;
                        }
                    }

                    // If not, create it.
                    if (folder == null)
                    {
                        folder = project.ProjectItems.AddFolder(path);
                    }

                    // Check if file already exists in project.
                    ProjectItem file = null;
                    foreach (ProjectItem item in folder.ProjectItems)
                    {
                        //TraverseProjectItem(item);
                    }

                    // If not, create it.
                    if (file == null)
                        folder.ProjectItems.AddFromFile(path);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        */

        internal static void TraverseProjectItem(Project project, string relPath, string absPath)
        {
            /*
             * We need to ensure any parent directories already exist before creating the new project item.
             * As we only have a relative path we will use string split to get the directories and then check them from the top down.
             * Linq also seems not to be an option, so the traversing needs to be done with loops.
             */

            // Get if we are adding a file.
            bool isFile = !string.IsNullOrEmpty(Path.GetExtension(relPath));

            // Split path into directories and the file if it exists.
            var dirs = relPath.Split('\\');

            // Get top level project items.
            var levelItems = project.ProjectItems;
            foreach (var d in dirs)
            {
                // Avoid creating folders out of files.
                if (!string.IsNullOrEmpty(Path.GetExtension(d)))
                    continue;

                ProjectItem folder = null;

                // Check if folder already exists.
                foreach (ProjectItem levelItem in levelItems)
                {
                    if (levelItem.Name.ToLower().Trim() == d.ToLower().Trim())
                    {
                        folder = levelItem;
                        break;
                    }
                }

                // If not, create it.
                if (folder == null)
                {
                    try
                    {
                        folder = levelItems.AddFolder(d);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
                        throw ex;
                    }
                }

                // Step down a level.
                levelItems = folder.ProjectItems;
            }

            // If we are adding a file we should now be in the correct level.
            if (isFile)
            {
                try
                {
                    if(!File.Exists(absPath))
                        File.Create(absPath).Dispose();

                    levelItems.AddFromFile(absPath);
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
                }
            }
        }
    }
}
