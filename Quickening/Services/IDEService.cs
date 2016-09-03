using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

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

        internal static void AddFolderAndFiles(Project project, string path)
        {
            try
            {
                // Adding a folder.
                if (string.IsNullOrEmpty(Path.GetExtension(path)))
                {
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
                        if (item.Name.ToLower() == Path.GetDirectoryName(path).ToLower())
                        {
                            file = item;
                            break;
                        }
                    }

                    // If not, create it.
                    if(file == null)
                        folder.ProjectItems.AddFromFile(path);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
