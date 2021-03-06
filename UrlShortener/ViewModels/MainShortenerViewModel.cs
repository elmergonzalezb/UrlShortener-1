﻿using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using UrlShortener.Data;
using UrlShortener.Model;

namespace UrlShortener.ViewModels
{
    public class MainShortenerViewModel : BindableBase
    {
        #region Single Url
        #region API
        private const string OpizoApiKey = "3DD3A7CD39B37BC8CBD9EFEEAC0B03DA";
        private const string PlinkApiKey = "Mhl5uGRbVYUm";
        private const string BitlyApiKey = "R_c597e397b606436fa6a9179626da61bb";
        private const string BitlyLoginKey = "o_1i6m8a9v55";
        private const string CuttlyApiKey = "3f2e3ef2f0597c089cd1af605680c962629c0";
        private const string RebrandlyApiKey = "b54644c5ea5147ea97a7095e354ca2bc";
        private const string MakhlasApiKey = "b64cc0ab-f274-486e-ac23-74dd3e10d9d1";

        #endregion

        #region Command
        public DelegateCommand<TextChangedEventArgs> LongUrlChangedCmd { get; private set; }
        public DelegateCommand<string> ShortenCmd { get; private set; }

        #endregion

        #region Property
        private ObservableCollection<ComboModel> _DataServer = new ObservableCollection<ComboModel>();
        public ObservableCollection<ComboModel> DataServer
        {
            get => _DataServer;
            set => SetProperty(ref _DataServer, value);
        }

        private bool _IsButtonEnable = false;
        public bool IsButtonEnable
        {
            get => _IsButtonEnable;
            set => SetProperty(ref _IsButtonEnable, value);
        }

        private string _SelectedCustomText;
        public string SelectedCustomText
        {
            get => _SelectedCustomText;
            set => SetProperty(ref _SelectedCustomText, value);
        }

        private int _SelectedItemIndex;
        public int SelectedItemIndex
        {
            get => _SelectedItemIndex;
            set
            {
                if (value != _SelectedItemIndex)
                {
                    SetProperty(ref _SelectedItemIndex, value);
                    GlobalData.Config.SelectedIndex = value;
                    GlobalData.Save();
                }
            }
        }

        private string _ShortedUrl;
        public string ShortedUrl
        {
            get => _ShortedUrl;
            set => SetProperty(ref _ShortedUrl, value);
        }

        #endregion

        #endregion

        #region Multiple Url
        #region Command
        public DelegateCommand ButtonOpenFileCmd { get; private set; }
        public DelegateCommand ButtonHelpCmd { get; private set; }
        public DelegateCommand<IList> ButtonStartCmd { get; private set; }
        #endregion

        #region Property

        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set => SetProperty(ref _IsBusy, value);
        }

        private ObservableCollection<ShorterListModel> _ShorterList = new ObservableCollection<ShorterListModel>();
        public ObservableCollection<ShorterListModel> ShorterList
        {
            get => _ShorterList;
            set => SetProperty(ref _ShorterList, value);
        }
        #endregion
        #endregion

        public MainShortenerViewModel()
        {
            #region Single Url
            LongUrlChangedCmd = new DelegateCommand<TextChangedEventArgs>(UrlChanged);
            ShortenCmd = new DelegateCommand<string>(Shorten);

            DataServer.Add(new ComboModel { Name = "Opizo", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "Bitly", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "PLink", IsCustomTextAvailable = true });
            DataServer.Add(new ComboModel { Name = "Cuttly", IsCustomTextAvailable = true });
            DataServer.Add(new ComboModel { Name = "Rebrandly", IsCustomTextAvailable = true });
            DataServer.Add(new ComboModel { Name = "TinyUrl [Need VPN From Iran]", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "Makhlas", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "ReLink", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "Shrturi", IsCustomTextAvailable = false });
            DataServer.Add(new ComboModel { Name = "Shrtco", IsCustomTextAvailable = false });
            #endregion

            #region Multiple Url
            ButtonOpenFileCmd = new DelegateCommand(OpenFile);
            ButtonHelpCmd = new DelegateCommand(Help);
            ButtonStartCmd = new DelegateCommand<IList>(Start);
            #endregion

            SelectedItemIndex = GlobalData.Config.SelectedIndex;
        }

