using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Styles.Themes.Extensions;

namespace Styles.Controls
{
    public partial class BaseUserControl :UserControl
    {
        public BaseUserControl()
        {
            DataContext = this;
        }
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int PhoneNumber { get; set; }
            public string Street { get; set; }
            public string Country { get; set; }
            public string Sity { get; set; }
        }
        public List<User> Users { get; set; } = new List<User>
        {
            new User {Id = 1, Name = "John", PhoneNumber = 777666880, Country="Belarus", Sity="Minsk", Street="Kalinovsokgo"},
            new User {Id = 2, Name = "Jane", PhoneNumber = 777666881, Country="Belarus", Sity="Minsk", Street="Kalinovsokgo"},
            new User {Id = 3, Name = "Jimmy", PhoneNumber = 777666882, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 4, Name = "Mary", PhoneNumber = 777666883, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 5, Name = "Simon", PhoneNumber = 777666884, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 6, Name = "Ann", PhoneNumber = 777666885, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 1, Name = "John", PhoneNumber = 777666880, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 2, Name = "Jane", PhoneNumber = 777666881, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 3, Name = "Jimmy", PhoneNumber = 777666882, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 4, Name = "Mary", PhoneNumber = 777666883, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 5, Name = "Simon", PhoneNumber = 777666884, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"},
            new User {Id = 6, Name = "Ann", PhoneNumber = 777666885, Country = "Belarus", Sity = "Minsk", Street = "Kalinovsokgo"}
        };


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var accentBrush = TryFindResource("AccentColorBrush") as SolidColorBrush;
            accentBrush?.Color.CreateAccentColors();
        }
    }
}
