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
        /// <summary>
        /// Adds an item to the current project.
        /// </summary>
        /// <param name="project">The current project.</param>
        /// <param name="relPath">The relative path of the item to add.</param>
        /// <param name="absPath">The absolute path of the item to add.</param>
        /// <param name="isFile">If the absolute path relates to a file.</param>
        internal static void AddItemToProject(Project project, string relPath, string absPath, bool isFile)
        {
            /*
             * We need to ensure any parent directories already exist before creating the new project item.
             * As we only have a relative path we will use string split to get the directories and then check them from the top down.
             * Linq also seems not to be an option, so the traversing needs to be done with loops.
             */
             
            // Split relative path into directories and the file if it exists.
            // We need the relative path here as that is how the project manages its' items.
            var dirs = relPath.Split('\\');

            // Get top level project items.
            var levelItems = project.ProjectItems;
            foreach (var d in dirs)
            {
                // Avoid creating folders out of files.
                if (isFile && !string.IsNullOrEmpty(Path.GetExtension(d)))
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
                        return;
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
                    // Create the file if needed and add it to the project.
                    if (!File.Exists(absPath))
                        File.Create(absPath).Dispose();

                    // Here we need the absolute path, unlike when adding a folder, because reasons.
                    levelItems.AddFromFile(absPath);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
                    return;
                }
            }
        }
    }
}
