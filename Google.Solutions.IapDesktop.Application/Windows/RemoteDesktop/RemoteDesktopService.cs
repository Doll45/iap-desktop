﻿using Google.Solutions.Compute;
using Google.Solutions.IapDesktop.Application.ObjectModel;
using Google.Solutions.IapDesktop.Application.Settings;
using Google.Solutions.IapDesktop.Windows;
using System;
using WeifenLuo.WinFormsUI.Docking;

namespace Google.Solutions.IapDesktop.Application.Windows.RemoteDesktop
{
    public interface IRemoteDesktiopSession
    {
        void Close();
    }

    public class RemoteDesktopService
    {
        private readonly IExceptionDialog exceptionDialog;
        private readonly IEventService eventService;
        private readonly DockPanel dockPanel;

        public RemoteDesktopService(IServiceProvider serviceProvider)
        {
            this.dockPanel = serviceProvider.GetService<IMainForm>().MainPanel;
            this.exceptionDialog = serviceProvider.GetService<IExceptionDialog>();
            this.eventService = serviceProvider.GetService<IEventService>();
        }

        public IRemoteDesktiopSession Connect(
            VmInstanceReference vmInstance,
            string server,
            ushort port,
            VmInstanceSettings settings)
        {
            var rdpPane = new RemoteDesktopPane(
                this.eventService,
                this.exceptionDialog,
                vmInstance,
                settings);
            rdpPane.Show(this.dockPanel, DockState.Document);
            
            rdpPane.Connect(server, port);

            return rdpPane;
        }
    }

    public abstract class RemoteDesktopEventBase
    {
        public VmInstanceReference Instance { get; }

        public RemoteDesktopEventBase(VmInstanceReference vmInstance)
        {
            this.Instance = vmInstance;
        }
    }

    public class RemoteDesktopConnectionSuceededEvent : RemoteDesktopEventBase
    {
        public RemoteDesktopConnectionSuceededEvent(VmInstanceReference vmInstance) : base(vmInstance)
        {
        }
    }

    public class RemoteDesktopConnectionFailedEvent : RemoteDesktopEventBase
    {
        public RdpException Exception { get; }

        public RemoteDesktopConnectionFailedEvent(VmInstanceReference vmInstance, RdpException exception) 
            : base(vmInstance)
        {
            this.Exception = exception;
        }
    }

    public class RemoteDesktopWindowClosedEvent : RemoteDesktopEventBase
    {
        public RdpException Exception { get; }

        public RemoteDesktopWindowClosedEvent(VmInstanceReference vmInstance) : base(vmInstance)
        {
        }
    }
}
