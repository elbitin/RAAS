/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Client.
 *
 * RAAS Client is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Client is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Client. If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Windows.Management.Deployment;
using System.IO;
using Windows.ApplicationModel;
using WixToolset.Dtf.WindowsInstaller;
using System.Windows.Forms;

namespace CustomAction1
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult InstallRAASClientContextMenu(Session session)
        {
            session.Log("Begin CustomAction InstallRAASClientContextMenu");
            string installLocation = session["APPLICATIONFOLDER"];
            string sparsePkgFile = session["MSIXFILE"];
            String externalUri = installLocation;
            String packageUri = Path.Combine(installLocation, sparsePkgFile);
            registerSparsePackage(externalUri, packageUri);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UninstallRAASClientContextMenu(Session session)
        {
            session.Log("Begin CustomAction InstallRAASClientContextMenu");
            string installLocation = session["APPLICATIONFOLDER"];
            string sparsePkgFile = session["MSIXFILE"];
            string sparsePkgName = session["MSIXAPPNAME"];
            PackageManager packageManager = new PackageManager();
            var packages = packageManager.FindPackagesForUser("");
            foreach (var package in packages)
	        {
                if (package.Id.Name != sparsePkgName)
                    continue;
                String fullName = package.Id.FullName;
                Windows.Foundation.IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = packageManager.RemovePackageAsync(fullName);
                ManualResetEvent opCompletedEvent = new ManualResetEvent(false);
                deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };
                Debug.WriteLine("Uninstalling package..");
                opCompletedEvent.WaitOne();
            }
            return ActionResult.Success;
        }

        private static bool registerSparsePackage(string externalLocation, string sparsePkgPath)
        {
            bool registration = false;
            try
            {
                Uri externalUri = new Uri(externalLocation);
                Uri packageUri = new Uri(sparsePkgPath);
                Console.WriteLine("exe Location {0}", externalLocation);
                Console.WriteLine("msix Address {0}", sparsePkgPath);
                Console.WriteLine("  exe Uri {0}", externalUri);
                Console.WriteLine("  msix Uri {0}", packageUri);
                PackageManager packageManager = new PackageManager();

                //Declare use of an external location
                var options = new AddPackageOptions();
                options.ExternalLocationUri = externalUri;

                Windows.Foundation.IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = packageManager.AddPackageByUriAsync(packageUri, options);
                ManualResetEvent opCompletedEvent = new ManualResetEvent(false);
                deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };
                Console.WriteLine("Installing package {0}", sparsePkgPath);
                Debug.WriteLine("Waiting for package registration to complete...");
                opCompletedEvent.WaitOne();
                if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Error)
                {
                    Windows.Management.Deployment.DeploymentResult deploymentResult = deploymentOperation.GetResults();
                    Debug.WriteLine("Installation Error: {0}", deploymentOperation.ErrorCode);
                    Debug.WriteLine("Detailed Error Text: {0}", deploymentResult.ErrorText);
                }
                else if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Canceled)
                {
                    Debug.WriteLine("Package Registration Canceled");
                }
                else if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    registration = true;
                    Debug.WriteLine("Package Registration succeeded!");
                }
                else
                {
                    Debug.WriteLine("Installation status unknown");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddPackageSample failed, error message: {0}", ex.Message);
                Console.WriteLine("Full Stacktrace: {0}", ex.ToString());
                return registration;
            }
            return registration;
        }
    }
}