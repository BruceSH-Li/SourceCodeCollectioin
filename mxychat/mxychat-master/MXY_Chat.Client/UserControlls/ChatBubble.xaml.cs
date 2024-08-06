using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MXY_Chat.Client.UserControlls
{
    /// <summary>
    /// ChatBubble.xaml 的交互逻辑
    /// </summary>
    [TemplatePart(Name = TextBlockTemplateName, Type = typeof(TextBlock))]
    public partial class ChatBubble : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ChatBubble), new PropertyMetadata(default(string), OnTextChanged));

        private const string TextBlockTemplateName = "PART_TextBlock";

        private static readonly Dictionary<string, string> Emotions = new Dictionary<string, string>
        {
            ["doge"] = "pack://application:,,,/WpfQQChat;component/Images/doge.png",
            ["喵喵"] = "pack://application:,,,/WpfQQChat;component/Images/喵喵.png"
        };

        private TextBlock _textBlock;

        static ChatBubble()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatBubble), new FrameworkPropertyMetadata(typeof(ChatBubble)));
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public override void OnApplyTemplate()
        {
            _textBlock = (TextBlock)GetTemplateChild(TextBlockTemplateName);

            UpdateVisual();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ChatBubble)d;

            obj.UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (_textBlock == null)
            {
                return;
            }

            _textBlock.Inlines.Clear();

            var buffer = new StringBuilder();
            foreach (var c in Text)
            {
                switch (c)
                {
                    case '[':
                        _textBlock.Inlines.Add(buffer.ToString());
                        buffer.Clear();
                        buffer.Append(c);
                        break;

                    case ']':
                        var current = buffer.ToString();
                        if (current.StartsWith("["))
                        {
                            var emotionName = current.Substring(1);
                            if (Emotions.ContainsKey(emotionName))
                            {
                                var image = new Image
                                {
                                    Width = 16,
                                    Height = 16,
                                    Source = new BitmapImage(new Uri(Emotions[emotionName]))
                                };
                                _textBlock.Inlines.Add(new InlineUIContainer(image));

                                buffer.Clear();
                                continue;
                            }
                        }

                        buffer.Append(c);
                        _textBlock.Inlines.Add(buffer.ToString());
                        buffer.Clear();
                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

            _textBlock.Inlines.Add(buffer.ToString());
        }
    }
}
