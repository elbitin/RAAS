/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Server.
 *
 * RAAS Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Server. If not, see <https://www.gnu.org/licenses/>.
 */
﻿// Copyright (c) Elbitin
using System.ServiceModel;

namespace Elbitin.Applications.RAAS.RAASServer.RAASSvr
{
    [ServiceContract(Namespace = "http://applications.elbitin.com/WCF/RAASServer/2.0.0/RAASServer.RAASSvr.IRAASServerService",
        SessionMode = SessionMode.Required, CallbackContract = typeof(IRAASServerChange))]
    public interface IRAASServerService
    {
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Unsubscribe();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetShortcutsXml();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetShareXml();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool GetLoggedInState();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool LogOff();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        byte[] GetIcon(string iconName);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string[] GetAutostartEntries();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        byte[] GetShortcutIcon(string path);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Reboot();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetVersion();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetShortcutsServerPath();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetKeepAliveAgentPath();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetAutostartPath();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string GetAppNamesPath();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        bool GetCanReboot();

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void UpdateShortcuts();
    }
}
