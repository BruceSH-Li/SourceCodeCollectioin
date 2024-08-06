using MXY_Chat.Client.Models;
using MXY_Chat.Client.SDK;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace MXY_Chat.Client.ViewModels
{
    public class AIViewModel : BindableBase
    {
        private SparkWebSDK sdk;
        public AIViewModel()
        {
            SendCmd = new DelegateCommand(Send, () => !string.IsNullOrEmpty(this.MsgToSend));
            sdk = new SparkWebSDK();
        }
        private async void Send()
        {
            sdk.Setup(this.AppId, this.ApiSecret, this.ApiKey, this.SelectedVersion);
            this.Messages.Add(new MessageModel(MessageType.Send, this.MsgToSend));
            var responseMessage = new MessageModel(MessageType.Receive);
            this.Messages.Add(responseMessage);
            var (ok, errMsg) = await sdk.Ask(new List<string>() { this.MsgToSend }, new System.Threading.CancellationToken(), strs =>
            {
                foreach (var str in strs)
                {
                    responseMessage.AddMsg(str);
                }
            });
            if (!ok)
            {
                MessageBox.Show(errMsg, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MsgToSend = "";
            }
        }

        private ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> Messages
        {
            get { return messages; }
            set { messages = value; }
        }
        private string apiKey = "e07573cde4b58ec7395a0b2c927c84df";
        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; this.RaisePropertyChanged(nameof(ApiKey)); }
        }

        private string apiSecret = "NmJlOWFhMGZjY2RmMGRiOGZiNDg3Mzkw";
        public string ApiSecret
        {
            get { return apiSecret; }
            set { apiSecret = value; this.RaisePropertyChanged(nameof(ApiSecret)); }
        }


        private string appId = "8249310e";
        public string AppId
        {
            get { return appId; }
            set { appId = value; this.RaisePropertyChanged(nameof(AppId)); }
        }

        private SparkVersions selectedVersion = SparkVersions.V3_0;
        public SparkVersions SelectedVersion
        {
            get { return selectedVersion; }
            set { selectedVersion = value; this.RaisePropertyChanged(nameof(SelectedVersion)); }
        }


        private string msgToSend;
        public string MsgToSend
        {
            get { return msgToSend; }
            set
            {
                msgToSend = value;
                this.RaisePropertyChanged(nameof(MsgToSend));
                this.SendCmd?.RaiseCanExecuteChanged();
            }
        }
        public DelegateCommand SendCmd { get; set; }
    }
}