        #region Multiple Url
        private async void Start(IList shorterSelectedList)
        {
            try
            {
                IsBusy = true;
                for (int i = 0; i < shorterSelectedList.Count; i++)
                {
                    ShorterListModel selectedItem = shorterSelectedList[i] as ShorterListModel;
                    string longLink = selectedItem.Link;

                    switch (SelectedItemIndex)
                    {
                        case 0:
                            ShorterList.Add(new ShorterListModel { ShortLink = await OpizoShorten(longLink) });
                            break;

                        case 1:
                            ShorterList.Add(new ShorterListModel { ShortLink = await BitlyShorten(longLink) });
                            break;

                        case 2:
                            ShorterList.Add(new ShorterListModel { ShortLink = await PlinkShorten(longLink) });
                            break;
                        case 3:
                            ShorterList.Add(new ShorterListModel { ShortLink = await CuttlyShorten(longLink) });
                            break;
                        case 4:
                            ShorterList.Add(new ShorterListModel { ShortLink = await RebrandlyShorten(longLink) });
                            break;
                        case 5:
                            ShorterList.Add(new ShorterListModel { ShortLink = await TinyUrlShorten(longLink) });
                            break;
                        case 6:
                            ShorterList.Add(new ShorterListModel { ShortLink = await MakhlasShorten(longLink) });
                            break;
                        case 7:
                            ShorterList.Add(new ShorterListModel { ShortLink = await RelLinkShorten(longLink) });
                            break;
                        case 8:
                            ShorterList.Add(new ShorterListModel { ShortLink = await ShrturiShorten(longLink) });
                            break;
                        case 9:
                            ShorterList.Add(new ShorterListModel { ShortLink = await ShrtcoShorten(longLink) });
                            break;
                    }
                }
                IsBusy = false;
                SaveFileDialog oDialog = new SaveFileDialog
                {
                    Title = "Save Text File",
                    Filter = "TXT files|*.txt"
                };
                if (oDialog.ShowDialog() == true)
                {
                    File.WriteAllLines(oDialog.FileName, ShorterList.Where(x => x.ShortLink != null).Select(x => x.ShortLink));
                }

                ShorterList = new ObservableCollection<ShorterListModel>();
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Help()
        {
            Growl.Info(new GrowlInfo
            {
                Message =
                    "Step 1: Create Txt File like this(each line one Link): http://Test.com   http://Test.com   Step 2: Load txt File   Step 3: Choose Url Shorten Service    Step 4: Select urls   Step 5: Click Start",
                ShowDateTime = false
            });
        }

        private void OpenFile()
        {
            ShorterList.Clear();
            OpenFileDialog theDialog = new OpenFileDialog
            {
                Title = "Open Text File",
                Filter = "TXT files|*.txt"
            };
            if (theDialog.ShowDialog() == true)
            {
                string filename = theDialog.FileName;

                string[] filelines = File.ReadAllLines(filename);

                foreach (string item in filelines)
                {
                    ShorterList.Add(new ShorterListModel { Link = item });
                }
            }
        }
        #endregion

        #region Single Url
        private void UrlChanged(TextChangedEventArgs e)
        {
            if (e.OriginalSource is HandyControl.Controls.TextBox item)
            {
                if (string.IsNullOrEmpty(item.Text))
                {
                    IsButtonEnable = false;
                }
                bool result = Uri.TryCreate(item.Text, UriKind.Absolute, out Uri uriResult)
                                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                IsButtonEnable = result;
            }

        }

        private async void Shorten(string longUrl)
        {
            IsButtonEnable = false;
            switch (SelectedItemIndex)
            {
                case 0:
                    ShortedUrl = await OpizoShorten(longUrl);
                    break;

                case 1:
                    ShortedUrl = await BitlyShorten(longUrl);
                    break;

                case 2:
                    ShortedUrl = await PlinkShorten(longUrl);
                    break;

                case 3:
                    ShortedUrl = await CuttlyShorten(longUrl);
                    break;

                case 4:
                    ShortedUrl = await RebrandlyShorten(longUrl);
                    break;

                case 5:
                    ShortedUrl = await TinyUrlShorten(longUrl);
                    break;

                case 6:
                    ShortedUrl = await MakhlasShorten(longUrl);
                    break;
                case 7:
                    ShortedUrl = await RelLinkShorten(longUrl);
                    break;
                case 8:
                    ShortedUrl = await ShrturiShorten(longUrl);
                    break;
                case 9:
                    ShortedUrl = await ShrtcoShorten(longUrl);
                    break;
            }
            IsButtonEnable = true;
            if (!ShortedUrl.Contains("error"))
            {
                Clipboard.SetText(ShortedUrl);
            }
        }

        public async Task<string> OpizoShorten(string longUrl)
        {
            try
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("url", longUrl)
                });

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-API-KEY", OpizoApiKey);
                HttpResponseMessage response = await client.PostAsync("https://opizo.com/api/v1/shrink/", content);
                dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                string status = root.status;

                if (status.Equals("success"))
                {
                    string link = root.data.url;
                    return link;
                }
                else
                {
                    string error = root.data;
                    Growl.Error(error);
                }
            }
            catch (Exception ex)
            {

                Growl.Error(ex.Message);

            }


