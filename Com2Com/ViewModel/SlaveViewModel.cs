using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using Com2Com.Model;
using Com2Com.Common;
using Com2Com.View;
using GalaSoft.MvvmLight.CommandWpf;

namespace Com2Com.ViewModel
{
    public class SlaveViewModel : ViewModelBase
    {
        private ProtocolFrame _protocolFrame;

        private SlaveModel _slaveModel;

        public SlaveViewModel()
        {
            CreateNavigateToMasterPageCommand();
            CreateSendProtocolFrameCommand();
        }

        #region Commands
        /// <summary>
        /// 
        /// </summary>
        public ICommand NavigateToMasterPage { get; private set; }
        private void ExecuteNavigateToMasterPageCommand()
        {
            NavigationHelper.NavigateTo<MasterDevicePage>(_protocolFrame);
        }
        private void CreateNavigateToMasterPageCommand()
        {
            NavigateToMasterPage = new RelayCommand(ExecuteNavigateToMasterPageCommand);
        }
        /// <summary>
        /// 
        /// </summary>
        public ICommand SendProtocolFrame { get; set; }
        private void ExecuteSendProtocolFrameCommand()
        { }
        private void CreateSendProtocolFrameCommand()
        {
            SendProtocolFrame = new RelayCommand(ExecuteSendProtocolFrameCommand);
        }
        #endregion
    }
}
