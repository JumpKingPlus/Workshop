using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JumpKing;
using System.Windows.Forms;
using System.Diagnostics;

public static class DependencyDealer
{
    /// <summary>
    /// CallResult to understand if Steam has installed the dependency required.
    /// </summary>
    //private static CallResult<RemoteStorageSubscribePublishedFileResult_t> _installation;
    
    /// <summary>
    /// Max wait time for the <see cref="_installation"/> to return something, 
    /// if this passes it'll tell the user that Steam timed out.
    /// </summary>
    internal static int MaxWaitTime = 15000;

    /// <summary>
    /// <see cref="Thread.Sleep(int)"/> (Milliseconds between each check).
    /// </summary>
    internal static int CheckEach = 500;

    /// <summary>
    /// Field used inside <see cref="WaitForInstallation"/>'s while() to define if the callback (finally) came back.
    /// </summary>
    //private static bool callback_recieved;

    /// <summary>
    /// Could happen that the installation fails.
    /// </summary>
    //private static bool did_installation_fail;

    /// <summary>
    /// Since Steam doesn't really check this, this is what you want to call on <c>YourMod.BeforeLevelLoad()</c> to check if your dependencies are installed properly.
    /// </summary>
    /// <param name="modName">Name of the mod that requires the dependency.</param>
    /// <param name="dependencyWorkshopIds">The required dependencies/items as <see cref="PublishedFileId_t"/>.</param>
    public static void InstallDependenciesIfMissing(string modName, params PublishedFileId_t[] dependencyWorkshopIds)
    {
        List<PublishedFileId_t> missingDependencies = new List<PublishedFileId_t>();
        foreach (var dependencyWorkshopId in dependencyWorkshopIds)
        {
            if (CheckIfInstalled(dependencyWorkshopId))
            {
                // the item is installed & subscribed therefore will (hopefully) work in JK
                continue;
            }

            // add to the missing dependencies list to install
            missingDependencies.Add(dependencyWorkshopId);
        }

        // all the items are installed & subscribed therefore will (hopefully) work in JK
        if (missingDependencies.Count == 0)
        {
            return;
        }

        // text formatting
        string text = "\"{modName}\" is missing one or more required dependencies in order for the mod to be working correctly.\n\n";
        missingDependencies.ForEach(dependencyMissing =>
        {
            text += $"- {dependencyMissing}";
        });
        text += "\n\nIt will now attempt to install them. (You will need an internet connection)";

        MessageBox.Show(
            caption: modName,
            text: text,
            buttons: MessageBoxButtons.OK,
            icon: MessageBoxIcon.Warning
        );

        bool failed_to_install = false;
        foreach (var dependencyWorkshopId in missingDependencies)
        {
            // if item is not subscribed OR not installed OR whatever is going on,
            // then install the dependency
            InstallDependency(dependencyWorkshopId);
        
            // wait before proceeding...
            if (!WaitForInstallation(modName, dependencyWorkshopId))
            {
                failed_to_install = true;
            }
        }

        if (!failed_to_install)
        {
            MessageBox.Show(
                caption: modName,
                text: $"\"{modName}\"'s required dependencies have been successfully installed." +
                $"Jump King will now close.",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Information
            );
        }

        // dependencies tend to not work right away in JK,
        // prompt the user to restart JK
        Game1.instance.Exit();
    }

    /// <summary>
    /// Checks if Steam has the item required subscribed and installed on the machine.
    /// </summary>
    /// <param name="dependencyWorkshopId">The required dependency/item as Workshop ID.</param>
    /// <returns>Returns if the item is installed or not.</returns>
    private static bool CheckIfInstalled(PublishedFileId_t dependencyWorkshopId)
    {
        // get current state for the required dependency
        var itemState = SteamUGC.GetItemState(dependencyWorkshopId);

        return itemState == ((uint)EItemState.k_EItemStateSubscribed + (uint)EItemState.k_EItemStateInstalled);
    }

    /// <summary>
    /// Waits until either Steam has the item installed or the <see cref="MaxWaitTime"/> times out.
    /// </summary>
    /// <param name="modName">Name of the mod that requires the dependency.</param>
    /// <param name="dependencyWorkshopId">The required dependency/item as Workshop ID.</param>
    /// <returns></returns>
    private static bool WaitForInstallation(string modName, PublishedFileId_t dependencyWorkshopId)
    {
        int time = 0;
        bool is_installed = false;

        // either the timer goes out or the callback comes back first
        while (time < MaxWaitTime && !is_installed)
        {
            is_installed = CheckIfInstalled(dependencyWorkshopId);
            time += CheckEach;
            Thread.Sleep(CheckEach);
        }

        // if the time goes out first then tell
        if (time > MaxWaitTime)
        {
            MessageBox.Show(
                caption: modName,
                text: $"\"{modName}\" couldn't install the required dependency because Steam timed out. " +
                $"You'll need to manually installed the required dependency.",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Error
            );
            Process.Start($"steam://url/CommunityFilePage/{dependencyWorkshopId}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tells Steam to install the required item.
    /// </summary>
    /// <param name="dependencyWorkshopId">The required dependency/item as Workshop ID.</param>
    private static void InstallDependency(PublishedFileId_t dependencyWorkshopId)
    {
        // reset previous value (if used)
        //callback_recieved = false;

        // force installation of dependency, get the call for a result
        var call = SteamUGC.SubscribeItem(dependencyWorkshopId);

        //var callResult = new CallResult<RemoteStorageSubscribePublishedFileResult_t>((t, failure) =>
        //{
        //    // assign something when the callresults happen
        //    if (t.m_nPublishedFileId == dependencyWorkshopId)
        //    {
        //        callback_recieved = true;
        //        did_installation_fail = t.m_eResult != EResult.k_EResultOK;
        //    }
        //});

        //// sets the api call to the callresult's class and this class field
        //callResult.Set(call);
        //_installation = callResult;
    }
}