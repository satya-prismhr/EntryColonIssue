using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace MAUICarousel;

public partial class MainPage : ContentPage
{
    private ItemViewModel ItemViewModelData
    {
        get
        {
            return BindingContext as ItemViewModel;
        }
    }
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ItemViewModel();
        ItemViewModelData.UpdateCarouselview = UpdateData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ItemViewModelData.LoadData();
    }

    private void UpdateData(int obj)
    {
        carouselview.CurrentItem = ItemViewModelData.Items[obj];
        carouselview.Position = obj;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if ((sender as Entry) != null && e.OldTextValue != null)
        {
            var entry = (Entry)sender;
            string entryText = entry.Text;

            if (entryText == null)
                return;

            if (entry.Text.Length > 5)
            {
                entryText = entryText.Remove(entryText.Length - 1);
                entry.Text = entryText;
                if (entryText.ToLower().Equals(e.OldTextValue.ToLower()))
                {
                    return;
                }
            }

            if (e.NewTextValue.Length >= e.OldTextValue.Length)
            {
                if (entryText.Contains(":"))
                {
                    if (e.NewTextValue.EndsWith(":"))
                    {
                        //Only Colon added forcefully so returning from here
                        return;
                    }
                    else
                    {
                        string[] strings = entryText.Split(':');
                        string hours = FormatHours(strings[0], true);
                        if (strings.Length > 1)
                        {
                            if (!string.IsNullOrWhiteSpace(strings[1]))
                            {
                                string minutes = FormatMinutes(strings[1]);
                                entry.Text = string.Format("{0}:{1}", hours, minutes);
                            }
                        }
                    }
                }
                else
                {
                    if (e.NewTextValue.ToLower().Equals(e.OldTextValue.ToLower()))
                    {
                        //Setting same value forcefully so return from here
                        return;
                    }
                    else
                    {
                        if (entryText.Length == 3 && !entryText.Contains(":"))
                        {
                            string minute = entryText.Substring(2);
                            entry.Text = string.Format("{0}:{1}", e.OldTextValue, FormatMinutes(minute));
                        }
                        else
                        {
                            string hours = FormatHours(entryText, true);
                            if (hours.Length == 2)
                            {
                                entry.Text = string.Format("{0}:", hours);
                            }
                            else
                            {
                                entry.Text = hours;
                            }
                        }
                    }
                }
            }
            if (entry.Text.Length == 3 && entry.Text.Contains(":") && e.OldTextValue.Length > e.NewTextValue.Length)
            {
                string text = entry.Text.Remove(entry.Text.Length - 1);
                entry.Text = text;
            }
        }
    }
    private string FormatHours(string hours, bool is12HoursTime)
    {
        if (string.IsNullOrEmpty(hours))
            return string.Empty;
        if (hours.Length > 2)
            hours = hours.Substring(0, 2);
        int hh;
        if (int.TryParse(hours, out hh))
        {
            string newHours;
            if (is12HoursTime)
            {
                int remender = hh % 12;
                if (remender > 1 && remender < 10 && hh < 12)
                {
                    newHours = string.Format("0{0}", hh);
                }
                else
                {
                    newHours = hh > 12 ? 12.ToString() : hours;
                }
            }
            else
            {
                int remender = hh % 24;
                if (remender > 2 && remender < 10 && hh < 23)
                {
                    newHours = string.Format("0{0}", hh);
                }
                else
                {
                    newHours = hh > 23 ? 23.ToString() : hours;
                }
            }
            return newHours;
        }
        else
        {
            return hours;
        }
    }

    private string FormatMinutes(string minutes)
    {
        if (string.IsNullOrEmpty(minutes))
            return string.Empty;
        if (minutes.Length > 2)
            minutes = minutes.Substring(0, 2);
        int mm;
        if (int.TryParse(minutes, out mm))
        {
            string newMinutes;
            int remender = mm % 60;
            if (remender > 5 && remender < 10 && mm < 59)
            {
                newMinutes = string.Format("0{0}", mm);
            }
            else
            {
                newMinutes = mm > 59 ? 59.ToString() : minutes;
            }
            return newMinutes;
        }
        else
        {
            return minutes;
        }
    }
}
public class ItemModel
{
    public string Name { get; set; }
    public string Location { get; set; }
}

public class ItemViewModel : BindableObject
{
    public Action<int> UpdateCarouselview { get; set; }
    public ItemViewModel()
    {

    }


    public void LoadData()
    {
        Items.Clear();
        for (int i = 1; i < 10; i++)
        {
            Items.Add(new ItemModel { Name = "Monkey " + i, Location = "Location" + i });
        }
        UpdateCarouselview?.Invoke(2);
    }

    private ObservableCollection<ItemModel> _items = new ObservableCollection<ItemModel>();
    public ObservableCollection<ItemModel> Items
    {
        get { return _items; }
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }
    }
}