            return "error";
        }

        public async Task<string> PlinkShorten(string longUrl)
        {
            try
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                string utf8Encoded = HttpUtility.UrlEncode(longUrl, utf8);
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync("https://plink.ir/api?api=" + PlinkApiKey + "& url=" + utf8Encoded +
                                             "&custom=" + SelectedCustomText))
                using (HttpContent content = response.Content)
                {
                    dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    string error = root.error;
                    if (error.Contains("0"))
                    {
                        string link = root.@short;
                        return link;
                    }
                    else
                    {
                        string msg = root.msg;
                        Growl.Error(msg);
                    }


                }
            }
            catch (Exception ex)
            {

                Growl.Error(ex.Message);

            }

            return "error";
        }

        public async Task<string> BitlyShorten(string longUrl)
        {

            try
            {
                string url = string.Format("http://api.bit.ly/shorten?format=json&longUrl={0}&login={1}&apiKey={2}", HttpUtility.UrlEncode(longUrl), BitlyLoginKey, BitlyApiKey);

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                    string statusCode = root.statusCode;

                    if (statusCode.Equals("OK"))
                    {
                        string link = root.results[longUrl].shortUrl;

                        return link;
                    }
                    else
                    {
                        string error = root.errorMessage;
                        Growl.Error(error);
                    }

                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }


            return "error";
        }

        public async Task<string> CuttlyShorten(string longUrl)
        {

            try
            {
                string url = string.Format("https://cutt.ly/api/api.php?key={0}&short={1}&name={2}", CuttlyApiKey, HttpUtility.UrlEncode(longUrl), SelectedCustomText);

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                    string statusCode = root.url.status;
                    if (statusCode.Equals("7"))
                    {
                        string link = root.url.shortLink;

                        return link;
                    }
                    else
                    {
                        string error = root.status;
                        Growl.Error(error);
                    }

                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }


            return "error";
        }

        public async Task<string> RebrandlyShorten(string longUrl)
        {
            try
            {
                var data = new { destination = longUrl, slashtag = SelectedCustomText };

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", RebrandlyApiKey);
                HttpResponseMessage response = await client.PostAsync("https://api.rebrandly.com/v1/links", data.AsJson());
                dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                string link = root.shortUrl;

                if (link != null)
                {
                    return link;
                }
                else
                {
                    string error = root.message;
                    Growl.Error(error);
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }

            return "error";
        }

        public async Task<string> TinyUrlShorten(string longUrl)
        {

            try
            {
                string url = string.Format("https://tinyurl.com/api-create.php?url={0}", HttpUtility.UrlEncode(longUrl));

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    string root = response.Content.ReadAsStringAsync().Result;

                    if (!root.Contains("Error"))
                    {
                        return root;
                    }
                    else
                    {
                        Growl.Error(root);
                    }

                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }


            return "error";
        }

        public async Task<string> MakhlasShorten(string longUrl)
        {
            try
            {
                var data = new { long_url = longUrl };

                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", $"Bearer {MakhlasApiKey}");

                HttpResponseMessage response = await client.PostAsync(
                    $"http://api.makhlas.com/v1/url", data.AsJson());
                dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                string status = root.success;
                if (status.ToLower().Contains("true"))
                {
                    string link = root.data[0].short_url;
                    return link;
                }
                else
                {
                    string error = root.detail;
                    Growl.Error(error);
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }

            return "error";
        }

        public async Task<string> RelLinkShorten(string longUrl)
        {
            try
            {
                var data = new { url = longUrl };

                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync("https://rel.ink/api/links/", data.AsJson());
                dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                string link = root.hashid;
                if (link != null)
                {
                    return $"https://rel.ink/{link}";
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }

            return "error";
        }

        public async Task<string> ShrturiShorten(string longUrl)
        {
            try
            {
                var data = new { url = longUrl };

                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync("https://shrturi.com/api/v1/shorten", data.AsJson());
                dynamic root = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                string link = root.result_url;
                if (link != null)
                {
                    return link;
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }

            return "error";
        }

        public async Task<string> ShrtcoShorten(string longUrl)
        {

            try
            {
                string url = string.Format("https://api.shrtco.de/v2/shorten?url={0}", HttpUtility.UrlEncode(longUrl));

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    string root = response.Content.ReadAsStringAsync().Result;
                    dynamic parse = JsonConvert.DeserializeObject<dynamic>(root);
                    string status = parse.ok;
                    if (!status.Contains("true"))
                    {
                        string link = parse.result.full_short_link;
                        return link;
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error(ex.Message);
            }


            return "error";
        }

        #endregion
    }
}
