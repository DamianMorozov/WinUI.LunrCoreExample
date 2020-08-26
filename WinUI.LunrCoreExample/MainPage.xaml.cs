using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.LunrCoreExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Lunr.Index _index = null;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void buttonCreateIndex_ClickAsync(object sender, RoutedEventArgs e)
        {
            textBox.Text = string.Empty;

            _index = await Lunr.Index.Build(async builder =>
            {
                builder
                    .AddField("id")
                    .AddField("body")
                    .AddField("title")
                    .AddField("author");
                var doc = new Lunr.Document
                {
                    { "id", "1" },
                    { "title", "Twelfth-Night" },
                    { "body", "If music be the food of love, play on: Give me excess of it…" },
                    { "author", "William Shakespeare" },
                };
                await builder.Add(doc);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBox.Text += "id: 1" + Environment.NewLine;
                    textBox.Text += "title: Twelfth-Night" + Environment.NewLine;
                    textBox.Text += "body: If music be the food of love, play on: Give me excess of it…" + Environment.NewLine;
                    textBox.Text += "author: William Shakespeare" + Environment.NewLine;
                    textBox.Text += "Created index." + Environment.NewLine;
                });
            });
        }

        private async void buttonSearch_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_index == null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBox.Text += "Must create index." + Environment.NewLine;
                });
                return;
            }
            var search = fieldSearch.Text;
            if (string.IsNullOrEmpty(search))
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBox.Text += "Must fill search field." + Environment.NewLine;
                });
                return;
            }

            textBox.Text = string.Empty;
            await foreach (Lunr.Result result in _index.Search(search))
            {
                // do something with that result
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    textBox.Text += $"result.DocumentReference: {result.DocumentReference}" + Environment.NewLine;
                    textBox.Text += $"result.Score: {result.Score}" + Environment.NewLine;
                    textBox.Text += $"result.MatchData.Term: {result.MatchData.Term}" + Environment.NewLine;
                    textBox.Text += $"result.MatchData.Field: {result.MatchData.Field}" + Environment.NewLine;
                });
            };
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textBox.Text += "Searching completed." + Environment.NewLine;
            });
        }
    }
}
