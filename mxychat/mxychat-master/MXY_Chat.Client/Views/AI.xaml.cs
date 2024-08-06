using MXY_Chat.Client.ViewModels;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// AI.xaml 的交互逻辑
    /// </summary>
    public partial class AI
    {
        public AI()
        {
            InitializeComponent();
            this.DataContext = new AIViewModel();
        }
    }
}
